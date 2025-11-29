using HMSMAL.POCO;

namespace HMSDAL.Interfaces.User
{
    /// <summary>
    /// Repository interface for user rights management
    /// </summary>
    public interface IUserRightsRepository
    {
        public List<Menus> GetUserMenus(int roleId, int departmentId);
    }
}
