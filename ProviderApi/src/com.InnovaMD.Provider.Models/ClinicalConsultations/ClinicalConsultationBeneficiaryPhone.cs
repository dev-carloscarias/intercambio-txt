using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ClinicalConsultationBeneficiaryPhone
    {
      public int? ClinicalConsultationBeneficiaryId {get; set;}
      public int? PhoneTypeId {get; set;}
      public string CountryCode {get; set;}
      public string AreaCode {get; set;}
      public string Exchange {get; set;}
      public string Number {get; set;}
      public string PhoneNumber {get; set;}
      public bool? IsPrimary { get; set; }
    }
}
