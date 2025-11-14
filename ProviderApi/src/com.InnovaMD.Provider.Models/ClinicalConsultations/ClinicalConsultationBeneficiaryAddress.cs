using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ClinicalConsultationBeneficiaryAddress
    {
          public int? ClinicalConsultationBeneficiaryId {get; set;}
          public int? AddressTypeId {get; set;}
          public string AddressLine1 {get; set;}
          public string AddressLine2 {get; set;}
          public string AddressLine3 {get; set;}
          public string City {get; set;}
          public string State {get; set;}
          public string Place {get; set;}
          public string ZIPCode {get; set;}
          public string ZIP4Code {get; set;}
          public string CountryFIPSCode {get; set;}
          public bool? IsPrimary { get; set; }
    }     
}
