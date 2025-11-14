using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Filters
{
    public class State
    {
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string? FIPSCode { get; set; }
        public string? USPSCode { get; set; }
        public string Name { get; set; } = string.Empty;
        public int? GNISId { get; set; }
        public string? StateIdProtected { get; set; }
    }
}
