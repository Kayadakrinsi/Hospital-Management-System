using ServiceStack.DataAnnotations;

namespace HMSMAL.POCO
{
    /// <summary>
    /// POCO Model Of Activity
    /// </summary>
    public class Activities
    {
        /// <summary>
        /// Activity ID
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int ActivityId { get; set; }

        /// <summary>
        /// Activity Key
        /// </summary>
        public string ActivityKey { get; set; } = string.Empty;

        /// <summary>
        /// Activity Name
        /// </summary>
        public string ActivityName { get; set; } = string.Empty;

        /// <summary>
        /// Parent Activity ID
        /// </summary>
        public int ParentActivityId { get; set; }

        /// <summary>
        /// 1 - Active, 0 - Inactive (Default : 1)
        /// </summary>
        public int IsActive { get; set; } = 1;

        /// <summary>
        /// 1 - If has child activities, 0 - If no child activities (Default : 0)
        /// </summary>
        public int HasChild { get; set; } = 0;

        /// <summary>
        /// Display Order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Department Ids CSV associated with this menu
        /// </summary>
        public string DepartmentIdsCsv { get; set; } = string.Empty;

        /// <summary>
        /// Role Ids CSV associated with this menu
        /// </summary>
        public string RoleIdsCsv { get; set; } = string.Empty;
    }
}
