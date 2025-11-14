
using System;
using System.Collections;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations
{
    public class ClinicalConsultationBeneficiary
    {
        public int? ClinicalConsultationBeneficiaryId { get; set; }
        public long? ClinicalConsultationId {get; set;}
        public int BeneficiaryId {get; set;}
        public string CardNumber {get; set;}
        public string Name {get; set;}
        public string FirstName {get; set;}
        public string MiddleName {get; set;}
        public string LastName {get; set;}
        public string FullName { get; set; }
        public DateTime? BirthDate {get; set;}
        public int? LineOfBusinessId {get; set;}
        public string LineOfBusinessIDProtected { get; set; }
        public string LineOfBusinessShortName {get; set;}
        public int? HealthPlanId {get; set;}
        public string HealthPlanName {get; set;}
        public int? ProductId {get; set;}
        public bool IsReferralRequired { get; set; }
        public string ProductName {get; set;}
        public string Brand {get; set;}
        public string Identifier {get; set;}
        public int? BeneficiaryIdentifierId { get; set; }

        public IEnumerable<ClinicalConsultationBeneficiaryAddress> Addresses {get; set;}
        public IEnumerable<ClinicalConsultationBeneficiaryPhone> Phones { get; set;}

    }

   
}
