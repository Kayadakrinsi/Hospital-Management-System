namespace HMSMAL.Auth
{
    public static class LoggedInUserDetails
    {
        /// <summary>
        /// User Id
        /// </summary>
        public static int UserId { get; set; } = 0;
        
        /// <summary>
        /// User Name
        /// </summary>
        public static string UserName { get; set; } = string.Empty;

        /// <summary>
        /// EMail ID of user
        /// </summary>
        public static string Email { get; set; } = string.Empty;

        /// <summary>
        /// Role Id of User
        /// </summary>
        public static int RoleId { get; set; } = 0;

        /// <summary>
        /// Department Id of User
        /// </summary>
        public static int DepartmentId { get; set; } = 0;

        /// <summary>
        /// Role of User
        /// </summary>
        public static string Role { get; set; } = string.Empty;

        /// <summary>
        /// Department of User
        /// </summary>
        public static string Department { get; set; } = string.Empty;
    }
}
