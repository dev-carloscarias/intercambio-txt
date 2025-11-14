using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Filters
{
    public class Specialty
    {
        public int SpecialtyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? TaxonomyCode { get; set; }
        public bool? AllowAnyContractedSpecialist { get; set; }
        public int? DefaultRoleId { get; set; }
        public int? ProviderEntityTypeId { get; set; }
        public bool? IsDirectoryDisplay { get; set; }
        public string? SpecialtyIdProtected { get; set; }
    }
}
