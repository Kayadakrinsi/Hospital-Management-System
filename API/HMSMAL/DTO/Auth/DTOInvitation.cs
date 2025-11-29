using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace HMSMAL.DTO.Auth
{
    /// <summary>
    /// DTO Model for invitation process
    /// </summary>
    public class DTOInvitation
    {

        /// <summary>
        /// Contact number of the invitee
        /// </summary>
        [Required(ErrorMessage = "Contact number is required")]
        public long Contact { get; set; }

        /// <summary>
        /// Email of the invitee
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Role Id assigned to the invitee
        /// </summary>
        [Required(ErrorMessage = "Role is required")]
        public int RoleId { get; set; }

        /// <summary>
        /// Department Id assigned to the invitee
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int DepartmentId { get; set; }

    }
}
