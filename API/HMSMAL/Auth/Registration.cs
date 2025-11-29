namespace HMSMAL.Auth
{
    /// <summary>
    /// Registration Model
    /// </summary>
    public class Registration
    {
        /// <summary>
        /// Invitation Code
        /// </summary>
        public int InvCode { get; set; }

        /// <summary>
        /// User Name
        /// </summary>
        public string Username { get; set; } = string.Empty;

        /// <summary>
        /// Password of User
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

}
