namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class RequestingProviderSearchRequest
    {
        public string BeneficiaryId { get; set; }

        public string Search { get; set; }

        public int Page { get; set; }
    }
}
