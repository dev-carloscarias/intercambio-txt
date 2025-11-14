using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Filters
{
    public class AdministrationGroup
    {
        public int AdministrationGroupId { get; set; }
        public int AdministrationGroupTypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NPI { get; set; }
        public string? Number { get; set; }
        public string? Code { get; set; }
        public bool? IsActive { get; set; }
        public int? AdministrationGroupClassificationId { get; set; }
        public int? LineOfBusinessId { get; set; }
        public string? AdministrationGroupIdProtected { get; set; }
    }
}
