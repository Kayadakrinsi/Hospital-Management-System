namespace HMSMAL.DTO.Auth
{
    /// <summary>
    /// Authentication response class
    /// </summary>
    public class AuthResponse
    {
        /// <summary>
        /// Auth Token
        /// </summary>
        public string Token { get; set; } = string.Empty;

        /// <summary>
        /// User Name
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// User Role
        /// </summary>
        public string Role { get; set; } = string.Empty;
    }
}
