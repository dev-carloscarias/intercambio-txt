using com.InnovaMD.Provider.Models.ClinicalConsultations.Filters;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ClinicalConsultationProviderSpecialty : Specialty
    {
        public ClinicalConsultationProviderSpecialty()
        {
        }

        public ClinicalConsultationProviderSpecialty(Specialty specialty)
        {
            if (specialty != null)
            {
                base.SpecialtyId = specialty.SpecialtyId;
                base.SpecialtyIdProtected = specialty.SpecialtyIdProtected;
                base.Name = specialty.Name;
                base.AllowAnyContractedSpecialist = specialty.AllowAnyContractedSpecialist;
                base.ProviderEntityTypeId = specialty.ProviderEntityTypeId;
            }
        }

        public long? ClinicalConsultationProviderSpecialtyId { get; set; }
        public long? ClinicalConsultationProviderId { get; set; }
    }
}
