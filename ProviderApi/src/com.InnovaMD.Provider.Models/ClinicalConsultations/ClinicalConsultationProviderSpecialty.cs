namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ClinicalConsultationProviderSpecialty
    {
        public long? ClinicalConsultationProviderSpecialtyId { get; set; }
        public long? ClinicalConsultationProviderId { get; set; }
        public int SpecialtyId { get; set; }
        public string Name { get; set; }
        public int? ProviderEntityTypeId { get; set; }
    }
}
