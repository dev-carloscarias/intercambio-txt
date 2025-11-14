using com.InnovaMD.Provider.Models.ClinicalConsultations.Filters;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ClinicalConsultationProvider
    {
        public long? ClinicalConsultationProviderId { get; set; }
        public long? ClinicalConsultationId { get; set; }
        public int? ProviderAffiliationId { get; set; }
        public int? RenderingProviderId { get; set; }
        public int? BillingProviderId { get; set; }

        public string RenderingNPI { get; set; }
        public string BillingNPI { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string BillingProviderName { get; set; }
        public int? AddressId { get; set; }
        public string FullAddress =>  $"{AddressLine1} {AddressLine2 ?? string.Empty} {CountyName}, {StateName} {ZipCode}"; 
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string CountyName { get; set; }
        public string StateName { get; set; }
        public string ZipCode { get; set; }
        public string LocationCodeAddress { get; set; }
        public string LocationAddress { get; set; }
        public int? PhoneId { get; set; }
        public string PhoneNumber { get; set; }
        public int? FaxId { get; set; }
        public string FaxNumber { get; set; }
        public string Email { get; set; }
        public bool? IsPPN { get; set; }
        public bool? IsBestPractice { get; set; }
        public bool? IsPriority { get; set; }
        public bool? IsCapitated { get; set; }

        public string Specialty { get; set; }

        public int? ClinicalConsultationProviderClassificationId { get; set; }
        public ClinicalConsultationProviderClassification Classification { get; set; }
        public IEnumerable<ClinicalConsultationProviderSpecialty> Specialties { get; set; }

        public int? ClinicalConsultationProviderTypeId { get; set; }
        public string ProviderType { get; set; }

        public int? AdministrationGroupId { get; set; }
        public string AdministrationGroupName { get; set; }

        public int? Priority { get; set; }
        public IEnumerable<AdministrationGroup> AdministrationGroups { get; set; }
    }
}
