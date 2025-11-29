namespace HMSMALPOCO
{
    /// <summary>
    /// POCO Model Of User Rights
    /// </summary>
    public class UserRights
    {
        /// <summary>
        /// Primary key for the UserRights
        /// </summary>
        public int UserRightId { get; set; }

        /// <summary>
        /// ID of the user
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// ID of the role assigned to the user
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Comma-separated list of Right IDs that the user has
        /// </summary>
        public string? RightsIdsCsv { get; set; }

        /// <summary>
        /// Comma-separated list of Right IDs that were removed for this user
        /// </summary>
        public string? RemovedRightsIdsCsv { get; set; }

        /// <summary>
        /// Date and time when the record was created
        /// </summary>
        public DateTime CreatedOn { get; set; }

        /// <summary>
        /// Date and time when the record was last modified
        /// </summary>
        public DateTime ModifiedOn { get; set; }
    }
}
