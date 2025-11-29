using ServiceStack.DataAnnotations;

namespace HMSMAL.Auth
{
    /// <summary>
    /// Invitation Model
    /// </summary>
    public class Invitations
    {
        /// <summary>
        /// Invitation Id
        /// </summary>
        [PrimaryKey,AutoIncrement]
        public int InvId { get; set; }

        /// <summary>
        /// Invitation Code
        /// </summary>
        public int InvCode { get; set; }

        /// <summary>
        /// Contact number of the invitee
        /// </summary>
        public long Contact { get; set; }

        /// <summary>
        /// Email of the invitee
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Role Id assigned to the invitee
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Department Id assigned to the invitee
        /// </summary>
        public int DepartmentId { get; set; }

        /// <summary>
        /// Created By User Id
        /// </summary>
        public int CreatedBy { get; set; }

        /// <summary>
        /// Creation DateTime
        /// </summary>
        [IgnoreOnUpdate]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
