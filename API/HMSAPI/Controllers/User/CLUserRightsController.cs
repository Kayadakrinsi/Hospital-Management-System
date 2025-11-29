using HMSBAL.Interfaces.User;
using HMSMAL.Common;
using HMSMAL.DTO.User;
using Microsoft.AspNetCore.Mvc;

namespace HMSAPI.Controllers.User
{
    [Route("api/[controller]")]
    [ApiController]
    public class CLUserRightsController : ControllerBase
    {
        IUserRightsService _userRightsService { get; set; }
        Response objResponse;

        public CLUserRightsController(IUserRightsService userRightsService)
        {
            _userRightsService = userRightsService;
            objResponse = new();
        }

        [HttpGet("GetUserMenus")]
        public IActionResult GetUserMenus()
        {
            objResponse = _userRightsService.GetUserMenus();
            return Ok(objResponse);
        }

        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            objResponse = _userRightsService.GetUsers();
            return Ok(objResponse);
        }

        [HttpGet("GetUserActivities")]
        public IActionResult GetUserActivities(int userId, int roleId)
        {
            objResponse = _userRightsService.GetUserActivities(userId, roleId);
            return Ok(objResponse);
        }

        [HttpPost("SaveUserRights")]
        public IActionResult SaveUserRights(DTOUserRights objDTOUserRights)
        {
            objResponse = _userRightsService.SaveUserRights(objDTOUserRights);
            return Ok(objResponse);
        }
    }
}
