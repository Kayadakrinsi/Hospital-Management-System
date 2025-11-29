using ServiceStack.DataAnnotations;

namespace HMSMAL.POCO
{
    /// <summary>
    /// POCO Model Of Roles
    /// </summary>
    public class Roles
    {
        /// <summary>
        /// Role Id
        /// </summary>
        [PrimaryKey,AutoIncrement]
        public int RoleId { get; set; }

        /// <summary>
        /// Role Name
        /// </summary>
        public string RoleName { get; set; } = string.Empty;

        /// <summary>
        /// Default Rights in CSV format
        /// </summary>
        public string DefaultRightsCsv { get; set; } = string.Empty;

        /// <summary>
        /// Parent Role Ids in CSV format
        /// </summary>
        public string ParentRoleId { get; set; } = string.Empty;
    }
}
