using HMSMAL.POCO;

namespace HMSDAL.Interfaces.User
{
    /// <summary>
    /// Repository interface for user rights management
    /// </summary>
    public interface IUserRightsRepository
    {
        /// <summary>
        /// Retrieves the list of menus available to the current user
        /// </summary>
        /// <returns></returns>
        public List<Menus> GetUserMenus(int roleId, int departmentId);
    }
}
