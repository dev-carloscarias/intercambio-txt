using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria
{
    public class RequestingProviderSearchCriteria: SearchCriteriaBase
    {
        public string NPI { get; set; }
        public string ProviderName { get; set; }

    }
}
