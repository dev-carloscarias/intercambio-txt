using System;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Models.ClinicalConsultations.Requests
{
    public class SubmitClinicalConsultationRequest
    {
        public string BeneficiaryId { get; set; }

        public SubmitRequestingProvider RequestingProvider { get; set; }

        public  IEnumerable<SubmitDiagnosis> Diagnoses { get; set; }

        public SubmitProcedureBundle Service { get; set; }

        public SubmitServicingProvider ServicingProvider { get; set; }

        public  string Purpose { get; set; }

        public  DateTime ConsultationDate { get; set; }

        public string AdditionalHealthPlanId { get; set; }

        public bool isRecreate { get; set; }

        public string OriginalClinicalConsultationId { get; set; }

        public string RecreateFrom { get; set; }
    }

    public class SubmitRequestingProvider
    {
        public string ProviderId { get; set; }
        public string BillingProviderId { get; set; }
        public string CityId { get; set; }
    }

    public class SubmitDiagnosis
    {
        public string DiagnosisId { get; set; }
        public  bool IsPrimary { get; set; }
    }

    public class SubmitProcedureBundle
    {
        public string ProcedureBundleId { get; set; }
        public int Units { get; set; }
    }

    public class SubmitServicingProvider
    {
        public string ProviderId { get; set; }
        public string BillingProviderId { get; set; }
        public string CityId { get; set; }
        public string SpecialtyId { get; set; }
        public bool AllowAnyContractedSpecialist { get; set; }
        public string OutOfPPNReasonId { get; set; }
    }
}
