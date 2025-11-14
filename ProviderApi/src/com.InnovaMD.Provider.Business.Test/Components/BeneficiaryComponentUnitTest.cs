using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Utilities.Provider.Security.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace com.InnovaMD.Provider.Business.Test.Components
{
    public class BeneficiaryComponentUnitTest
    {
        private readonly IBeneficiaryComponent _component;

        public BeneficiaryComponentUnitTest()
        {
            Startup.Start();
            _component = Startup.ServiceProvider.GetService<IBeneficiaryComponent>();
        }


        [Fact]
        public void CheckButtonPersmision()
        {
            var beneficiaryId = 175463;
            var user = new Models.Security.IdentityUser()
            {
                UserId = 6383,
                ActiveRole = new IdentityRole()
                {
                    Context = new IdentityRoleApplicationDomainContext()
                    {
                        Id = (int)ApplicationDomainContexts.Provider

                    },
                    SubContext = new IdentityRoleApplicationDomainSubContext()
                    {
                        Id = (int)ApplicationDomainSubContexts.PCP
                    }

                }
            };
            var result = _component.CheckCreateButtonPermissions(beneficiaryId, user);

            Assert.NotNull(result);
        }
    }
}
