using HMSDAL.Common;
using HMSDAL.Interfaces.Auth;
using HMSMAL.Auth;
using HMSMAL.POCO;
using ServiceStack.OrmLite;

namespace HMSDAL.Repositories.Auth
{
    /// <summary>
    /// DB Context class for login
    /// </summary>
    public class DBLoginContext : ILoginRepository
    {
        /// <summary>
        /// Retrieves the list of available user roles
        /// </summary>
        /// <returns></returns>
        public List<Roles> GetRoles()
        {
            List<Roles> result = new();

            string query = string.Format(@"SELECT RoleId,
                                                  RoleName 
                                           FROM 
                                                  Roles
                                           WHERE
                                                  FIND_IN_SET({0},ParentRoleId)"
                                           , LoggedInUserDetails.RoleId);

            using (var db = new MySqlOrmLite().Open())
            {
                result = db.Select<Roles>(query);
            }

            return result;
        }
    }
}
