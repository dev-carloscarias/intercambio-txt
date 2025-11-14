using System;
using System.Collections.Generic;
using System.Text;

namespace com.InnovaMD.Provider.Models.Common
{
    public abstract class AuditableEntity
    {
        public virtual int CreatedUserId { get; set; }

        public virtual DateTime CreatedDate { get; set; }

        public virtual int? ModifiedUserId { get; set; }

        public virtual DateTime? ModifiedDate { get; set; }
    }
}
