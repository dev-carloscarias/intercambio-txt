using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class BeneficiaryInformation
    {
        public int BeneficiaryId { get; set; }
        public string BeneficiaryIdProtected { get; set; }
        public int? BeneficiaryIdentifierId { get; set; }
        public string Identifier { get; set; }
        public string CardNumber { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public int? ProductId { get; set; }
        public int? LineOfBusinessId { get; set; }
        public string LineOfBusinessIdProteted { get; set; }
        public int? HealthPlanId { get; set; }
        public int? PCPAffiliationId { get; set; }
        public int? PCPRenderingProviderId { get; set; }
        public int? PCPBillingProviderId { get; set; }
        public int? CityId { get; set; }
        public string ZipCode { get; set; }

        public IEnumerable<int> Networks { get; set; }
    }
}
