using HMSMAL.POCO;
using System.Data;

namespace HMSDAL.Interfaces.Auth
{
    public interface ILoginRepository
    {
        public List<Roles> GetRoles();
    }
}
