using com.InnovaMD.Provider.Models.ClinicalConsultations.Requests;
using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Utilities.Provider.Security.Models;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Xunit;

namespace com.InnovaMD.Provider.Business.Test.Components
{
    public class ClinicalConsultationComponentUnitTest
    {

        private readonly IClinicalConsultationHistoryComponent _component;

        public ClinicalConsultationComponentUnitTest()
        {
            Startup.Start();
            _component = Startup.ServiceProvider.GetService<IClinicalConsultationHistoryComponent>();
        }


        [Fact]
        public void GetBeneficiaryClinicalConsultationsAll()
        {
            var result = _component.GetBeneficiaryClinicalConsultations(new BeneficiaryClinicalConsultationsRequest()
            {
                BeneficiaryId = "725675",
                Page = 1
            }, new Models.Security.IdentityUser()
            {
                UserId = 6383
            });
            Assert.NotEmpty(result.ClinicalConsultations);
            Assert.NotEqual(0, result.TotalCount);
        }

        [Fact]
        public void GetBeneficiaryClinicalConsultationsTest()
        {
            var result = _component.GetBeneficiaryClinicalConsultations(new BeneficiaryClinicalConsultationsRequest()
            {
                BeneficiaryId = "175463",
                Search = "RF42025022702378",
                Page = 1
            }, new Models.Security.IdentityUser()
            {
                UserId = 6383
            });
            Assert.NotEmpty(result.ClinicalConsultations);
            Assert.NotEqual(0, result.TotalCount);
        }

        [Fact]
        public void GetBeneficiaryClinicalConsultationDetailTest()
        {
            
            var result = _component.GetClinicalConsultationDetail(1138, new Models.Security.IdentityUser()
            {
                UserId = 6383
            });
            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GenerateClinicalConsultationForm()
        {
            var clinicalConsultationId = 1147;
            var report = await _component.GetClinicalConsultationForm(clinicalConsultationId);
            Assert.NotNull(report);
        }
    }
}
