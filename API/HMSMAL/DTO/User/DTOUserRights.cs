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
        /// <summary>
        /// User Id
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Role Id
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// Added rights csv
        /// </summary>
        public string AddRights { get; set; } = string.Empty;

        /// <summary>
        /// Removed rights csv
        /// </summary>
        public string RemoveRights { get; set; } = string.Empty;
    }

}
