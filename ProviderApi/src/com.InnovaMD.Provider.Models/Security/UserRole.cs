using System;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.Security
{
    public class UserRole : Role
    {
        public virtual int? UserRoleId { get; set; }

        public virtual int UserId { get; set; }

        public virtual DateTime StartDate { get; set; }

        public virtual DateTime? EndDate { get; set; }

        public IEnumerable<UserRoleDataAccess> RoleDataAccesses { get; set; }
    }
}
