using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.Models.Security;
using System;

namespace com.InnovaMD.Provider.Business
{
    public interface IBeneficiaryComponent: IDisposable
    {
        BeneficiaryInformation GetBeneficiaryBasicInformation(int beneficiaryId);
        public ClinicalConsultationCreateButton CheckCreateButtonPermissions(int beneficiaryId, IdentityUser user);
    }
}