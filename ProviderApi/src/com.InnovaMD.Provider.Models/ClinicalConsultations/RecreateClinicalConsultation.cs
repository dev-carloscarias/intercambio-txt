using com.InnovaMD.Provider.Models.ClinicalConsultations.Filters;
using System;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class RecreateClinicalConsultation
    {
        public long ClinicalConsultationId { get; set; }
        public string ClinicalConsultationIdProtected { get; set; }
        public string ClinicalConsultationNumber { get; set; }
        public DateTime ClinicalConsultationDate { get; set; }
        public string Purpose { get; set; }
        public bool IsConsultation { get; set; }
        public bool AnyContractedSpecialist { get; set; }
        public ProviderSpecialty ServicingSpecialty { get; set; }
        public City ServicingCity { get; set; }
        public City RequestingCity { get; set; }
        public ServicingNonPPNReason ServicingNonPPNReason { get; set; }
        public AdditionalHealthPlan AdditionalHealthPlan { get; set; }
        public IEnumerable<ClinicalConsultationDiagnosis> Diagnoses { get; set; }
        public ClinicalConsultationProcedureBundle Procedure { get; set; }
        public RequestingProvider RequestingProvider { get; set; }
        public ServicingProvider ServicingProvider { get; set; }
    }

}