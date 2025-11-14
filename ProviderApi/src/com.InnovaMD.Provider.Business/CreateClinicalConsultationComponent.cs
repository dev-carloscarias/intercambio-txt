using Azure.Core;
using Azure.Identity;
using com.InnovaMD.Provider.Business.Common;
using com.InnovaMD.Provider.Business.Factories;
using com.InnovaMD.Provider.Data.ClinicalConsultations;
using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Filters;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Requests;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.Models.Common;
using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Provider.Models.SystemConfiguration;
using com.InnovaMD.Utilities.Dates;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using static Microsoft.Azure.Amqp.Serialization.SerializableType;


namespace com.InnovaMD.Provider.Business
{
    public class CreateClinicalConsultationComponent : BusinessComponentBase, ICreateClinicalConsultationComponent
    {
        private readonly ILogger<CreateClinicalConsultationComponent> _logger;
        private readonly ICreateClinicalConsultationRepository _createClinicalConsultationRepository;
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly ClinicalConsultationConfigurationModel _clinicalConsultationModel;
        private readonly GlobalConfigurationModel _globalConfigurationModel;
        private SearchRequestingProviderStrategyFactory _searchRequestingProviderFactory;

        public CreateClinicalConsultationComponent(
            ILogger<CreateClinicalConsultationComponent> logger,
            ICreateClinicalConsultationRepository createClinicalConsultationRepository,
            ClinicalConsultationConfigurationModel clinicalConsultationModel,
            IBeneficiaryRepository beneficiaryRepository,
            GlobalConfigurationModel globalConfigurationModel,
             SearchRequestingProviderStrategyFactory searchRequestingProviderFactory)
        {
            _logger = logger;
            _createClinicalConsultationRepository = createClinicalConsultationRepository;
            _clinicalConsultationModel = clinicalConsultationModel;
            _beneficiaryRepository = beneficiaryRepository;
            _searchRequestingProviderFactory = searchRequestingProviderFactory;
            _globalConfigurationModel = globalConfigurationModel;

        }

        #region IDisposable
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        public CreateConsultationConfigurationsResponseModel GetConfigurations()
        {
            var response = new CreateConsultationConfigurationsResponseModel();
            response.CancelConsultationCreateMessage = _clinicalConsultationModel.CancelCreateConsultationMessage;
            response.AlertConsultationCreateCompleteStepMessage = _clinicalConsultationModel.CreateConsultationCompleteStepsValidationMsg;
            response.SubmitValidationMessage = _clinicalConsultationModel.CreateConsultationSubmitValidationMsg;
            return response;
        }
        
        public ServicingNonPPNReasonResponse GetServicingNonPPNReason(int beneficiaryId)
        {
            var beneficiaryInfo = _beneficiaryRepository.GetBeneficiaryInformation(beneficiaryId);
            int lineOfBusinessId = (int)beneficiaryInfo.LineOfBusinessId;
            var response = _createClinicalConsultationRepository.GetServicingNonPPNReason(lineOfBusinessId);
            
            return response;
        }

        public RequestingProviderOptionsResponse GetRequestingProviderOptions(int beneficiaryId, IdentityUser user)
        {
            var response = new RequestingProviderOptionsResponse();
            var beneficiaryInfo = _beneficiaryRepository.GetBeneficiaryInformation(beneficiaryId);

            if (user.ActiveRole.Context.Id == (int)ApplicationDomainContexts.Provider
                && ((int)ApplicationDomainSubContexts.PCP == user.ActiveRole.SubContext.Id
                  || (int)ApplicationDomainSubContexts.Specialist == user.ActiveRole.SubContext.Id))
            {
                response.RequestingProvider = _createClinicalConsultationRepository.FindRequestingProvider(beneficiaryInfo.LineOfBusinessId.Value, user.ProviderAffiliations.ToArray());
            }
            else
            {
                if (beneficiaryInfo.PCPAffiliationId.HasValue)
                {
                    response.BeneficiaryPcp = _createClinicalConsultationRepository.FindRequestingProvider(beneficiaryInfo.LineOfBusinessId.Value, beneficiaryInfo.PCPAffiliationId.Value);
                }
                else
                {
                    response.NoPCP = true;
                    response.NoPCPMessage = _clinicalConsultationModel.NoPCPMessage;
                }

                response.AllowSearch = true;
            }

            return response;
        }

        public RequestingProviderSearchResponse SearchRequestingProvider(RequestingProviderSearchRequest request, IdentityUser user)
        {
            string name = user.ActiveRole?.Context?.Name?.Replace(" ", "");
            var strategy = _searchRequestingProviderFactory.GetStrategy(name);

            var searchCriteria = new RequestingProviderSearchCriteria()
            {
                Page = request.Page,
                PageSize = _clinicalConsultationModel.SearchRequestingProviderPageSize
            };

            if (request.Search != null)
            {
                if (Regex.IsMatch(request.Search, @"^[0-9]*$"))
                {
                    searchCriteria.NPI = request.Search;
                }
                else
                {
                    searchCriteria.ProviderName = request.Search;
                }
            }

            return strategy.SearchRequestingProvider(int.Parse(request.BeneficiaryId), user, searchCriteria);
        }

        public RecentDiagnosesResponse GetRecentDiagnoses(int beneficiaryId)
        {
            var maxAmount = _clinicalConsultationModel.MaxNumberOfRecentDiagnoses;
            var diagnoses = _createClinicalConsultationRepository.GetRecentDiagnoses(beneficiaryId, maxAmount);

            var response = new RecentDiagnosesResponse()
            {
                Diagnoses = diagnoses
            };

            return response;
        }

        public DiagnosisSearchResponse SearchDiagnosis(DiagnosisSearchRequest request, IdentityUser user)
        {
            var searchCriteria = new DiagnosisSearchCriteria()
            {
                Page = request.Page,
                PageSize = _clinicalConsultationModel.SearchDiagnosisPageSize,
                Code = request.Search,
                Description = request.Search
            };

            var (total, diagnoses) = _createClinicalConsultationRepository.SearchDiagnosis(searchCriteria);

            var response = new DiagnosisSearchResponse()
            {
                Total = total,
                Diagnoses = diagnoses
            };

            return response;
        }

        public ServicesResponse GetServices(int beneficiaryId)
        {
            var beneficiaryInfo = _beneficiaryRepository.GetBeneficiaryInformation(beneficiaryId);

            var response = new ServicesResponse();

            response.ErrorMessageRequired = _clinicalConsultationModel.SERVICE_QUANTITY_REQUIRED;
            response.ErrorMessageMaximum = _clinicalConsultationModel.SERVICE_QUANTITY_MAXIMUM;
            response.ErrorMessageMinimum = _clinicalConsultationModel.SERVICE_QUANTITY_MINIMUM;

            _clinicalConsultationModel.LineOfBusinessId = beneficiaryInfo.LineOfBusinessId;

            response.Procedures = _createClinicalConsultationRepository.GetServices(beneficiaryInfo.LineOfBusinessId.Value);
            response.AllowAutoSelect = _clinicalConsultationModel.AllowProcedureBundleAutoSelect;

            return response;
        }

        public ServicingProviderSearchResponse SearchServicingProvider(ServicingProviderSearchRequest request, IdentityUser user)
        {
            var beneficiaryId = int.Parse(request.BeneficiaryId);
            var beneficiaryInfo = _beneficiaryRepository.GetBeneficiaryInformation(beneficiaryId);

            var searchCriteria = new ServicingProviderSearchCriteria()
            {
                Page = request.Page,
                PageSize = _clinicalConsultationModel.SearchRequestingProviderPageSize,
                BeneficiaryId = beneficiaryId,
                LineOfBusinessId = beneficiaryInfo.LineOfBusinessId.Value,
                AdministrationGroupId = !string.IsNullOrEmpty(request.AdministrationGroup) ? int.Parse(request.AdministrationGroup) : null,
                CityId = !string.IsNullOrEmpty(request.City) ? int.Parse(request.City) : null,
                SpecialtyId = !string.IsNullOrEmpty(request.Specialty) ? int.Parse(request.Specialty) : null,
                StateId = !string.IsNullOrEmpty(request.Country) ? int.Parse(request.Country) : _globalConfigurationModel.DefaultStateID,
                ZipCode = request.ZipCode,
                BeneficiaryCityId = beneficiaryInfo.CityId
            };

            if (request.Search != null)
            {
                if (Regex.IsMatch(request.Search, @"^[0-9]*$"))
                {
                    searchCriteria.NPI = request.Search;
                }
                else
                {
                    searchCriteria.ProviderName = request.Search;
                }
            }

            var (providers, count) = _createClinicalConsultationRepository.SearchServicingProvider(searchCriteria);

            var response = new ServicingProviderSearchResponse()
            {
                ServicingProviders = providers,
                Total = count
            };

            return response;
        }

        public ServicingProviderFiltersResponse GetServicingProviderFilters(ServicingProviderSearchRequest request)
        {
            var beneficiaryId = int.Parse(request.BeneficiaryId);
            var beneficiaryInfo = _beneficiaryRepository.GetBeneficiaryInformation(beneficiaryId);
            var response = new ServicingProviderFiltersResponse();
            var defaultState = _globalConfigurationModel.DefaultStateID;

            var filters = new ServicingProviderSearchCriteria()
            {
                LineOfBusinessId = beneficiaryInfo.LineOfBusinessId.Value,
                StateId = !string.IsNullOrEmpty(request.Country) ? int.Parse(request.Country) : defaultState,
            };

            if (filters.StateId == defaultState)
            {
                _globalConfigurationModel.LineOfBusinessId = beneficiaryInfo.LineOfBusinessId;
                var adminGroupTypeId = _globalConfigurationModel.AdministrationGroupTypes;

                response = _createClinicalConsultationRepository.GetServicingProviderFiltersDefaults(filters, adminGroupTypeId);
            }
            else
            {
                response = _createClinicalConsultationRepository.UpdateServicingProviderFilters(filters);
            }

            return response;
        }
        public AdditionalInformationConfigurationsResponse GetAdditionalInformationConfigurations()
        {
            var response = new AdditionalInformationConfigurationsResponse();
            response.ConsultationDaysBackAllowed = _clinicalConsultationModel.CreateConsultationDaysBackAllowed;
            response.CreateRequestConsultationMaximumAllowedMessage = _clinicalConsultationModel.CreateRequestConsultationMaximumAllowedMessage;
            response.RequestConsultationMaximumAllowedValue = _clinicalConsultationModel.CreateRequestConsultationMaximumAllowedValue;
            response.RuleOutsMessage = _clinicalConsultationModel.RuleOutsMessage;
            return response;
        }

        public ServicingNonPPNReasonConfigurationResponse GetServicingNonPPNReasonConfigurations()
        {
            var response = new ServicingNonPPNReasonConfigurationResponse();
            response.PPNReason = _clinicalConsultationModel.PPN_REASON;
            return response;
        }

        public IEnumerable<AdditionalHealthPlan> GetHealthPlans()
        {
            var response = _createClinicalConsultationRepository.GetHealthPlans();
            return response;
        }
        public SubmitClinicalConsultationResponse SubmitClinicalConsultation(SubmitClinicalConsultationRequest request, IdentityUserExtended user)
        {
            var beneficiaryInfo = _beneficiaryRepository.GetBeneficiaryInformation(int.Parse(request.BeneficiaryId));

            var beneficiary = PrepareBeneficiaryInformation(request);
            var pcp = beneficiaryInfo.PCPAffiliationId.HasValue ? PreparePCPInformation(request) : null;
            var requestingProvider = PrepareRequestingProvider(request);
            var diagnoses = PrepareDiagnoses(request);
            var service = PrepareProcedureBundle(request);
            var (servicingProvider, servicingSpecialty) = PrepareServicingProvider(request, beneficiaryInfo.LineOfBusinessId.Value);
            var additionalHealthPlan = PrepareAdditinalHealthPlan(request);
            var servicingNonPPNReason = PrepareServicingNonPPNReason(request);

            _clinicalConsultationModel.LineOfBusinessId = beneficiary.LineOfBusinessId.Value;

            var (isConsultation, commonAdminGroup) = GetIsClinicalConsultation(beneficiary, pcp, servicingProvider);

            if (commonAdminGroup != null)
            {
                servicingProvider.AdministrationGroupId = commonAdminGroup.AdministrationGroupId;
                servicingProvider.AdministrationGroupName = commonAdminGroup.Name;

                pcp.AdministrationGroupId = commonAdminGroup.AdministrationGroupId;
                servicingProvider.AdministrationGroupName = commonAdminGroup.Name;
            }

            var clinicalConsultation = new ClinicalConsultation()
            {
                ClinicalConsultationNumber = GetClinicalConsultationNumber(isConsultation),
                Beneficiary = beneficiary,
                PCP = pcp,
                Requesting = requestingProvider,
                Diagnosis = diagnoses,
                Procedures = new List<ClinicalConsultationProcedureBundle> { service },
                Servicing = servicingProvider,
                ServicingSpecialty = servicingSpecialty,
                AnyContractedSpecialist = request.ServicingProvider.AllowAnyContractedSpecialist,
                ServicingNonPPNReason = servicingNonPPNReason,
                ClinicalConsultationDate = request.ConsultationDate.FromSystemTimezoneToUTC(),
                AdditionalHealthPlan = additionalHealthPlan,
                Purpose = request.Purpose,

                ExpirationDate = GetClinicalConsultationExpirationDate(_clinicalConsultationModel.CreateClinicalConsultationExpirationDate),
                EffectiveDate = DateTime.UtcNow,
                IsConsultation = isConsultation,

                CreatedUserId = user.UserId,
                CreatedBy = user.Name,
                SourceIdentifier = "IMD",
                IsRecreate = request.isRecreate,
                OriginalClinicalConsultationId = request.isRecreate ? long.Parse(request.OriginalClinicalConsultationId) : null
            };

            var clinicalConsultationId = _createClinicalConsultationRepository.Insert(clinicalConsultation);

            return new SubmitClinicalConsultationResponse()
            {
                ClinicalConsultationId = clinicalConsultationId
            };
        }

        #region "Submit Helpers"
        private ClinicalConsultationBeneficiary PrepareBeneficiaryInformation(SubmitClinicalConsultationRequest request)
        {
            var beneficiaryId = int.Parse(request.BeneficiaryId);
            return _createClinicalConsultationRepository.GetBeneficiaryInformationForClinicalConsultation(beneficiaryId);
        }

        private ClinicalConsultationProvider PreparePCPInformation(SubmitClinicalConsultationRequest request)
        {
            var beneficiaryId = int.Parse(request.BeneficiaryId);
            var pcp = _createClinicalConsultationRepository.GetPCPInformationForClinicalConsultation(beneficiaryId);

            if (pcp != null)
            {
                var adminGroup = pcp.AdministrationGroups?.FirstOrDefault();
                pcp.AdministrationGroupId = adminGroup?.AdministrationGroupId;
                pcp.AdministrationGroupName = adminGroup?.Name;
            }

            return pcp;
        }

        private ClinicalConsultationProvider PrepareRequestingProvider(SubmitClinicalConsultationRequest request)
        {
            var providerId = int.Parse(request.RequestingProvider.ProviderId);
            var billingProviderId = int.Parse(request.RequestingProvider.BillingProviderId);
            var cityId = int.Parse(request.RequestingProvider.CityId);

            return _createClinicalConsultationRepository.GetRequestingProviderInformationForClinicalConsultation(providerId, billingProviderId, cityId);
        }

        private IEnumerable<ClinicalConsultationDiagnosis> PrepareDiagnoses(SubmitClinicalConsultationRequest request)
        {
            var diagnosisIds = request.Diagnoses.Select(d => int.Parse(d.DiagnosisId));
            var primaryDiagnosisId = request.Diagnoses.Where(d => d.IsPrimary == true).Select(d => int.Parse(d.DiagnosisId)).SingleOrDefault(); //Only one diagnosis can me marked as primary.

            if (primaryDiagnosisId == 0)
            {
                throw new Exception("No primary diagnosis selected");
            }

            var diagnoses = _createClinicalConsultationRepository.GetDiagnosesInformationForClinicalConsultation(diagnosisIds);

            var orderedDiagnoses = diagnosisIds
                       .Select(id => diagnoses.FirstOrDefault(x => x.DiagnosisId == id))
                       .Where(x => x != null)
                       .ToList();

            foreach (var d in orderedDiagnoses)
            {
                d.IsPrimary = d.DiagnosisId == primaryDiagnosisId;
            }

            return orderedDiagnoses;
        }

        private ClinicalConsultationProcedureBundle PrepareProcedureBundle(SubmitClinicalConsultationRequest request)
        {
            if (request.Service.Units == 0)
            {
                throw new Exception("Invalid service units");
            }

            var procedureBundleId = int.Parse(request.Service.ProcedureBundleId);

            var procedure = _createClinicalConsultationRepository.GetProcedureBundleInformationForClinicalConsultation(procedureBundleId);

            procedure.Units = request.Service.Units;

            return procedure;
        }

        private (ClinicalConsultationProvider, Specialty) PrepareServicingProvider(SubmitClinicalConsultationRequest request, int lineOfBusinessId)
        {
            var specialtyId = int.Parse(request.ServicingProvider.SpecialtyId);

            if (request.ServicingProvider.AllowAnyContractedSpecialist)
            {
                var specialty = _createClinicalConsultationRepository.GetSpeciltyInformationForClinicalConsultation(specialtyId, lineOfBusinessId);

                if (specialty.AllowAnyContractedSpecialist == true)
                {
                    return (null, specialty);
                }

                throw new Exception("Invalid specialty");
            }
            else
            {
                var providerId = int.Parse(request.ServicingProvider.ProviderId);
                var billingProviderId = int.Parse(request.ServicingProvider.BillingProviderId);
                var cityId = int.Parse(request.ServicingProvider.CityId);

                var provider = _createClinicalConsultationRepository.GetServicingProviderInformationForClinicalConsultation(providerId, billingProviderId, cityId, specialtyId);

                switch (provider.Priority)
                {
                    case (int)ServicingProviderPriority.MSOClinicProvider:
                    case (int)ServicingProviderPriority.MSOClinics:
                        provider.ClinicalConsultationProviderClassificationId = (int)ClinicalConsultationProviderClassifications.MSOClinics;
                        break;
                    case (int)ServicingProviderPriority.Capitated:
                        provider.ClinicalConsultationProviderClassificationId = (int)ClinicalConsultationProviderClassifications.Capitated;
                        break;
                    case (int)ServicingProviderPriority.MSOPPN:
                        provider.ClinicalConsultationProviderClassificationId = (int)ClinicalConsultationProviderClassifications.MSOPPN;
                        break;
                    case (int)ServicingProviderPriority.Other:
                        provider.ClinicalConsultationProviderClassificationId = (int)ClinicalConsultationProviderClassifications.Others;
                        break;
                }

                return (provider, new Specialty()
                {
                    SpecialtyId = provider.Specialties.First().SpecialtyId,
                });
            }
        }

        private ClinicalConsultationServicingNonPPNReason PrepareServicingNonPPNReason(SubmitClinicalConsultationRequest request)
        {
            if (string.IsNullOrEmpty(request.ServicingProvider?.OutOfPPNReasonId)) return null;
            return _createClinicalConsultationRepository.GetServicingNonPPNReasonForClinicalConsultation(int.Parse(request.ServicingProvider.OutOfPPNReasonId));
        }

        private AdditionalHealthPlan PrepareAdditinalHealthPlan(SubmitClinicalConsultationRequest request)
        {
            if (string.IsNullOrEmpty(request.AdditionalHealthPlanId)) return null;

            var additionalHealthPlanId = int.Parse(request.AdditionalHealthPlanId);

            return _createClinicalConsultationRepository.GetAdditionalHealthPlanInformationForClinicalConsultation(additionalHealthPlanId);
        }

        private DateTime GetClinicalConsultationExpirationDate(int days)
        {
            return DateTime.UtcNow.AddDays(days);
        }

        private string GetClinicalConsultationNumber(bool isConsultation)
        {
            var sequence = _createClinicalConsultationRepository.GetSequenceNumber(isConsultation);

            if (isConsultation)
            {
                return $@"CN{DateTime.UtcNow.ToString("yyyyMMdd")}{sequence.ToString()}";
            }
            else
            {
                return $@"RF{DateTime.UtcNow.ToString("yyyyMMdd")}{sequence.ToString()}";
            }

        }

        private (bool, AdministrationGroup) GetIsClinicalConsultation(ClinicalConsultationBeneficiary beneficiary, ClinicalConsultationProvider pcp, ClinicalConsultationProvider servicingProvider)
        {
            if (beneficiary.LineOfBusinessId == (int)LinesOfBusiness.MA)
            {
                if (!beneficiary.IsReferralRequired)
                {
                    return (true, null);
                }

                if (beneficiary.IsReferralRequired && (bool)servicingProvider.IsPPN)
                {
                    return (true, null);
                }
            }
            else
            {
                if (pcp?.AdministrationGroups != null
                    && pcp.AdministrationGroups.Any()
                    && servicingProvider?.AdministrationGroups != null
                    && servicingProvider.AdministrationGroups.Any())
                {
                    var pcpAdminGroups = pcp.AdministrationGroups.Select(a => a.AdministrationGroupId).ToHashSet();

                    AdministrationGroup commonAdminGroup = null;

                    foreach (var servicingAdminGroup in servicingProvider.AdministrationGroups)
                    {
                        if (pcpAdminGroups.Contains(servicingAdminGroup.AdministrationGroupId))
                        {
                            commonAdminGroup = servicingAdminGroup;
                            break;
                        }
                    }

                    if (commonAdminGroup != null)
                    {
                        return (true, commonAdminGroup);
                    }
                }
            }

            return (false, null);
        }

        #endregion

    }
}
