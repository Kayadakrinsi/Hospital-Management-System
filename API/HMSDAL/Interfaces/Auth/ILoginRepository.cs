using HMSMAL.POCO;

namespace HMSDAL.Interfaces.Auth
{
    /// <summary>
    /// Repository interface for login related operations
    /// </summary>
    public interface ILoginRepository
    {
        /// <summary>
        /// Retrieves the list of available user roles
        /// </summary>
        /// <returns></returns>
        public List<Roles> GetRoles();
    }
}
