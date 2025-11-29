using ServiceStack.DataAnnotations;

namespace HMSMAL.POCO
{
    /// <summary>
    /// POCO Model Of Menu
    /// </summary>
    public class Menus
    {
        /// <summary>
        /// Menu ID
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int MenuId { get; set; }

        /// <summary>
        /// Menu Name
        /// </summary>
        public string MenuName { get; set; } = string.Empty;

        /// <summary>
        /// Menu url
        /// </summary>
        public string MenuUrl { get; set; } = string.Empty;

        /// <summary>
        /// Parent Menu Id (0 if root/parent menu)
        /// </summary>
        public int ParentId { get; set; } = 0;

        /// <summary>
        /// Display Order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// 1 - Active, 0 - Inactive (Default : 1)
        /// </summary>
        public int IsActive { get; set; } = 1;

        /// <summary>
        /// View Activity Id associated with this menu
        /// </summary>
        public int ViewActivityId { get; set; }

        /// <summary>
        /// Department Ids CSV associated with this menu
        /// </summary>
        public string DepartmentIdsCsv { get; set; } = string.Empty;

        /// <summary>
        /// Role Ids CSV associated with this menu
        /// </summary>
        public string RoleIdsCsv { get; set; } = string.Empty;

        /// <summary>
        /// Icon Name for the menu
        /// </summary>
        public string IconName { get; set; } = string.Empty;
    }
}
