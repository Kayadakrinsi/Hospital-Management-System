using HMSMAL.Common;
using HMSMAL.DTO.User;

namespace HMSBAL.Interfaces.User
{
    /// <summary>
    /// Service interface for user rights management
    /// </summary>
    public interface IUserRightsService
    {
        public Response GetUserMenus();
        public Response GetUsers();
        public Response GetUserActivities(int userId, int roleId);
        public Response SaveUserRights(DTOUserRights objDTOUserRights);
    }
}
