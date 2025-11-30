using HMSBAL.Interfaces.User;
using HMSDAL.Common;
using HMSDAL.Interfaces.User;
using HMSMAL.Auth;
using HMSMAL.Common;
using HMSMAL.DTO.User;
using HMSMAL.POCO;
using HMSMALPOCO;
using NLog;
using ServiceStack.OrmLite;

namespace HMSBAL.Services.User
{
    /// <summary>
    /// Handler class for user rights management
    /// </summary>
    public class BLUserRightsHandler : IUserRightsService
    {
        #region Private Members

        /// <summary>
        /// Stores instance of Response class
        /// </summary>
        Response _response;

        /// <summary>
        /// Stores instance of IUserRightsRepository interface
        /// </summary>

        IUserRightsRepository _userRightsRepository;

        /// <summary>
        /// Stores instance of Logger class
        /// </summary>
        Logger _logger;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes necessary services/objects
        /// </summary>
        /// <param name="userRightsRepository"></param>
        public BLUserRightsHandler(IUserRightsRepository userRightsRepository)
        {
            _response = new();
            _userRightsRepository = userRightsRepository;
            _logger = LogManager.GetLogger("appLog");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Retrieves the list of menus available to the current user
        /// </summary>
        /// <returns></returns>
        public Response GetUserMenus()
        {
            List<Menus> lstMenus = new();
            List<int> lstFinalRights = new();

            lstMenus = _userRightsRepository.GetUserMenus(LoggedInUserDetails.RoleId, LoggedInUserDetails.DepartmentId);

            lstFinalRights = GetFinalRights(LoggedInUserDetails.UserId, LoggedInUserDetails.RoleId);

            // 4️⃣ Filter Menus where ViewActivityId is in final rights
            var accessibleMenus = lstMenus
                .Where(m => lstFinalRights.Contains(m.ViewActivityId))
                .ToList();

            _response.Data = accessibleMenus;

            return _response;
        }

        /// <summary>
        /// Retrieves a list of users available to the current user to manage rights
        /// </summary>
        /// <returns></returns>
        public Response GetUsers()
        {
            List<Users> lstUsers = new();
            List<Roles> lstRoles = new();

            try
            {
                using (var db = new MySqlOrmLite().Open())
                {
                    lstUsers = db.Select<Users>(_user => _user.CreatedBy == LoggedInUserDetails.UserId && _user.IsActive == 1);
                    lstRoles = db.Select<Roles>();
                }

                if (lstUsers.Count > 0 && lstRoles.Count > 0)
                {
                    _response.Data = lstUsers.Select(u => new
                    {
                        u.UserId,
                        u.RoleId,
                        u.UserName,
                        Role = lstRoles.FirstOrDefault(r => r.RoleId == u.RoleId)?.RoleName,
                        Rights = GetFinalRights(u.UserId, u.RoleId)
                    }).ToList();
                }
                else
                {
                    _response.ErrorCode = EnmErrorCodes.E0003;
                }

            }
            catch (Exception ex)
            {
                _response.ErrorCode = EnmErrorCodes.E0003;
                _response.Message = ex.Message;

                _logger.Error(ex, "Error in GetUsers: " + ex.Message);

                throw ex;
            }

            return _response;
        }
        /// <summary>
        /// Retrieves the list of activities available to a specified user based on their role.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="roleId">Role Id</param>
        /// <returns></returns>
        public Response GetUserActivities(int userId, int roleId)
        {
            List<Activities> lstActivities = new();

            try
            {
                using (var db = new MySqlOrmLite().Open())
                {
                    if (roleId == 2)
                    {
                        int deptId = db.Scalar<int>(db.From<Users>()
                                                 .Where(_user => _user.UserId == userId && _user.RoleId == roleId)
                                                 .Select(_user => _user.DepartmentId));

                        lstActivities = db.Select<Activities>(_activity => _activity.DepartmentIdsCsv.Contains(deptId.ToString())
                                                                        && _activity.RoleIdsCsv.Contains(roleId.ToString())
                                                                        && _activity.IsActive == 1);
                    }
                    else
                    {
                        lstActivities = db.Select<Activities>(_activity => _activity.RoleIdsCsv.Contains(roleId.ToString())
                                                                        && _activity.IsActive == 1);
                    }
                }

                if (lstActivities.Count > 0)
                {
                    _response.Data = lstActivities.OrderBy(_activity => _activity.DisplayOrder);
                }
                else
                {
                    _response.ErrorCode = EnmErrorCodes.E0003;
                }

            }
            catch (Exception ex)
            {
                _response.ErrorCode = EnmErrorCodes.E0003;
                _response.Message = ex.Message;

                _logger.Error(ex, "Error in GetUserActivities: " + ex.Message);

                throw ex;
            }

            return _response;

        }

        /// <summary>
        /// Saves the user rights for a specified user
        /// </summary>
        /// <param name="objDTOUserRights">Instace of DTOUserRights class</param>
        /// <returns></returns>
        public Response SaveUserRights(DTOUserRights objDTOUserRights)
        {
            try
            {
                using (var db = new MySqlOrmLite().Open())
                {
                    // Get existing record
                    UserRights record = db.Single<UserRights>(_ur => _ur.UserId == objDTOUserRights.UserId && _ur.RoleId == objDTOUserRights.RoleId);

                    if (record == null)
                    {
                        _response.ErrorCode = EnmErrorCodes.E0002;
                    }

                    objDTOUserRights.AddRights = string.IsNullOrEmpty(objDTOUserRights.AddRights) ? record.RightsIdsCsv : record.RightsIdsCsv + "," + objDTOUserRights.AddRights;
                    objDTOUserRights.RemoveRights = string.IsNullOrEmpty(objDTOUserRights.RemoveRights) ? record.RemovedRightsIdsCsv : record.RemovedRightsIdsCsv + "," + objDTOUserRights.RemoveRights;

                    db.UpdateOnly(() => new UserRights
                    {
                        RightsIdsCsv = objDTOUserRights.AddRights,
                        RemovedRightsIdsCsv = objDTOUserRights.RemoveRights
                    },
                    where: _ur => _ur.UserId == objDTOUserRights.UserId && _ur.RoleId == objDTOUserRights.RoleId);

                    _response.Message = SuccessMessages.S0004;
                }
            }
            catch (Exception ex)
            {
                _response.ErrorCode = EnmErrorCodes.E0002;
                _response.Message = ex.Message;
                _logger.Error(ex, "Error in SaveUserRights");
                throw;
            }

            return _response;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Convert csv to list of integers
        /// </summary>
        /// <param name="csv">String of rights csv</param>
        /// <returns></returns>
        private List<int> ToIntList(string csv)
        {
            if (string.IsNullOrWhiteSpace(csv)) return new List<int>();
            return csv.Split(',', StringSplitOptions.RemoveEmptyEntries)
                      .Select(s => int.TryParse(s.Trim(), out var v) ? v : 0)
                      .Where(v => v > 0)
                      .ToList();
        }

        /// <summary>
        /// Prepares final rights list for given user id and role id
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="roleId">Role Id</param>
        /// <returns></returns>
        private List<int> GetFinalRights(int userId, int roleId)
        {
            List<int> lstDefaultRights = new(), lstAddedRights = new(), lstRemovedRights = new(), lstFinalRights = new();
            string defaultRights = string.Empty, addedRights = string.Empty, removedRights = string.Empty;

            using (var db = new MySqlOrmLite().Open())
            {
                defaultRights = db.Scalar<string>(db.From<Roles>()
                                                 .Where(_role => _role.RoleId == roleId)
                                                 .Select(_role => _role.DefaultRightsCsv));

                addedRights = db.Scalar<string>(db.From<UserRights>()
                                                 .Where(_urg => _urg.UserId == userId && _urg.RoleId == roleId)
                                                 .Select(_urg => _urg.RightsIdsCsv));

                removedRights = db.Scalar<string>(db.From<UserRights>()
                                                 .Where(_urg => _urg.UserId == userId && _urg.RoleId == roleId)
                                                 .Select(_urg => _urg.RemovedRightsIdsCsv));
            }

            lstDefaultRights = ToIntList(defaultRights);
            lstAddedRights = ToIntList(addedRights);
            lstRemovedRights = ToIntList(removedRights);

            // 3️⃣ Merge (Added + Default - Removed)
            lstFinalRights = lstDefaultRights
                .Union(lstAddedRights)
                .Except(lstRemovedRights)
                .Distinct()
                .ToList();

            return lstFinalRights;

        }

        #endregion
    }
}
