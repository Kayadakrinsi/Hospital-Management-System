using ServiceStack.DataAnnotations;

namespace HMSMAL.POCO
{
    /// <summary>
    /// POCO Model Of Department
    /// </summary>
    public class Departments
    {
        /// <summary>
        /// Department Id
        /// </summary>
        [PrimaryKey,AutoIncrement]
        public int DepartmentId { get; set; }

        /// <summary>
        /// Department Name
        /// </summary>
        public string DepartmentName { get; set; } = string.Empty;
    }
}
