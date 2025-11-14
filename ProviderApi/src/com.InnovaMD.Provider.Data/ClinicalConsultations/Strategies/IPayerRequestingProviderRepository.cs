using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations.Strategies
{
    public interface IPayerRequestingProviderRepository : IDisposable
    {
        (IEnumerable<RequestingProvider> providers, int count) SearchRequestingProviders(RequestingProviderSearchCriteria searchCriteria, int lineOfBusinessId, int healthPlanId);
    }
}
