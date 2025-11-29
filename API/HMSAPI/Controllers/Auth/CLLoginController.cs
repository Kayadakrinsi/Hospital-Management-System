using HMSBAL.Interfaces.Auth;
using HMSMAL.Auth;
using HMSMAL.Common;
using HMSMAL.DTO.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HMSAPI.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class CLLoginController : ControllerBase
    {
        ILoginService _loginService { get; set; }
        Response objResponse;

        public CLLoginController(ILoginService loginService)
        {
            _loginService = loginService;
            objResponse = new();
        }

        [HttpGet("GetRoles")]
        public IActionResult GetRoles()
        {
            objResponse = _loginService.GetRoles();
            return Ok(objResponse);
        }

        [HttpGet("GetDepartments")]
        public IActionResult GetDepartments()
        {
            objResponse = _loginService.GetDepartments();
            return Ok(objResponse);
        }

        [HttpPost("Invite")]
        public IActionResult Invite([FromBody] DTOInvitation objInv)
        {
            objResponse = _loginService.Invite(objInv);
            return Ok(objResponse);
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            objResponse = _loginService.Logout(Request.Headers);
            return Ok(objResponse);
        }

        [AllowAnonymous, HttpPost("Register")]
        public IActionResult Register([FromBody] Registration objRegistration)
        {
            objResponse = _loginService.Register(objRegistration);
            return Ok(objResponse);
        }

        [AllowAnonymous, HttpPost("Login")]
        public IActionResult Login([FromBody] Login objLogin)
        {
            objResponse = _loginService.Login(objLogin);
            return Ok(objResponse);
        }

    }
}
