
namespace com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria
{
    public class BeneficiaryClinicalConsultationSearchCriteria : SearchCriteriaBase
    {
        public int BeneficiaryId { get; set; }
        public string ClinicalConsultationNumber { get; set; }
        public string ProviderName { get; set; }
    }
}
