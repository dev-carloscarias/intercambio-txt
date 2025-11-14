using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Requests;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.Models.Security;
using System;
using System.Collections.Generic;

namespace com.InnovaMD.Provider.Business
{
    public interface ICreateClinicalConsultationComponent: IDisposable
    {
        CreateConsultationConfigurationsResponseModel GetConfigurations();

        RequestingProviderOptionsResponse GetRequestingProviderOptions(int beneficiaryId, IdentityUser user);

        RequestingProviderSearchResponse SearchRequestingProvider(RequestingProviderSearchRequest request, IdentityUser user);

        RecentDiagnosesResponse GetRecentDiagnoses(int beneficiaryId);

        DiagnosisSearchResponse SearchDiagnosis(DiagnosisSearchRequest request, IdentityUser user);

        ServicesResponse GetServices(int beneficiaryId);
        ServicingProviderSearchResponse SearchServicingProvider(ServicingProviderSearchRequest request, IdentityUser user);

        ServicingProviderFiltersResponse GetServicingProviderFilters(ServicingProviderSearchRequest request);
        ServicingNonPPNReasonResponse GetServicingNonPPNReason(int beneficiaryId);

        public AdditionalInformationConfigurationsResponse GetAdditionalInformationConfigurations();
        public ServicingNonPPNReasonConfigurationResponse GetServicingNonPPNReasonConfigurations();

        public IEnumerable<AdditionalHealthPlan> GetHealthPlans();

        SubmitClinicalConsultationResponse SubmitClinicalConsultation(SubmitClinicalConsultationRequest request, IdentityUserExtended user);
    }
}