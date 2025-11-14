using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.Models.Security;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Business.Strategies.RequestingProvidier
{
    public interface ISearchRequestingProviderStrategy
    {
        string Name { get; }

        RequestingProviderSearchResponse SearchRequestingProvider(int beneficiaryId, IdentityUser user, RequestingProviderSearchCriteria searchCriteria);
    }
}
