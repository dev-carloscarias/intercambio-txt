using System;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class SuggestionsResponse
    {
        public long ClinicalConsultationId { get; set; }
        public string ClinicalConsultationIdProtected { get; set; }
        public string ClinicalConsultationNumber { get; set; }
        public DateTime ClinicalConsultationDate { get; set; }
        public bool AnyContractedSpecialist { get; set; }
        public string Diagnosis { get; set; }
        public string Specialty { get; set; }
        public string Purpose { get; set; }        
        public string ProviderName { get; set; }        
    }
}