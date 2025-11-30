using HMSBAL.Common;
using HMSBAL.Interfaces.Auth;
using HMSDAL.Common;
using HMSDAL.Interfaces.Auth;
using HMSMAL.Auth;
using HMSMAL.Common;
using HMSMAL.DTO.Auth;
using HMSMAL.POCO;
using HMSMALPOCO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.IdentityModel.Tokens;
using NLog;
using ServiceStack.OrmLite;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;

namespace HMSBAL.Services.Auth
{
    /// <summary>
    /// Handler class for login operations
    /// </summary>
    public class BLLoginHandler : ILoginService
    {
        #region Private Members

        /// <summary>
        /// Stores instance of Response class
        /// </summary>
        Response objResponse;

        /// <summary>
        /// Stores instance of Response class
        /// </summary>
        Invitations pocoInvitation { get; set; }

        /// <summary>
        /// Stores instance of Registration class
        /// </summary>
        Registration pocoRegistration { get; set; }

        /// <summary>
        /// Stores instance of Users class
        /// </summary>
        Users pocoUser { get; set; }

        /// <summary>
        /// Stores instance of UserRights class
        /// </summary>
        UserRights pocoUserRights { get; set; }

        /// <summary>
        /// Stores instance of Logger class
        /// </summary>
        Logger _logger;

        /// <summary>
        /// Stores instance of IDistributedCache
        /// </summary>
        readonly IDistributedCache _cache;

        /// <summary>
        /// Stores instance of ILoginRepository interface
        /// </summary>
        ILoginRepository _dbloginRepo;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes necessary dependencies,services and objects
        /// </summary>
        /// <param name="cache">Instance of IDistributedCache</param>
        /// <param name="loginRepo">Instance of ILoginRepository</param>
        public BLLoginHandler(IDistributedCache cache, ILoginRepository loginRepo)
        {
            objResponse = new();
            _cache = cache;
            _dbloginRepo = loginRepo;
            _logger = LogManager.GetLogger("appLog");
        }

        #endregion

        #region Public Methods

        #region Get Data

        /// <summary>
        /// Retrieves the list of available user roles
        /// </summary>
        /// <returns></returns>
        public Response GetRoles()
        {
            List<Roles> lstRoles = new();

            lstRoles = _dbloginRepo.GetRoles();

            if (lstRoles.Count > 0)
            {
                objResponse.Data = lstRoles;
            }
            else
            {
                objResponse.ErrorCode = EnmErrorCodes.E0003;
            }

            return objResponse;
        }

        /// <summary>
        /// Retrieves the list of available departments
        /// </summary>
        /// <returns></returns>
        public Response GetDepartments()
        {
            List<Departments> lstDepts = new();

            using (var db = new MySqlOrmLite().Open())
            {
                lstDepts = db.Select<Departments>();
            }

            if (lstDepts.Count > 0)
            {
                objResponse.Data = lstDepts;
            }
            else
            {
                objResponse.ErrorCode = EnmErrorCodes.E0003;
            }

            return objResponse;
        }

        #endregion

        #region Invitation

        /// <summary>
        /// Invites a user by processing the provided invitation details
        /// </summary>
        /// <param name="objInv">Instance of DTOInvitation class</param>
        /// <returns></returns>
        public Response Invite(DTOInvitation objInv)
        {
            objResponse = ValidateInvitation(objInv);

            if (!objResponse.IsError)
            {
                PreSaveInvitation(objInv);

                objResponse = SaveInvitation();
            }

            return objResponse;
        }

        /// <summary>
        /// Validate invitation details
        /// </summary>
        /// <param name="objInv">Instace of DTOInvitation class</param>
        /// <returns>Response Model</returns>
        public Response ValidateInvitation(DTOInvitation objInv)
        {
            bool isExists = false;

            using (var db = new MySqlOrmLite().Open())
            {
                isExists = db.Exists<Invitations>(_inv => (_inv.Contact == objInv.Contact || _inv.Email == objInv.Email) &&
                                                          _inv.RoleId == objInv.RoleId && _inv.DepartmentId == objInv.DepartmentId);
            }

            if (isExists)
            {
                objResponse.ErrorCode = EnmErrorCodes.INV01;
            }

            return objResponse;
        }

        /// <summary>
        /// Prepares invitation details to be saved
        /// </summary>
        /// <param name="objInv">Instance of DTOInvitation class</param>
        public void PreSaveInvitation(DTOInvitation objInv)
        {
            pocoInvitation = new();

            BLGlobalClass.CopyProperties<DTOInvitation, Invitations>(objInv, pocoInvitation);

            pocoInvitation.InvCode = GenerateInvitationCode();
            pocoInvitation.CreatedBy = LoggedInUserDetails.UserId;
        }

        /// <summary>
        /// Generates unique invitation code
        /// </summary>
        /// <returns>Invitation code</returns>
        public int GenerateInvitationCode()
        {
            int code;
            bool exists;

            do
            {
                code = new Random().Next(10000000, 99999999);

                using (var db = new MySqlOrmLite().Open())
                {
                    exists = db.Exists<Invitations>(_inv => _inv.InvCode == code);
                }

            }
            while (exists);

            return code;
        }

        /// <summary>
        /// Saves invitation details and sends invitation email
        /// </summary>
        /// <returns></returns>
        public Response SaveInvitation()
        {
            try
            {
                using (var db = new MySqlOrmLite().Open())
                {
                    db.Insert<Invitations>(pocoInvitation);
                }

                objResponse = SendInvitationEmail(pocoInvitation.Email, pocoInvitation.InvCode.ToString(), DateTime.Now.AddDays(30));

                if (!objResponse.IsError)
                {
                    objResponse.Message = SuccessMessages.S0001;
                }
            }
            catch (Exception ex)
            {
                objResponse.ErrorCode = EnmErrorCodes.E0001;

                _logger = LogManager.GetLogger("errorLog");
                _logger.Info($"SaveInvitation : {ex.Message}");
            }

            return objResponse;
        }

        /// <summary>
        /// Sends invitation email to the invited user
        /// </summary>
        /// <param name="email">Email of invited user</param>
        /// <param name="invCode">Invitation code of invited user</param>
        /// <param name="expiry">Email expiry date</param>
        /// <returns></returns>
        private Response SendInvitationEmail(string email, string invCode, DateTime expiry)
        {
            string frontendUrl = BLGlobalClass.GetSetting("AppSettings:FrontendUrl"),
                   registrationLink = $"{frontendUrl}/Register.html?invCode={invCode}",
                   subject = "HMS Registration Invitation",
                   body = $@"<p>Hello,</p>
                             <p>You have been invited to register on the <b>Hospital Management System</b>.</p>
                             <p><a href='{registrationLink}' target='_blank'>Click here to register</a></p>
                             <p>This link will expire on {expiry:dd MMM yyyy HH:mm}.</p>
                             <br/>
                             <p>Regards,<br/>HMS Admin</p>";

            try
            {
                using (var client = new SmtpClient(BLGlobalClass.GetSetting("Email:SmtpHost"), int.Parse(BLGlobalClass.GetSetting("Email:SmtpPort"))))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(BLGlobalClass.GetSetting("Email:User"), BLGlobalClass.GetSetting("Email:Password"));

                    var mail = new MailMessage
                    {
                        From = new MailAddress(BLGlobalClass.GetSetting("Email:User"), "HMS Admin"),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };
                    mail.To.Add(email);

                    client.Send(mail);
                }

            }
            catch (Exception ex)
            {
                objResponse.ErrorCode = EnmErrorCodes.INV02;

                _logger = LogManager.GetLogger("errorLog");
                _logger.Info($"SendInvitationEmail : {ex.Message}");
            }

            return objResponse;
        }

        #endregion

        #region Registration

        /// <summary>
        /// Registers a new user based on the provided registration details
        /// </summary>
        /// <param name="objRegistration">Instance Registration of class</param>
        /// <returns></returns>
        public Response Register(Registration objRegistration)
        {
            objResponse = ValidateRegistration(objRegistration);

            if (!objResponse.IsError)
            {
                PreSaveRegistration(objRegistration);
                objResponse = SaveRegistration();
            }

            return objResponse;
        }

        /// <summary>
        /// Validates registration details
        /// </summary>
        /// <param name="objRegistration">Instance of Registration class</param>
        /// <returns></returns>
        public Response ValidateRegistration(Registration objRegistration)
        {
            using (var db = new MySqlOrmLite().Open())
            {
                pocoInvitation = db.Single<Invitations>(_inv => _inv.InvCode == objRegistration.InvCode);

                if (pocoInvitation == null)
                {
                    objResponse.ErrorCode = EnmErrorCodes.REG01;
                }
            }

            return objResponse;
        }

        /// <summary>
        /// Prepares registration details to be saved
        /// </summary>
        /// <param name="objRegistration">Instance of Registration class</param>
        public void PreSaveRegistration(Registration objRegistration)
        {
            pocoRegistration = objRegistration;
            PrepareUser();
            PrepareUserRights();
        }

        /// <summary>
        /// Preapres user details to be saved
        /// </summary>
        public void PrepareUser()
        {
            pocoUser = new()
            {
                UserName = pocoRegistration.Username,
                Email = pocoInvitation.Email,
                Password = BLGlobalClass.HashPassword(pocoRegistration.Password),
                RoleId = pocoInvitation.RoleId,
                DepartmentId = pocoInvitation.DepartmentId,
                Contact = pocoInvitation.Contact,
                IsActive = 1,
                CreatedBy = pocoInvitation.CreatedBy,
                CreatedOn = DateTime.Now,
            };
        }

        /// <summary>
        /// Prepares user rights details to be saved
        /// </summary>
        public void PrepareUserRights()
        {
            pocoUserRights = new()
            {
                RoleId = pocoInvitation.RoleId,
                CreatedOn = DateTime.Now,
            };
        }

        /// <summary>
        /// Saves registration details
        /// </summary>
        /// <returns></returns>
        public Response SaveRegistration()
        {
            try
            {
                using (var db = new MySqlOrmLite().Open())
                {
                    int userId = (int)db.Insert(pocoUser,selectIdentity:true);

                    pocoUserRights.UserId = userId;

                    db.Insert(pocoUserRights);
                }

                objResponse.Message = SuccessMessages.S0002;
            }
            catch (Exception ex)
            {
                objResponse.ErrorCode = EnmErrorCodes.REG02;

                _logger = LogManager.GetLogger("errorLog");
                _logger.Info($"SaveRegistration : {ex.Message}");
            }

            return objResponse;
        }

        #endregion

        #region Login

        /// <summary>
        /// Logs in a user by validating the provided login credentials, generates JWT Token, saves user details in cache
        /// </summary>
        /// <param name="objLogin"></param>
        /// <returns></returns>
        public Response Login(Login objLogin)
        {
            bool isInvalidLoginId = false, isInvalidPassword = false;

            string role = string.Empty, department = string.Empty;

            Users objUser = new();

            objLogin.Password = BLGlobalClass.HashPassword(objLogin.Password);
            _logger.Info($"Password hashed : {objLogin.Password}");

            using (var db = new MySqlOrmLite().Open())
            {
                objUser = db.Single<Users>(_user => _user.Email == objLogin.Email && _user.Password == objLogin.Password && _user.IsActive == 1);
                
                if (objUser == null)
                {
                    objResponse.ErrorCode = EnmErrorCodes.LOG01;
                    return objResponse;
                }
                else if (objLogin.Email != objUser.Email)
                {
                    objResponse.ErrorCode = EnmErrorCodes.LOG02;
                    return objResponse;
                }
                else if (objLogin.Password != objUser.Password)
                {
                    objResponse.ErrorCode = EnmErrorCodes.LOG03;
                    return objResponse;
                }
                else
                {
                    role = db.Scalar<string>(db.From<Roles>()
                                               .Where(_role => _role.RoleId == objUser.RoleId)
                                               .Select(_role => _role.RoleName));

                    department = db.Scalar<string>(db.From<Departments>()
                                                     .Where(_dept => _dept.DepartmentId == objUser.DepartmentId)
                                                     .Select(_dept => _dept.DepartmentName));
                }
            }

            // Example user info from database

            LoggedInUserDetails.UserId = objUser.UserId;
            LoggedInUserDetails.UserName = objUser.UserName;
            LoggedInUserDetails.Email = objUser.Email;
            LoggedInUserDetails.RoleId = objUser.RoleId;
            LoggedInUserDetails.DepartmentId = objUser.DepartmentId;
            LoggedInUserDetails.Role = role;
            LoggedInUserDetails.Department = (!String.IsNullOrEmpty(department)) ? department : string.Empty;

            // Generate JWT Token
            string token = GenerateJwtToken();

            // Store in Redis Cache (key: token)
            string cacheKey = $"HMS_session_{token}";
            string cacheData = $"UserId={LoggedInUserDetails.UserId};UserName={LoggedInUserDetails.UserName};Role={LoggedInUserDetails.Role};Dept={LoggedInUserDetails.Department}";
            _cache.SetString(cacheKey, cacheData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1)
            });

            objResponse.Message = SuccessMessages.S0003;
            objResponse.Token = token;
            objResponse.Data = new
            {
                UserId = LoggedInUserDetails.UserId,
                UserName = LoggedInUserDetails.UserName,
                Email = LoggedInUserDetails.Email,
                RoleId = LoggedInUserDetails.RoleId,
                DepartmentId = LoggedInUserDetails.DepartmentId,
                Role = LoggedInUserDetails.Role,
                Department = LoggedInUserDetails.Department,
            };

            return objResponse;
        }

        /// <summary>
        /// Generates JWT Token for authenticated user
        /// </summary>
        /// <returns></returns>
        private string GenerateJwtToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(BLGlobalClass.GetSetting("Jwt:Key")));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, LoggedInUserDetails.UserName),
                new Claim("id",Convert.ToString(LoggedInUserDetails.UserId)),
                new Claim(ClaimTypes.Role, LoggedInUserDetails.Role),
                new Claim("dept", LoggedInUserDetails.Department),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: BLGlobalClass.GetSetting("Jwt:Issuer"),
                audience: BLGlobalClass.GetSetting("Jwt:Audience"),
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion

        #region Log Out

        /// <summary>
        /// Logs out the currently authenticated user and clears their session(clears cache session) or authentication token
        /// </summary>
        /// <param name="requestHeaders">Current HTTP Request Headers</param>
        /// <returns></returns>
        public Response Logout(IHeaderDictionary requestHeaders)
        {
            try
            {
                string authHeader = requestHeaders["Authorization"];

                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    objResponse.ErrorCode = EnmErrorCodes.LOG04; 
                    return objResponse;
                }

                string token = authHeader.Replace("Bearer ", "").Trim();
                string cacheKey = $"HMS_session_{token}";
                _cache.Remove(cacheKey); 

                LoggedInUserDetails.UserId = 0;
                LoggedInUserDetails.UserName = string.Empty;
                LoggedInUserDetails.Email = string.Empty;
                LoggedInUserDetails.RoleId = 0;
                LoggedInUserDetails.DepartmentId = 0;
                LoggedInUserDetails.Role = string.Empty;
                LoggedInUserDetails.Department = string.Empty;

                objResponse.Message = SuccessMessages.S0006;
            }
            catch (Exception ex)
            {
                objResponse.IsError = true;
                objResponse.Message = ex.Message;
            }

            return objResponse;
        }

        #endregion

        #endregion
    }
}


