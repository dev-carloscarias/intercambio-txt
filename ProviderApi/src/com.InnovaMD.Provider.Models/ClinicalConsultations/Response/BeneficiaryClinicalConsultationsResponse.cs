using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class BeneficiaryClinicalConsultationsResponse
    {
        public IEnumerable<ClinicalConsultation> ClinicalConsultations { get; set; }
        public int? TotalCount { get; set; }
    }
}
