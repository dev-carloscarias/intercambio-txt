using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class RequestingProviderSearchResponse
    {
        public IEnumerable<RequestingProvider> RequestingProviders { get; set; }
        public int Total { get; set; }
    }
}
