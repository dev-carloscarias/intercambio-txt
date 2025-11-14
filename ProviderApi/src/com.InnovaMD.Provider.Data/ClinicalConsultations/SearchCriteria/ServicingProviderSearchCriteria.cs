using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria
{
    public class ServicingProviderSearchCriteria : SearchCriteriaBase
    {
        public string NPI { get; set; }
        public string ProviderName { get; set; }
        public int BeneficiaryId { get; set; }
        public int LineOfBusinessId { get; set; }
        public int? CityId { get; set; }
        public int? StateId { get; set; }
        public int? AdministrationGroupId { get; set; }
        public int? SpecialtyId { get;set; }
        public string ZipCode { get; set; }

        public int? BeneficiaryCityId { get; set; }
    }
}
