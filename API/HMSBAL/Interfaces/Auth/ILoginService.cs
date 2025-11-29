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
        public Response Invite(DTOInvitation objInv);
        public Response Register(Registration objRegistration);
        public Response Login(Login objLogin);
        public Response GetRoles();
        public Response GetDepartments();
        public Response Logout(IHeaderDictionary requestHeaders);
    }
}
