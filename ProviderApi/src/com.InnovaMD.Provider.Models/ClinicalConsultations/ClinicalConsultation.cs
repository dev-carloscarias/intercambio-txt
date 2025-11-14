

using com.InnovaMD.Provider.Models.ClinicalConsultations.Filters;
using com.InnovaMD.Utilities.Dates;
using System;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ClinicalConsultation
    {
        public long ClinicalConsultationId { get; set; }
        public string ClinicalConsultationIdProtected { get; set; }
        public string ClinicalConsultationNumber { get; set; }
        public DateTime ClinicalConsultationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Purpose { get; set; }
        public AdditionalHealthPlan AdditionalHealthPlan { get; set; }
        public bool IsConsultation { get; set; }
        public bool AnyContractedSpecialist { get; set; } 
        public Specialty ServicingSpecialty { get; set; }
        public ClinicalConsultationServicingNonPPNReason ServicingNonPPNReason { get; set; }

        public string CreatedBy { get; set; }
        public int? CreatedUserId { get; set; }

        private DateTime? _createdDate;
        public DateTime? CreatedDate
        {
            get => _createdDate;
            set => _createdDate = value.FromUtcToSystemTimezone();
        }

        public int? ModifiedUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public string SourceIdentifier { get; set; }

        public ClinicalConsultationBeneficiary Beneficiary { get; set; }
        public IEnumerable<ClinicalConsultationDiagnosis> Diagnosis { get; set; }
        public ClinicalConsultationProvider Requesting { get; set; }
        public ClinicalConsultationProvider Servicing { get; set; }
        public ClinicalConsultationProvider PCP { get; set; }
        public IEnumerable<ClinicalConsultationProcedureBundle> Procedures { get; set; }

        private DateTime? _viewedOn;
        public DateTime? ViewedOn
        {
            get => _viewedOn;
            set => _viewedOn = value.FromUtcToSystemTimezone();
        }
        public bool IsRecreate { get; set; }
        public long? OriginalClinicalConsultationId { get; set; }
    }
}
