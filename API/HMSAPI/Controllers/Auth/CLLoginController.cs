using HMSBAL.Interfaces.Auth;
using HMSMAL.Auth;
using HMSMAL.Common;
using HMSMAL.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSAPI.Controllers.Auth
{
    /// <summary>
    /// Controller for handling login related operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CLLoginController : ControllerBase
    {
        #region Private Members

        /// <summary>
        /// Stores instance of ILoginService interface
        /// </summary>
        ILoginService _loginService { get; set; }

        /// <summary>
        /// Stores instance of Response class
        /// </summary>
        Response objResponse;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes necessary services/objects
        /// </summary>
        /// <param name="loginService">Instance of ILoginService</param>
        public CLLoginController(ILoginService loginService)
        {
            _loginService = loginService;
            objResponse = new();
        }

        #endregion

        /// <summary>
        /// Retrieves the list of available user roles
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetRoles")]
        public IActionResult GetRoles()
        {
            objResponse = _loginService.GetRoles();
            return Ok(objResponse);
        }

        /// <summary>
        /// Retrieves the list of available departments
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetDepartments")]
        public IActionResult GetDepartments()
        {
            objResponse = _loginService.GetDepartments();
            return Ok(objResponse);
        }

        /// <summary>
        /// Invites a user by processing the provided invitation details
        /// </summary>
        /// <param name="objInv">Instance of DTOInvitation class</param>
        /// <returns></returns>
        [HttpPost("Invite")]
        public IActionResult Invite([FromBody] DTOInvitation objInv)
        {
            objResponse = _loginService.Invite(objInv);
            return Ok(objResponse);
        }

        /// <summary>
        /// Logs out the currently authenticated user and clears their session(clears cache session) or authentication token
        /// </summary>
        /// <returns></returns>
        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            objResponse = _loginService.Logout(Request.Headers);
            return Ok(objResponse);
        }

        /// <summary>
        /// Registers a new user based on the provided registration details
        /// </summary>
        /// <param name="objRegistration">Instance Registration of class</param>
        /// <returns></returns>
        [AllowAnonymous, HttpPost("Register")]
        public IActionResult Register([FromBody] Registration objRegistration)
        {
            objResponse = _loginService.Register(objRegistration);
            return Ok(objResponse);
        }

        /// <summary>
        /// Logs in a user by validating the provided login credentials, generates JWT Token, saves user details in cache
        /// </summary>
        /// <param name="objLogin"></param>
        /// <returns></returns>
        [AllowAnonymous, HttpPost("Login")]
        public IActionResult Login([FromBody] Login objLogin)
        {
            objResponse = _loginService.Login(objLogin);
            return Ok(objResponse);
        }

    }
}
