using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Filters
{
    public class County
    {
        public int CountyId { get; set; }
        public int StateId { get; set; }
        public string? FIPSCode { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? FIPSClassCode { get; set; }
        public int? CreatedUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ModifiedUserId { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string? CountyIdProtected { get; set; }
    }
}
