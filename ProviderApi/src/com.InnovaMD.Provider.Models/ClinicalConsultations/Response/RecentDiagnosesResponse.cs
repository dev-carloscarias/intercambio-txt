using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Response
{
    public class RecentDiagnosesResponse
    {
        public IEnumerable<Diagnosis> Diagnoses { get; set; }
    }
}
