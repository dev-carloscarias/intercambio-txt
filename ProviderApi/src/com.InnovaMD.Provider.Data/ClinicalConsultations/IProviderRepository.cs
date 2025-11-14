using com.InnovaMD.Provider.Models.ClinicalConsultations;
using System;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations
{
    public interface IProviderRepository : IDisposable
    {
        int FindProvider(IEnumerable<int> providerAffiliationIds);
    }
}
