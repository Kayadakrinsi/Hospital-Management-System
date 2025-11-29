namespace HMSMAL.Common
{
    /// <summary>
    /// Response Model
    /// </summary>
    public class Response
    {
        /// <summary>
        /// True in case of error, False otherwise
        /// </summary>
        public bool IsError { get; set; } = false;

        /// <summary>
        /// Error Code
        /// </summary>
        public EnmErrorCodes ErrorCode
        {
            get;
            set
            {
                if (value != null)
                {
                    IsError = true;
                    Message = ErrorMessages.Messages.ContainsKey(value)
                        ? ErrorMessages.Messages[value]
                        : "Unknown error.";
                }
            }
        }

        /// <summary>
        /// Response Message
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Response Message
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// Data Model
        /// </summary>
        public object? Data { get; set; }

        /// <summary>
        /// User Rights Model
        /// </summary>
        public object? UserRights { get; set; }
    }
}
