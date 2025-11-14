using System;
using System.Collections.Generic;
using System.Text;

namespace com.InnovaMD.Provider.Models.Security
{
    public class Privilege
    {
        public virtual int RoleId { get; set; }
        public virtual bool IsGranted { get; set; }
        public virtual Feature Feature { get; set; }
    }
}
