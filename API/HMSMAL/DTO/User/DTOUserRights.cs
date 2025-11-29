using System;
using System.Collections.Generic;
using System.Text;

namespace HMSMAL.DTO.User
{
    /// <summary>
    /// DTO Model To Save User Rights
    /// </summary>
    public class DTOUserRights
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public string AddRights { get; set; } = string.Empty;
        public string RemoveRights { get; set; } = string.Empty;
    }

}
