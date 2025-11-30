using HMSDAL.Common;
using HMSDAL.Interfaces.User;
using HMSMAL.POCO;
using ServiceStack.OrmLite;

namespace HMSDAL.Repositories.User
{
    /// <summary>
    /// DB Context class for User Rights 
    /// </summary>
    public class DBUserRightsContext : IUserRightsRepository
    {
        /// <summary>
        /// Retrieves the list of menus available to the current user
        /// </summary>
        /// <returns></returns>
        public List<Menus> GetUserMenus(int roleId, int departmentId)
        {
            List<Menus> result;

            string where = string.Empty,
                   query = string.Empty;

            if (roleId == 2)
            {
                where += string.Format(@" FIND_IN_SET({0}, DepartmentIdsCsv) AND ", departmentId);
            }

            query = string.Format(@"SELECT MenuId,
                                                  MenuName,
                                                  MenuUrl,
                                                  ParentId,
                                                  DisplayOrder,
                                                  ViewActivityId,
                                                  IconName
                                           FROM 
                                                  Menus 
                                           WHERE 
                                                 IsActive = 1 AND  
                                                 {0}
                                                 FIND_IN_SET({1}, RoleIdsCsv)
                                           ORDER BY ParentId, MenuName, DisplayOrder"
                                           ,where, roleId);

            using (var db = new MySqlOrmLite().Open())
            {
                result = db.Select<Menus>(query);
            }

            return result;
        }
    }
}
