namespace HMSMAL.Common
{
    /// <summary>
    /// Enum For Genders
    /// </summary>
    public enum EnmGenders
    {
        /// <summary>
        /// 0 - Other
        /// </summary>
        O,

        /// <summary>
        /// 1 - Male
        /// </summary>
        M,

        /// <summary>
        /// 2 - Female
        /// </summary>
        F
    }

    /// <summary>
    /// Enum For Error Codes
    /// </summary>
    public enum EnmErrorCodes
    {
        #region Invitation Errors

        /// <summary>
        /// User already exists for this contact or email, Please enter different details
        /// </summary>
        INV01,

        /// <summary>
        /// Failed to send email
        /// </summary>
        INV02,

        #endregion

        #region Registration Errors

        /// <summary>
        /// Invalid Invitation Code
        /// </summary>
        REG01,

        /// <summary>
        /// Registration Failed, Please Try Again
        /// </summary>
        REG02,

        #endregion

        #region Login Errors

        /// <summary>
        /// Invalid Login Credentials
        /// </summary>
        LOG01,

        /// <summary>
        /// Invalid Login ID
        /// </summary>
        LOG02,

        /// <summary>
        /// Invalid Password
        /// </summary>
        LOG03,

        /// <summary>
        /// Token Not Found
        /// </summary>
        LOG04,

        #endregion

        /// <summary>
        /// Failed to add record
        /// </summary>
        E0001,

        /// <summary>
        /// Failed to update record
        /// </summary>
        E0002,

        /// <summary>
        /// No records found
        /// </summary>
        E0003,

        /// <summary>
        /// Slot unavailable!
        /// </summary>
        E0004,

        /// <summary>
        /// Invalid appointment!
        /// </summary>
        A0001,
    }
}
