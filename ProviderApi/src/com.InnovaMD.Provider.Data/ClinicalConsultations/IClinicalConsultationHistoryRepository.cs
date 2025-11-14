using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Requests;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using System;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations
{
    public interface IClinicalConsultationHistoryRepository : IDisposable
    {
        (IEnumerable<ClinicalConsultation>, int) GetBeneficiaryClinicalConsultations(BeneficiaryClinicalConsultationSearchCriteria searchCriteria, int userId);
        ClinicalConsultation GetClinicalConsultationDetail(int clinicalConsultationId, int userId);
        public int GetLineOfBusinessFromClinicalConsultationId(int clinicalConsultationId);
    }
}
