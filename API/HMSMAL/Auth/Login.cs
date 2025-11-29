using System.ComponentModel.DataAnnotations;

namespace HMSMAL.DTO.Auth
{
    /// <summary>
    /// Login Model
    /// </summary>
    public class Login
    {
        /// <summary>
        /// EMail ID Of User
        /// </summary>
        [Required(ErrorMessage = "EMail is required")]
        [RegularExpression(@"^[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$", ErrorMessage = "Invalid Email")]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Password Of User
        /// </summary>
        [Required(ErrorMessage = "Password is required")]
       // [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{6,50}$", ErrorMessage = "Password must be 6–50 characters long and contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
        public string Password { get; set; } = string.Empty;
    }
}
