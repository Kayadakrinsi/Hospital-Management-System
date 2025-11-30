using HMSMAL.Auth;
using HMSMAL.Common;
using HMSMAL.DTO.Auth;
using Microsoft.AspNetCore.Http;

namespace HMSBAL.Interfaces.Auth
{
    /// <summary>
    /// Service interface for login operations
    /// </summary>
    public interface ILoginService
    {
        /// <summary>
        /// Retrieves the list of available user roles
        /// </summary>
        /// <returns></returns>
        public Response GetRoles();

        /// <summary>
        /// Retrieves the list of available departments
        /// </summary>
        /// <returns></returns>
        public Response GetDepartments();

        /// <summary>
        /// Invites a user by processing the provided invitation details
        /// </summary>
        /// <param name="objInv">Instance of DTOInvitation class</param>
        /// <returns></returns>
        public Response Invite(DTOInvitation objInv);

        /// <summary>
        /// Logs out the currently authenticated user and clears their session(clears cache session) or authentication token
        /// </summary>
        /// <param name="requestHeaders">Current HTTP Request Headers</param>
        /// <returns></returns>
        public Response Logout(IHeaderDictionary requestHeaders);

        /// <summary>
        /// Registers a new user based on the provided registration details
        /// </summary>
        /// <param name="objRegistration">Instance Registration of class</param>
        /// <returns></returns>
        public Response Register(Registration objRegistration);

        /// <summary>
        /// Logs in a user by validating the provided login credentials, generates JWT Token, saves user details in cache
        /// </summary>
        /// <param name="objLogin"></param>
        /// <returns></returns>
        public Response Login(Login objLogin);
    }
}
