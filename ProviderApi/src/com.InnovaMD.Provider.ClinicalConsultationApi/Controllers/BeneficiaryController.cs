using com.InnovaMD.Provider.Business;
using com.InnovaMD.Provider.ClinicalConsultationApi.Common;
using com.InnovaMD.Provider.PortalApi.Common;
using com.InnovaMD.Provider.PortalApi.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace com.InnovaMD.Provider.ClinicalConsultationApi.Controllers
{
    [ApiController]
    [Authorize(Policy = nameof(Policies.BearerClinicalConsultation))]
    [Route("api/[controller]")]
    public class BeneficiaryController : ClinicalConsultationApiControllerBase
    {
        private readonly IBeneficiaryComponent _beneficiaryComponent;

        public BeneficiaryController(IBeneficiaryComponent beneficiaryComponent, 
            IDataProtectionProvider dataProtection) : base(dataProtection)
        {
            _beneficiaryComponent = beneficiaryComponent;
        }


        [HttpGet("information/{beneficiaryIdProtected}")]
        public IActionResult GetBeneficiaryInformation(string beneficiaryIdProtected)
        {
            if (!int.TryParse(Protector.Unprotect(beneficiaryIdProtected), out int beneficiaryId))
            {
                return BadRequest();
            }

            var responseModel = _beneficiaryComponent.GetBeneficiaryBasicInformation(beneficiaryId);

            if (responseModel == null) { return new NoContentResult(); }

            responseModel.LineOfBusinessIdProteted = Protector.Protect(responseModel.LineOfBusinessId.ToString());
            responseModel.BeneficiaryIdProtected = beneficiaryIdProtected;

            return new ObjectResult(responseModel);
        }

        [HttpGet("checkcreatebuttonpermissions/{beneficiaryIdProtected}")]
        public IActionResult CheckCreateButtonPermissions(string beneficiaryIdProtected)
        {

            var user = User.GetIdentityUser();
            if (!int.TryParse(Protector.Unprotect(beneficiaryIdProtected), out int beneficiaryId))
            {
                return BadRequest();
            }

            var responseModel = _beneficiaryComponent.CheckCreateButtonPermissions(beneficiaryId, user);

            if (responseModel == null)
            {
                return new NoContentResult();
            }
            return new ObjectResult(responseModel);
        }

    }
}
