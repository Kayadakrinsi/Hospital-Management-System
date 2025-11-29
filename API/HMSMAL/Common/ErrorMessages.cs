namespace HMSMAL.Common
{
    /// <summary>
    /// Error Messages Class
    /// </summary>
    public static class ErrorMessages
    {
        public static readonly Dictionary<EnmErrorCodes, string> Messages = new()
        {
            { EnmErrorCodes.INV01, "User already exists for this contact or email, Please enter different details" },
            { EnmErrorCodes.INV02, "Failed to send email" },

            { EnmErrorCodes.REG01, "Invalid Invitation Code" },
            { EnmErrorCodes.REG02, "Registration Failed, Please Try Again" },

            { EnmErrorCodes.LOG01, "Invalid Login Credentials" },
            { EnmErrorCodes.LOG02, "Invalid Login ID" },
            { EnmErrorCodes.LOG03, "Invalid Password" },
            { EnmErrorCodes.LOG04, "Token Not Found" },
           
            { EnmErrorCodes.E0001, "Failed to add record" },
            { EnmErrorCodes.E0002, "Failed to update record" },
            { EnmErrorCodes.E0003, "No records found" },
            { EnmErrorCodes.E0004, "Slot unavailable" },

            { EnmErrorCodes.A0001, "Invalid appointment" },

        };
    }
}
