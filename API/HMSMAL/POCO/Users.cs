using HMSMAL.Common;
using ServiceStack.DataAnnotations;

namespace HMSMAL.POCO
{
    /// <summary>
    /// POCO Model Of User
    /// </summary>
    public class Users
    {
        /// <summary>
        /// User Id
        /// </summary>
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// EMail ID of user
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password of User
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Role of User
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Department of User
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Gender of User (Default : 0 - Other)
        /// </summary>
        public int Gender { get; set; } = (int)EnmGenders.O;

        /// <summary>
        /// Date Of Birth of User
        /// </summary>
        public DateOnly DOB { get; set; }

        /// <summary>
        /// Contact Number Of User
        /// </summary>
        public long Contact { get; set; }

        /// <summary>
        /// Address Of User
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// 1 - Active, 0 - Inactive (Default : 1)
        /// </summary>
        public int IsActive { get; set; } = 1;

        /// <summary>
        /// Created By User Id (Parent User ID)
        /// </summary>
        [IgnoreOnUpdate]
        public int CreatedBy { get; set; }

        /// <summary>
        /// User Creation DateTime
        /// </summary>
        [IgnoreOnUpdate]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        /// <summary>
        /// User Modification DateTime
        /// </summary>
        [IgnoreOnInsert]
        public DateTime ModifiedOn { get; set; } = DateTime.Now;
    }
}
