using com.InnovaMD.Provider.Models.ClinicalConsultations.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class ServicingProviderFiltersResponse
    {
        public IEnumerable<Filters.Specialty> Specialty { get; set; }
        public IEnumerable<State> State { get; set; }
        public IEnumerable<County> County { get; set; }
        public IEnumerable<ZipCode> ZipCode { get; set; }
        public IEnumerable<AdministrationGroup> AdministrationGroup { get; set; }

    }
}
