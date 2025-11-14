using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class ServicingProviderSearchResponse
    {
        public IEnumerable<ServicingProvider> ServicingProviders { get; set; }
        public int? Total { get; set; }
    }
}
