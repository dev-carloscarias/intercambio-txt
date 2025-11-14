namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class RequestingProviderOptionsResponse
    {
        public bool AllowSearch { get; set; }

        public RequestingProvider RequestingProvider { get; set; }

        public RequestingProvider BeneficiaryPcp { get; set; }

        public bool? NoPCP { get; set; }
        public string NoPCPMessage { get; set; }
    }
}
