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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static IdentityModel.OidcConstants;

namespace com.InnovaMD.Provider.ClinicalConsultationApi.Controllers
{
    [ApiController]
    [Authorize(Policy = nameof(Policies.ViewClinicalConsultationHistory))]
    [Route("api/[controller]")]
    public class ClinicalConsultationController : ClinicalConsultationApiControllerBase
    {

        private readonly IClinicalConsultationHistoryComponent _clinicalConsultationComponent;
        private readonly IDocumentComponent _documentComponent;


        public ClinicalConsultationController(IClinicalConsultationHistoryComponent clinicalConsultationComponent,
            IDocumentComponent documentComponent,
            IDataProtectionProvider dataProtection) : base(dataProtection)
        {
            _clinicalConsultationComponent = clinicalConsultationComponent;
            _documentComponent = documentComponent;
        }

        [HttpPost]
        public IActionResult GetClinicalConsultations([FromBody] BeneficiaryClinicalConsultationsRequest request)
        {
            if (!int.TryParse(Protector.Unprotect(request.BeneficiaryId), out int beneficiaryId))
            {
                return BadRequest();
            }

            request.BeneficiaryId = beneficiaryId.ToString();

            var user = User.GetIdentityUser();

            var responseModel = _clinicalConsultationComponent.GetBeneficiaryClinicalConsultations(request, user);

            if (responseModel == null) { return new NoContentResult(); }

            foreach (var consultation in responseModel.ClinicalConsultations)
            {
                consultation.ClinicalConsultationIdProtected = Protector.Protect(consultation.ClinicalConsultationId.ToString());
            }

            return new ObjectResult(responseModel);
        }

        [HttpGet("{clinicalConsultationIdProtected}")]
        public IActionResult GetClinicalConsultationDetail(string clinicalConsultationIdProtected)
        {

            if (!int.TryParse(Protector.Unprotect(clinicalConsultationIdProtected), out int clinicalConsultationId))
            {
                return BadRequest();
            }

            var user = User.GetIdentityUser();

            var responseModel = _clinicalConsultationComponent.GetClinicalConsultationDetail(clinicalConsultationId, user);

            if (responseModel == null) { return new NoContentResult(); }
            return new ObjectResult(responseModel);
        }


        [Authorize(Policy = nameof(Policies.AllowPrintOrDownloadClinicalConsultation))]
        [HttpPost("form")]
        public async Task<IActionResult> GenerateClinicalConsultationForm([FromBody] GenerateClinicalConsultationFormRequest request)
        {
            if (!int.TryParse(Protector.Unprotect(request.ClinicalConsultationIdProtected), out int clinicalConsultationId))
            {
                return BadRequest();
            }

            var report = await _clinicalConsultationComponent.GetClinicalConsultationForm(clinicalConsultationId);

            if (report == null)
            {
                return new NoContentResult();
            }

            var response = _documentComponent.StoreReportInCache(report);

            return new ObjectResult(response);
        }
    }
} 
