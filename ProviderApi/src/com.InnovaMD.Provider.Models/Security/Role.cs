using com.InnovaMD.Provider.Models.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace com.InnovaMD.Provider.Models.Security
{
    public class Role : AuditableEntity
    {
        public virtual int? RoleId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual bool? IsSystemRole { get; set; }
        public virtual bool? IsPrimary { get; set; }
        public virtual RoleStatus Status { get; set; }
        public virtual IEnumerable<Privilege> Privileges { get; set; }
        public virtual bool? IsAssigned { get; set; }
        public virtual IEnumerable<Role> ParentRoles { get; set; }
        public virtual DateTime LastStatusDateTime { get; set; }
        public virtual ApplicationDomainContext Context { get; set; }
        public virtual ApplicationDomainSubContext SubContext { get; set; }
    }
}
