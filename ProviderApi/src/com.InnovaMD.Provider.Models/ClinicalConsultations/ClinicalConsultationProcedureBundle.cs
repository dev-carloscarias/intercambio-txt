using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ClinicalConsultationProcedureBundle : ProcedureBundle
    {
          public long? ClinicalConsultationProcedureBundleId {get; set;}
          public long? ClinicalConsultationId {get; set;}
          public int? Units {get; set;}
    }
}
