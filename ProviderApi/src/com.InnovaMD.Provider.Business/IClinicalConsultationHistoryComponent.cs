using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Requests;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.Models.Security;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Business
{
    public interface IClinicalConsultationHistoryComponent : IDisposable
    {
        public BeneficiaryClinicalConsultationsResponse GetBeneficiaryClinicalConsultations(BeneficiaryClinicalConsultationsRequest request, IdentityUser user);
        public ClinicalConsultation GetClinicalConsultationDetail(int clinicalConsultationNumber, IdentityUser user);
        public Task<Models.Common.Report> GetClinicalConsultationForm(int clinicalConsultationId);
    }
}