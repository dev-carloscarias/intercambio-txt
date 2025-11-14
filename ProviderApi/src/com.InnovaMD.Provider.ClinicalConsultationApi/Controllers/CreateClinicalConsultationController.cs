using Azure.Core;
using com.InnovaMD.Provider.Business;
using com.InnovaMD.Provider.ClinicalConsultationApi.Common;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Requests;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.PortalApi.Common;
using com.InnovaMD.Provider.PortalApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using static IdentityModel.OidcConstants;
using System.Linq;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Filters;

namespace com.InnovaMD.Provider.ClinicalConsultationApi.Controllers
{
    [ApiController]
    [Authorize(Policy = nameof(Policies.CreateClinicalConsultation))]
    [Route("api/[controller]")]
    public class CreateClinicalConsultationController : ClinicalConsultationApiControllerBase
    {
        private readonly ICreateClinicalConsultationComponent _component;

        public CreateClinicalConsultationController(ICreateClinicalConsultationComponent component,
            IDataProtectionProvider dataProtection) : base(dataProtection)
        {
            _component = component;
        }

        [HttpGet("configurations")]
        public IActionResult GetConfigurations()
        {
            var responseModel = _component.GetConfigurations();

            if (responseModel == null) { return new NoContentResult(); }

            return new ObjectResult(responseModel);
        }

        [HttpGet("requestingprovider/options/{beneficiaryIdProtected}")]
        public IActionResult GetRequestingProviderOptions(string beneficiaryIdProtected)
        {
            if (!int.TryParse(Protector.Unprotect(beneficiaryIdProtected), out int beneficiaryId))
            {
                return BadRequest();
            }

            var user = User.GetIdentityUser();

            var responseModel = _component.GetRequestingProviderOptions(beneficiaryId, user);

            if (responseModel == null) { return new NoContentResult(); }

            if (responseModel.RequestingProvider != null)
            {
                responseModel.RequestingProvider.RenderingProviderIdProtected = Protector.Protect(responseModel.RequestingProvider.RenderingProviderId.ToString());
                responseModel.RequestingProvider.BillingProviderIdProtected = Protector.Protect(responseModel.RequestingProvider.BillingProviderId.ToString());

                if (responseModel.RequestingProvider.Cities != null)
                {
                    foreach (var city in responseModel.RequestingProvider.Cities)
                    {
                        city.CityIdProtected = Protector.Protect(city.CityId.ToString());
                    }
                }
            }

            if (responseModel.BeneficiaryPcp != null)
            {
                responseModel.BeneficiaryPcp.RenderingProviderIdProtected = Protector.Protect(responseModel.BeneficiaryPcp.RenderingProviderId.ToString());
                responseModel.BeneficiaryPcp.BillingProviderIdProtected = Protector.Protect(responseModel.BeneficiaryPcp.BillingProviderId.ToString());

                if (responseModel.BeneficiaryPcp.Cities != null)
                {
                    foreach (var city in responseModel.BeneficiaryPcp.Cities)
                    {
                        city.CityIdProtected = Protector.Protect(city.CityId.ToString());
                    }
                }
            }

            return new ObjectResult(responseModel);
        }

        [HttpPost("requestingprovider/search")]
        public IActionResult SearchRequestingProvider([FromBody] RequestingProviderSearchRequest request)
        {
            if (!int.TryParse(Protector.Unprotect(request.BeneficiaryId), out int beneficiaryId))
            {
                return BadRequest();
            }

            request.BeneficiaryId = beneficiaryId.ToString();

            var user = User.GetIdentityUser();

            var responseModel = _component.SearchRequestingProvider(request, user);


            if (responseModel == null) { return new NoContentResult(); }


            foreach (var requesting in responseModel.RequestingProviders)
            {
                requesting.RenderingProviderIdProtected = Protector.Protect(requesting.RenderingProviderId.ToString());
                requesting.BillingProviderIdProtected = Protector.Protect(requesting.BillingProviderId.ToString());
                if (requesting.Cities != null)
                {
                    foreach (var city in requesting.Cities)
                    {
                        city.CityIdProtected = Protector.Protect(city.CityId.ToString());
                    }
                }
            }
            return new ObjectResult(responseModel);
        }

        [HttpGet("diagnosis/recent/{beneficiaryIdProtected}")]
        public IActionResult SearchDiagnosis(string beneficiaryIdProtected)
        {
            if (!int.TryParse(Protector.Unprotect(beneficiaryIdProtected), out int beneficiaryId))
            {
                return BadRequest();
            }

            var responseModel = _component.GetRecentDiagnoses(beneficiaryId);

            if (responseModel == null) { return new NoContentResult(); }

            foreach (var d in responseModel.Diagnoses)
            {
                d.DiagnosisIdProtected = Protector.Protect(d.DiagnosisId.ToString());
            }

            return new ObjectResult(responseModel);
        }

        [HttpPost("diagnosis/search")]
        public IActionResult SearchDiagnosis([FromBody] DiagnosisSearchRequest request)
        {
            if (!int.TryParse(Protector.Unprotect(request.BeneficiaryId), out int beneficiaryId))
            {
                return BadRequest();
            }

            request.BeneficiaryId = beneficiaryId.ToString();

            var user = User.GetIdentityUser();

            var responseModel = _component.SearchDiagnosis(request, user);

            if (responseModel == null) { return new NoContentResult(); }

            foreach (var d in responseModel.Diagnoses)
            {
                d.DiagnosisIdProtected = Protector.Protect(d.DiagnosisId.ToString());
            }

            return new ObjectResult(responseModel);
        }

        [HttpGet("services/{beneficiaryIdProtected}")]
        public IActionResult GetServices(string beneficiaryIdProtected)
        {
            if (!int.TryParse(Protector.Unprotect(beneficiaryIdProtected), out int beneficiaryId))
            {
                return BadRequest();
            }

            var responseModel = _component.GetServices(beneficiaryId);

            if (responseModel == null)
            {
                return new NoContentResult();
            }

            foreach (var d in responseModel.Procedures)
            {
                d.LineOfBusinessIdProtected = Protector.Protect(d.LineOfBusinessId.ToString());
                d.ProcedureBundleIdProtected = Protector.Protect(d.ProcedureBundleId.ToString());
            }
            return new ObjectResult(responseModel);
        }

        [HttpPost("servicingprovider/search")]
        public IActionResult SearchServicingProvider([FromBody] ServicingProviderSearchRequest request)
        {
            if (!int.TryParse(Protector.Unprotect(request.BeneficiaryId), out int beneficiaryId))
            {
                return BadRequest();
            }
            request.BeneficiaryId = beneficiaryId.ToString();

            if (!request.AdministrationGroup.IsNullOrEmpty())
            {
                if (!int.TryParse(Protector.Unprotect(request.AdministrationGroup), out int administrationGroupId))
                {
                    return BadRequest();
                }
                request.AdministrationGroup = administrationGroupId.ToString();
            }

            if (!request.Country.IsNullOrEmpty())
            {
                if (!int.TryParse(Protector.Unprotect(request.Country), out int countryId))
                {
                    return BadRequest();
                }

                request.Country = countryId.ToString();
            }

            if (!request.City.IsNullOrEmpty())
            {
                if (!int.TryParse(Protector.Unprotect(request.City), out int cityId))
                {
                    return BadRequest();
                }

                request.City = cityId.ToString();
            }

            if (!request.Specialty.IsNullOrEmpty())
            {
                if (!int.TryParse(Protector.Unprotect(request.Specialty), out int specialtyId))
                {
                    return BadRequest();
                }
                request.Specialty = specialtyId.ToString();
            }
            if (!request.ZipCode.IsNullOrEmpty())
            {
                if (Protector.Unprotect(request.ZipCode).IsNullOrEmpty())
                {
                    return BadRequest();
                }

                request.ZipCode = Protector.Unprotect(request.ZipCode);
            }

            var user = User.GetIdentityUser();

            var responseModel = _component.SearchServicingProvider(request, user);

            if (responseModel == null) { return new NoContentResult(); }

            foreach (var servicing in responseModel.ServicingProviders)
            {
                servicing.RenderingProviderIdProtected = Protector.Protect(servicing.RenderingProviderId.ToString());
                servicing.BillingProviderIdProtected = Protector.Protect(servicing.BillingProviderId.ToString());
                if (servicing.Cities != null)
                {
                    foreach (var city in servicing.Cities)
                    {
                        city.CityIdProtected = Protector.Protect(city.CityId.ToString());
                    }
                }

                if (servicing.Specialties != null)
                {
                    foreach (var specialty in servicing.Specialties)
                    {
                        specialty.SpecialtyIdProtected = Protector.Protect(specialty.SpecialtyId.ToString());
                    }
                }
            }
            return new ObjectResult(responseModel);
        }

        [HttpGet("servicingprovider/filters")]
        public IActionResult GetServicingProviderFilters([FromQuery] ServicingProviderSearchRequest request)
        {
            if (!int.TryParse(Protector.Unprotect(request.BeneficiaryId), out int beneficiaryId))
            {
                return BadRequest();
            }
            request.BeneficiaryId = beneficiaryId.ToString();


            var responseModel = _component.GetServicingProviderFilters(request);
            if (responseModel.County != null)
            {
                foreach (var county in responseModel.County)
                {
                    county.CountyIdProtected = Protector.Protect(county.CountyId.ToString());
                }
            }

            if (responseModel.State != null)
            {
                foreach (var state in responseModel.State)
                {
                    state.StateIdProtected = Protector.Protect(state.StateId.ToString());
                }
            }
            if (responseModel.AdministrationGroup != null)
            {
                foreach (var administrationGroup in responseModel.AdministrationGroup)
                {
                    administrationGroup.AdministrationGroupIdProtected = Protector.Protect(administrationGroup.AdministrationGroupId.ToString());
                }
            }

            if (responseModel.ZipCode != null)
            {
                foreach (var zipCode in responseModel.ZipCode)
                {
                    zipCode.ZipCodeValueProtected = Protector.Protect(zipCode.ZipCodeValue.ToString());
                }
            }
            if (responseModel.Specialty != null)
            {
                foreach (var specialty in responseModel.Specialty)
                {
                    specialty.SpecialtyIdProtected = Protector.Protect(specialty.SpecialtyId.ToString());

                }
            }
            return new ObjectResult(responseModel);
        }

        [HttpGet("servicingprovider/servicingNonPPNReason/{beneficiaryIdProtected}")]
        public IActionResult GetServicingNonPPNReason(string beneficiaryIdProtected)
        {
            if (!int.TryParse(Protector.Unprotect(beneficiaryIdProtected), out int beneficiaryId))
            {
                return BadRequest();
            }
            var responseModel = _component.GetServicingNonPPNReason(beneficiaryId);

            if (responseModel == null) { return new NoContentResult(); }

            if (responseModel.ServicingNonPPNReasons != null)
            {
                foreach (var reason in responseModel.ServicingNonPPNReasons)
                {
                    reason.ServicingNonPPNReasonIdProtected = Protector.Protect(reason.ServicingNonPPNReasonId.ToString());
                }
            }

            return new ObjectResult(responseModel);
        }

        [HttpGet("additionalinfo/configurations")]
        public IActionResult GetAdditionalInformationConfigurations()
        {
            var responseModel = _component.GetAdditionalInformationConfigurations();

            if (responseModel == null) { return new NoContentResult(); }

            return new ObjectResult(responseModel);
        }

        [HttpGet("additionalinfo/healthplans")]
        public IActionResult GetHealthPlans()
        {
            var responseModel = _component.GetHealthPlans();

            if (responseModel == null || !responseModel.Any()) { return new NoContentResult(); }

            foreach (var hp in responseModel)
            {
                hp.AdditionalHealthPlanIdProtected = Protector.Protect(hp.AdditionalHealthPlanId.ToString());
            }

            return new ObjectResult(responseModel);
        }

        [HttpGet("ppnreasons/configurations")]
        public IActionResult GetPPNReasonConfigurations()
        {
            var responseModel = _component.GetServicingNonPPNReasonConfigurations();

            if (responseModel == null) { return new NoContentResult(); }

            return new ObjectResult(responseModel);
        }

        [HttpPost("submit")]
        public IActionResult Submit([FromBody] SubmitClinicalConsultationRequest request)
        {
            if (!int.TryParse(Protector.Unprotect(request.BeneficiaryId), out int beneficiaryId))
            {
                return BadRequest();
            }
            request.BeneficiaryId = beneficiaryId.ToString();

            if (request.RequestingProvider != null)
            {
                if (!int.TryParse(Protector.Unprotect(request.RequestingProvider.ProviderId), out int providerId))
                {
                    return BadRequest();
                }
                request.RequestingProvider.ProviderId = providerId.ToString();

                if (!int.TryParse(Protector.Unprotect(request.RequestingProvider.BillingProviderId), out int billingProviderId))
                {
                    return BadRequest();
                }
                request.RequestingProvider.BillingProviderId = billingProviderId.ToString();

                if (!int.TryParse(Protector.Unprotect(request.RequestingProvider.CityId), out int cityId))
                {
                    return BadRequest();
                }
                request.RequestingProvider.CityId = cityId.ToString();
            }
            else
            {
                return BadRequest();
            }

            if (request.Diagnoses != null && request.Diagnoses.Any())
            {
                foreach (var d in request.Diagnoses)
                {
                    if (!int.TryParse(Protector.Unprotect(d.DiagnosisId), out int diagnosisId))
                    {
                        return BadRequest();
                    }

                    d.DiagnosisId = diagnosisId.ToString();
                }
            }
            else
            {
                return BadRequest();
            }

            if (request.Service != null)
            {
                if (!int.TryParse(Protector.Unprotect(request.Service.ProcedureBundleId), out int procedureBundleId))
                {
                    return BadRequest();
                }
                request.Service.ProcedureBundleId = procedureBundleId.ToString();
            }
            else
            {
                return BadRequest();
            }

            if (request.ServicingProvider != null)
            {
                if (!string.IsNullOrEmpty(request.ServicingProvider.ProviderId))
                {
                    if (!int.TryParse(Protector.Unprotect(request.ServicingProvider.ProviderId), out int providerId))
                    {
                        return BadRequest();
                    }
                    request.ServicingProvider.ProviderId = providerId.ToString();
                }

                if (!string.IsNullOrEmpty(request.ServicingProvider.BillingProviderId))
                {
                    if (!int.TryParse(Protector.Unprotect(request.ServicingProvider.BillingProviderId), out int billingProviderId))
                    {
                        return BadRequest();
                    }
                    request.ServicingProvider.BillingProviderId = billingProviderId.ToString();
                }

                if (!string.IsNullOrEmpty(request.ServicingProvider.CityId))
                {
                    if (!int.TryParse(Protector.Unprotect(request.ServicingProvider.CityId), out int cityId))
                    {
                        return BadRequest();
                    }
                    request.ServicingProvider.CityId = cityId.ToString();
                }

                if (!string.IsNullOrEmpty(request.ServicingProvider.SpecialtyId))
                {
                    if (!int.TryParse(Protector.Unprotect(request.ServicingProvider.SpecialtyId), out int specialtyId))
                    {
                        return BadRequest();
                    }
                    request.ServicingProvider.SpecialtyId = specialtyId.ToString();
                }

                if (!string.IsNullOrEmpty(request.ServicingProvider.OutOfPPNReasonId))
                {
                    if (!int.TryParse(Protector.Unprotect(request.ServicingProvider.OutOfPPNReasonId), out int outOfPPNReasonId))
                    {
                        return BadRequest();
                    }
                    request.ServicingProvider.OutOfPPNReasonId = outOfPPNReasonId.ToString();
                }
            }
            else
            {
                return BadRequest();
            }

            if (request.AdditionalHealthPlanId != null)
            {
                if (!int.TryParse(Protector.Unprotect(request.AdditionalHealthPlanId), out int additionalHealthPlanId))
                {
                    return BadRequest();
                }
                request.AdditionalHealthPlanId = additionalHealthPlanId.ToString();
            }

            if(request.OriginalClinicalConsultationId != null)
            {
                if (!int.TryParse(Protector.Unprotect(request.OriginalClinicalConsultationId), out int originalClinicalConsultationId))
                {
                    return BadRequest();
                }
                request.OriginalClinicalConsultationId = originalClinicalConsultationId.ToString();
            }

            var user = User.GetIdentityUserExtended();

            var responseModel = _component.SubmitClinicalConsultation(request, user);

            if (responseModel == null) { return new NoContentResult(); }

            responseModel.ClinicalConsultationIdProtected = Protector.Protect(responseModel.ClinicalConsultationId.ToString());

            return Ok(responseModel);
        }


        [HttpGet("recreate/consultation/{clinicalConsultationIdProtected}")]
        public IActionResult GetConsultationForRecreated(string clinicalConsultationIdProtected)
        {
            if (!int.TryParse(Protector.Unprotect(clinicalConsultationIdProtected), out int clinicalConsultationId))
            {
                return BadRequest();
            }

            var responseModel = _component.GetClinicalConsultationForRecreate(clinicalConsultationId);

            if (responseModel == null) { return new NoContentResult(); }

            responseModel.ClinicalConsultationIdProtected = Protector.Protect(responseModel.ClinicalConsultationId.ToString());

            if (responseModel.ServicingSpecialty != null)
            {
                responseModel.ServicingSpecialty.SpecialtyIdProtected = Protector.Protect(responseModel.ServicingSpecialty.SpecialtyId.ToString());
            }

            if (responseModel.ServicingCity != null)
            {
                responseModel.ServicingCity.CityIdProtected = Protector.Protect(responseModel.ServicingCity.CityId.ToString());
            }

            if (responseModel.RequestingCity != null)
            {
                responseModel.RequestingCity.CityIdProtected = Protector.Protect(responseModel.RequestingCity.CityId.ToString());
            }

            if (responseModel.ServicingNonPPNReason != null)
            {
                responseModel.ServicingNonPPNReason.ServicingNonPPNReasonIdProtected = Protector.Protect(responseModel.ServicingNonPPNReason.ServicingNonPPNReasonId.ToString());
            }

            if (responseModel.AdditionalHealthPlan != null)
            {
                responseModel.AdditionalHealthPlan.AdditionalHealthPlanIdProtected = Protector.Protect(responseModel.AdditionalHealthPlan.AdditionalHealthPlanId.ToString());
            }

            if (responseModel.Procedure != null)
            {
                responseModel.Procedure.ProcedureBundleIdProtected = Protector.Protect(responseModel.Procedure.ProcedureBundleId.ToString());
            }

            if (responseModel.Diagnoses != null)
            {
                foreach(var diagnosis in responseModel.Diagnoses)
                {
                    diagnosis.DiagnosisIdProtected = Protector.Protect(diagnosis.DiagnosisId.ToString());
                }
            }

            if (responseModel.RequestingProvider != null)
            {
                responseModel.RequestingProvider = protectRequestingProvider(responseModel.RequestingProvider);
            }

            if (responseModel.ServicingProvider != null)
            {
                responseModel.ServicingProvider = protecServicingProvider(responseModel.ServicingProvider);
            }

            return new ObjectResult(responseModel);
        }

        [HttpGet("suggestions/{beneficiaryIdProtected}")]
        public IActionResult SearchSuggestions(string beneficiaryIdProtected)
        {
            if (!int.TryParse(Protector.Unprotect(beneficiaryIdProtected), out int beneficiaryId))
            {
                return BadRequest();
            }
            var user = User.GetIdentityUser();

            var responseModel = _component.GetRecentSuggestions(beneficiaryId);

            if (responseModel == null) { return new NoContentResult(); }

            foreach (var consultation in responseModel)
            {

                consultation.ClinicalConsultationIdProtected = Protector.Protect(consultation.ClinicalConsultationId.ToString());

            }
            return new ObjectResult(responseModel);
        }


        #region ProtectedRegion
        private RequestingProvider protectRequestingProvider(RequestingProvider requestingProvider)
        {
            requestingProvider.RenderingProviderIdProtected = Protector.Protect(requestingProvider.RenderingProviderId.ToString());
            requestingProvider.BillingProviderIdProtected = Protector.Protect(requestingProvider.BillingProviderId.ToString());

            foreach (var city in requestingProvider.Cities)
            {
                city.CityIdProtected = Protector.Protect(city.CityId.ToString());
            }

            foreach (var specialty in requestingProvider.Specialties)
            {
                specialty.SpecialtyIdProtected = Protector.Protect(specialty.SpecialtyId.ToString());
            }
            return requestingProvider;
        } 

        private ServicingProvider protecServicingProvider(ServicingProvider servicingProvider)
        {

            servicingProvider.RenderingProviderIdProtected = Protector.Protect(servicingProvider.RenderingProviderId.ToString());
            servicingProvider.BillingProviderIdProtected = Protector.Protect(servicingProvider.BillingProviderId.ToString());

            foreach (var city in servicingProvider.Cities)
            {
                city.CityIdProtected = Protector.Protect(city.CityId.ToString());
            }

            foreach (var specialty in servicingProvider.Specialties)
            {
                specialty.SpecialtyIdProtected = Protector.Protect(specialty.SpecialtyId.ToString());
            }

            return servicingProvider;
        }
        #endregion
    }
}
