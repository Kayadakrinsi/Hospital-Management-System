using HMSBAL.Interfaces.User;
using HMSMAL.Common;
using HMSMAL.DTO.User;
using Microsoft.AspNetCore.Mvc;

namespace HMSAPI.Controllers.User
{
    /// <summary>
    /// Controller for handling user rights and menu related operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CLUserRightsController : ControllerBase
    {
        #region Private Members

        /// <summary>
        /// Stores instance of IUserRightsService interface
        /// </summary>
        IUserRightsService _userRightsService { get; set; }

        /// <summary>
        /// Stores instance of Response class
        /// </summary>
        Response objResponse;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes necessary services/objects
        /// </summary>
        /// <param name="userRightsService">Instance of IUserRightsService</param>
        public CLUserRightsController(IUserRightsService userRightsService)
        {
            _userRightsService = userRightsService;
            objResponse = new();
        }

        #endregion

        /// <summary>
        /// Retrieves the list of menus available to the current user
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUserMenus")]
        public IActionResult GetUserMenus()
        {
            objResponse = _userRightsService.GetUserMenus();
            return Ok(objResponse);
        }

        /// <summary>
        /// Retrieves a list of users available to the current user to manage rights
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            objResponse = _userRightsService.GetUsers();
            return Ok(objResponse);
        }

        /// <summary>
        /// Retrieves the list of activities available to a specified user based on their role.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="roleId">Role Id</param>
        /// <returns></returns>
        [HttpGet("GetUserActivities")]
        public IActionResult GetUserActivities(int userId, int roleId)
        {
            objResponse = _userRightsService.GetUserActivities(userId, roleId);
            return Ok(objResponse);
        }

        /// <summary>
        /// Saves the user rights for a specified user
        /// </summary>
        /// <param name="objDTOUserRights">Instace of DTOUserRights class</param>
        /// <returns></returns>
        [HttpPost("SaveUserRights")]
        public IActionResult SaveUserRights(DTOUserRights objDTOUserRights)
        {
            objResponse = _userRightsService.SaveUserRights(objDTOUserRights);
            return Ok(objResponse);
        }
    }
}
