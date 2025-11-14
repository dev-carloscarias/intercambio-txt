using com.InnovaMD.Provider.Models.ClinicalConsultations;
using System;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations
{
    public interface IBeneficiaryRepository : IDisposable
    {
        public BeneficiaryInformation GetBeneficiaryInformation(int beneficiaryId);
    }
}
