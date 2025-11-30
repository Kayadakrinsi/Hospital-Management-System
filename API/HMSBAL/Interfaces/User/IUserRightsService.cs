using HMSMAL.Common;
using HMSMAL.DTO.User;

namespace HMSBAL.Interfaces.User
{
    /// <summary>
    /// Service interface for user rights management
    /// </summary>
    public interface IUserRightsService
    {
        /// <summary>
        /// Retrieves the list of menus available to the current user
        /// </summary>
        /// <returns></returns>
        public Response GetUserMenus();

        /// <summary>
        /// Retrieves a list of users available to the current user to manage rights
        /// </summary>
        /// <returns></returns>
        public Response GetUsers();

        /// <summary>
        /// Retrieves the list of activities available to a specified user based on their role.
        /// </summary>
        /// <param name="userId">User Id</param>
        /// <param name="roleId">Role Id</param>
        /// <returns></returns>
        public Response GetUserActivities(int userId, int roleId);

        /// <summary>
        /// Saves the user rights for a specified user
        /// </summary>
        /// <param name="objDTOUserRights">Instace of DTOUserRights class</param>
        /// <returns></returns>
        public Response SaveUserRights(DTOUserRights objDTOUserRights);
    }
}
