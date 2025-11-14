using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ClinicalConsultationProcedureBundle
    {
          public long? ClinicalConsultationProcedureBundleId {get; set;}
          public long? ClinicalConsultationId {get; set;}
          public int? ProcedureBundleId {get; set;}
          public string Description {get; set;}
          public int? Units {get; set;}
          public string ReferenceCode {get; set;}
          public string ServiceTypeCode { get; set; }
    }
}
