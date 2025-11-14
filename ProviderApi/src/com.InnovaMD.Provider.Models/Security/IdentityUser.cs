using com.InnovaMD.Utilities.Provider.Security.Models;
using System;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.Security
{
    /**
     * Representation of a user based on the claims provided in the access token
     * 
     * The properties in this model may vary depending on the necesity of the project.
     */
     
    public class IdentityUser
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public bool IsDelegate { get; set; }
        public int ApplicationDomainId { get; set; }
        public IdentityRole ActiveRole { get; set; }
        public int? ProviderId { get; set; }
        public IEnumerable<int> ProviderAffiliations { get; set; }
        public IdentityAdministrationGroup AdministrationGroup { get; set; }
        public IdentityHealthPlan HealthPlan { get; set; }
        public DateTime Expiration { get; set; }
        public string SessionId { get; set; }
    }
}
