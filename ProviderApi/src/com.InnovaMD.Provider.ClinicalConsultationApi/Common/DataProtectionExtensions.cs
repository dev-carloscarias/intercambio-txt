using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.PortalApi.Common
{
    public static class DataProtectionExtensions
    {
        public static int UnprotectInt(this IDataProtector protector, string value)
        {
            return int.Parse(protector.Unprotect(value));
        }
    }
}
