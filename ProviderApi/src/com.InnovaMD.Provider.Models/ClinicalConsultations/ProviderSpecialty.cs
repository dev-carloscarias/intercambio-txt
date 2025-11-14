namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ProviderSpecialty
    {
        public int? SpecialtyId { get; set; }
        public string SpecialtyIdProtected { get; set; }
        public string Name { get; set; }
        public bool? IsPrimarySpecialty { get; set; }
        public bool? AllowAnyContractedSpecialist { get; set; }
        public int? ProviderAffiliationId { get; set; }
    }
}
