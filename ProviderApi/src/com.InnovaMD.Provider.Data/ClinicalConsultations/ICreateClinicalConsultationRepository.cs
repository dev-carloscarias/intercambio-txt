using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Filters;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using System;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations
{
    public interface ICreateClinicalConsultationRepository : IDisposable
    {
        RequestingProvider FindRequestingProvider(int lineOfBusinessId, params int[] providerAffiliationIds);
        (int total, IEnumerable<Diagnosis> diagnoses) SearchDiagnosis(DiagnosisSearchCriteria searchCriteria);
        IEnumerable<Diagnosis> GetRecentDiagnoses(int beneficiaryId, int maxAmount);
        IEnumerable<ProcedureBundle> GetServices(int lineOfBusinessId);
        (IEnumerable<ServicingProvider>, int?) SearchServicingProvider(ServicingProviderSearchCriteria criteria);
        ServicingProviderFiltersResponse GetServicingProviderFiltersDefaults(ServicingProviderSearchCriteria filter, int adminGroupTypeId);
        ServicingProviderFiltersResponse UpdateServicingProviderFilters(ServicingProviderSearchCriteria filter);
        ServicingNonPPNReasonResponse GetServicingNonPPNReason(int lineOfBusinessid);
        IEnumerable<AdditionalHealthPlan> GetHealthPlans();
        ClinicalConsultationBeneficiary GetBeneficiaryInformationForClinicalConsultation(int beneficiaryId);
        ClinicalConsultationProvider GetPCPInformationForClinicalConsultation(int beneficiaryId);
        ClinicalConsultationProvider GetRequestingProviderInformationForClinicalConsultation(int renderingProviderId, int billingProviderId, int cityId);
        IEnumerable<ClinicalConsultationDiagnosis> GetDiagnosesInformationForClinicalConsultation(IEnumerable<int> diagnosisIds);
        ClinicalConsultationProcedureBundle GetProcedureBundleInformationForClinicalConsultation(int procedureBundleId);
        ClinicalConsultationProvider GetServicingProviderInformationForClinicalConsultation(int renderingProviderId, int billingProviderId, int cityId, int specialtyId);
        ClinicalConsultationServicingNonPPNReason GetServicingNonPPNReasonForClinicalConsultation(int servicingNonPPNReasonId);
        Specialty GetSpeciltyInformationForClinicalConsultation(int specialtyId, int lineOfBusinessId);
        AdditionalHealthPlan GetAdditionalHealthPlanInformationForClinicalConsultation(int additionalHealthPlanId);
        long Insert(ClinicalConsultation clinicalConsultation);
        int GetSequenceNumber(bool isConsultation);
        RecreateClinicalConsultation GetClinicalConsultationForRecreate(int clinicalConsultationId, int lineOfBusinessId);
        int FindConsultationBeneficiaryId(int clinicalConsultationId);
        IEnumerable<SuggestionsResponse> GetRecentSuggestions(int beneficiaryId, int maxAmount);
    }
}
