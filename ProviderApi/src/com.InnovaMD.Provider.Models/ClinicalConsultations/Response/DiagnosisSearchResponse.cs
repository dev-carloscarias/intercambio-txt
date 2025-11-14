using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class DiagnosisSearchResponse
    {
        public IEnumerable<Diagnosis> Diagnoses { get; set; }
        public int Total { get; set; }
    }
}
