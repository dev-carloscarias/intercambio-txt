using System;
using System.Collections.Generic;
using System.Text;

namespace com.InnovaMD.Provider.Models.Security
{
    public class ApplicationDomainSubContext
    {
        public int ApplicationDomainSubContextId { get; set; }
        public string Name { get; set; }
        public bool IsIndividual { get; set; }
        public bool IsOrganization { get; set; }
        public ApplicationDomainContext DomainContext { get; set; }
    }
}
