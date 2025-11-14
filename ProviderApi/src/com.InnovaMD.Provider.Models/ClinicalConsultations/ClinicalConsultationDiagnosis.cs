using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ClinicalConsultationDiagnosis
    {
      public int DiagnosisId {get; set;}
      public string DiagnosisIdProtected {get; set;}
      public string Code {get; set;}
      public string Description {get; set;}
      public bool? IsPrimary { get; set; }
    }
   
}
