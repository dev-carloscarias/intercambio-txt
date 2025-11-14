using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Utilities.Provider.Security.Models;
using System;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ServicingProvider
    {
        public int? ProviderAffiliationId { get; set; }
        public int RenderingProviderId { get; set; }
        public string RenderingProviderNPI { get; set; }
        public string RenderingProviderName { get; set; }
        public int? BillingProviderId { get; set; }
        public string BillingProviderNPI { get; set; }
        public string BillingProviderName { get; set; }
        public string ProviderLocationName { get; set; }
        public string FacilityName { get; set; }        
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<City> Cities { get; set; }
        public IEnumerable<ProviderSpecialty> Specialties { get; set; }
        public string RenderingProviderIdProtected { get; set; }
        public string BillingProviderIdProtected { get; set; }
        public bool? IsMSOPPN { get; set; }
        public bool? IsBestPractice { get; set; }
        public bool? IsMSOClinic { get; set; }
        public bool? IsCapitated { get; set; }
        public int? Priority { get; set; }
        public bool? PreferredNetwork { get; set; }
    }
}
