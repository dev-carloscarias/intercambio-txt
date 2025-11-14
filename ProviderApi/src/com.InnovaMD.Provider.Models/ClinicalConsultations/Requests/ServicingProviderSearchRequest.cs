namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class ServicingProviderSearchRequest
    {
        public string BeneficiaryId { get; set; }

        public string Search { get; set; }

        public string Specialty { get; set; }

        public string AdministrationGroup { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public int Page { get; set; }
    }
}
