using Azure.Core;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Requests;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Utilities.Provider.Security.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace com.InnovaMD.Provider.Business.Test.Components
{
    public class CreateClinicalConsultationComponentUnitTest
    {
        private readonly ICreateClinicalConsultationComponent _component;

        public CreateClinicalConsultationComponentUnitTest()
        {
            Startup.Start();
            _component = Startup.ServiceProvider.GetService<ICreateClinicalConsultationComponent>();
        }

        [Fact]
        public void GetRequestingProviderOptionsTest()
        {
            var beneficiaryId = 1711740;
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
                        Id = (int)ApplicationDomainSubContexts.Hospital
                    }
                }
            };

            var options = _component.GetRequestingProviderOptions(beneficiaryId, user);

            Assert.NotNull(options);
            Assert.NotNull(options.BeneficiaryPcp);
            Assert.True(options.AllowSearch);


            user = new Models.Security.IdentityUser()
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

            options = _component.GetRequestingProviderOptions(beneficiaryId, user);

            Assert.NotNull(options);
            Assert.Null(options.BeneficiaryPcp);
            Assert.False(options.AllowSearch);
        }

        [Fact]
        public void SearchRequestingProviderPayerTest()
        {
            var request = new RequestingProviderSearchRequest()
            {
                BeneficiaryId = 1711740.ToString(), //Beneficiary with lob = 2 to match healthPlanId
                Page = 1,
                Search = "Jose"
            };
            
            var user = new Models.Security.IdentityUser()
            {
                ActiveRole = new IdentityRole()
                {
                    Context = new IdentityRoleApplicationDomainContext()
                    {
                        Id = (int)ApplicationDomainContexts.Payer,
                        Name = nameof(ApplicationDomainContexts.Payer)
                    },
                },
                HealthPlan = new IdentityHealthPlan()
                {
                    HealthPlanId = 3
                }
            };

            var response = _component.SearchRequestingProvider(request, user);

            Assert.NotNull(response);
            Assert.NotEmpty(response.RequestingProviders);
            Assert.NotEqual(0, response.Total);
        }

        [Fact]
        public void SearchRequestingProviderProviderTest()
        {
            var request = new RequestingProviderSearchRequest()
            {
                BeneficiaryId = 1069543.ToString(),
                Page = 1,
            };

            var user = new Models.Security.IdentityUser()
            {
                ActiveRole = new IdentityRole()
                {
                    Context = new IdentityRoleApplicationDomainContext()
                    {
                        Id = (int)ApplicationDomainContexts.Provider,
                        Name = nameof(ApplicationDomainContexts.Provider)
                    },
                    SubContext = new IdentityRoleApplicationDomainSubContext()
                    {
                        Id = (int)ApplicationDomainSubContexts.Hospital,
                        Name = nameof(ApplicationDomainSubContexts.Hospital)
                    },
                },
            };

            var response = _component.SearchRequestingProvider(request, user);

            Assert.NotNull(response);
            Assert.NotEmpty(response.RequestingProviders);
            Assert.NotEqual(0, response.Total);
            
            user = new Models.Security.IdentityUser()
            {
                ActiveRole = new IdentityRole()
                {
                    Context = new IdentityRoleApplicationDomainContext()
                    {
                        Id = (int)ApplicationDomainContexts.Provider,
                        Name = nameof(ApplicationDomainContexts.Provider)
                    },
                    SubContext = new IdentityRoleApplicationDomainSubContext()
                    {
                        Id = (int)ApplicationDomainSubContexts.GroupPractice,
                        Name = nameof(ApplicationDomainSubContexts.GroupPractice)
                    },
                },
                ProviderId = 45219,
                ProviderAffiliations = [59621]
            };

            response = _component.SearchRequestingProvider(request, user);

            Assert.NotNull(response);
            Assert.NotEmpty(response.RequestingProviders);
            Assert.NotEqual(0, response.Total);
        }

        [Fact]
        public void SearchRequestingProviderAdminGroupTest()
        {
            var request = new RequestingProviderSearchRequest()
            {
                BeneficiaryId = 1069543.ToString(),
                Page = 1,
                Search = "j"
            };

            var user = new Models.Security.IdentityUser()
            {
                ActiveRole = new IdentityRole()
                {
                    Context = new IdentityRoleApplicationDomainContext()
                    {
                        Id = (int)ApplicationDomainContexts.AdministrationGroup,
                        Name = nameof(ApplicationDomainContexts.AdministrationGroup)
                    },
                },
                AdministrationGroup = new IdentityAdministrationGroup()
                {
                    AdministrationGroupId = 1,
                    TypeId = (int)AdministrationGroupTypes.MSO
                }
            };


            var response = _component.SearchRequestingProvider(request, user);

            Assert.NotNull(response);
            Assert.NotEmpty(response.RequestingProviders);
            Assert.NotEqual(0, response.Total);


            user = new Models.Security.IdentityUser()
            {
                ActiveRole = new IdentityRole()
                {
                    Context = new IdentityRoleApplicationDomainContext()
                    {
                        Id = (int)ApplicationDomainContexts.AdministrationGroup,
                        Name = nameof(ApplicationDomainContexts.AdministrationGroup)
                    },
                },
                AdministrationGroup = new IdentityAdministrationGroup()
                {
                    AdministrationGroupId = 116,
                    TypeId = (int)AdministrationGroupTypes.IPA
                }
            };


            response = _component.SearchRequestingProvider(request, user);

            Assert.NotNull(response);
            Assert.NotEmpty(response.RequestingProviders);
            Assert.NotEqual(0, response.Total);
        }


        [Fact]
        public void SearchRequestingProviderPayerNPISearchTest()
        {
            var testNPI = "1659305225";
            var request = new RequestingProviderSearchRequest()
            {
                BeneficiaryId = 1069543.ToString(), //Beneficiary with lob = 1 to match healthPlanId
                Page = 1,
                Search = testNPI.Substring(0, testNPI.Length - 1)
            };


            //-------------------------------------------------------------------------------------------------------------------
            // PAYER USER
            //-------------------------------------------------------------------------------------------------------------------
            var user = new Models.Security.IdentityUser()
            {
                ActiveRole = new IdentityRole()
                {
                    Context = new IdentityRoleApplicationDomainContext()
                    {
                        Id = (int)ApplicationDomainContexts.Payer,
                        Name = nameof(ApplicationDomainContexts.Payer)
                    },
                },
                HealthPlan = new IdentityHealthPlan()
                {
                    HealthPlanId = 1
                }
            };

            var response = _component.SearchRequestingProvider(request, user);

            Assert.NotNull(response);
            Assert.NotEmpty(response.RequestingProviders);
            Assert.NotEqual(0, response.Total);

            if (response.Total == 1)
            {
                Assert.Equal(testNPI, response.RequestingProviders.First().RenderingProviderNPI);
            }

            //-------------------------------------------------------------------------------------------------------------------
            // PROVIDER USER
            //-------------------------------------------------------------------------------------------------------------------
            user = new Models.Security.IdentityUser()
            {
                ActiveRole = new IdentityRole()
                {
                    Context = new IdentityRoleApplicationDomainContext()
                    {
                        Id = (int)ApplicationDomainContexts.Provider,
                        Name = nameof(ApplicationDomainContexts.Provider)
                    },
                    SubContext = new IdentityRoleApplicationDomainSubContext()
                    {
                        Id = (int)ApplicationDomainSubContexts.Hospital,
                        Name = nameof(ApplicationDomainSubContexts.Hospital)
                    },
                },
            };

            response = _component.SearchRequestingProvider(request, user);

            Assert.NotNull(response);
            Assert.NotEmpty(response.RequestingProviders);
            Assert.NotEqual(0, response.Total);

            if (response.Total == 1)
            {
                Assert.Equal(testNPI, response.RequestingProviders.First().RenderingProviderNPI);
            }

            user = new Models.Security.IdentityUser()
            {
                ActiveRole = new IdentityRole()
                {
                    Context = new IdentityRoleApplicationDomainContext()
                    {
                        Id = (int)ApplicationDomainContexts.Provider,
                        Name = nameof(ApplicationDomainContexts.Provider)
                    },
                    SubContext = new IdentityRoleApplicationDomainSubContext()
                    {
                        Id = (int)ApplicationDomainSubContexts.GroupPractice,
                        Name = nameof(ApplicationDomainSubContexts.GroupPractice)
                    },
                },
                ProviderId = 45219,
                ProviderAffiliations = [59621]
            };

            response = _component.SearchRequestingProvider(request, user);

            Assert.NotNull(response);
            Assert.NotEmpty(response.RequestingProviders);
            Assert.NotEqual(0, response.Total);

            if (response.Total == 1)
            {
                Assert.Equal(testNPI, response.RequestingProviders.First().RenderingProviderNPI);
            }


            //-------------------------------------------------------------------------------------------------------------------
            // ADMINISTRATION GROUP USER
            //-------------------------------------------------------------------------------------------------------------------
            user = new Models.Security.IdentityUser()
            {
                ActiveRole = new IdentityRole()
                {
                    Context = new IdentityRoleApplicationDomainContext()
                    {
                        Id = (int)ApplicationDomainContexts.AdministrationGroup,
                        Name = nameof(ApplicationDomainContexts.AdministrationGroup)
                    },
                },
                AdministrationGroup = new IdentityAdministrationGroup()
                {
                    AdministrationGroupId = 1,
                    TypeId = (int)AdministrationGroupTypes.MSO
                }
            };


            response = _component.SearchRequestingProvider(request, user);

            Assert.NotNull(response);
            Assert.NotEmpty(response.RequestingProviders);
            Assert.NotEqual(0, response.Total);

            if (response.Total == 1)
            {
                Assert.Equal(testNPI, response.RequestingProviders.First().RenderingProviderNPI);
            }


            user = new Models.Security.IdentityUser()
            {
                ActiveRole = new IdentityRole()
                {
                    Context = new IdentityRoleApplicationDomainContext()
                    {
                        Id = (int)ApplicationDomainContexts.AdministrationGroup,
                        Name = nameof(ApplicationDomainContexts.AdministrationGroup)
                    },
                },
                AdministrationGroup = new IdentityAdministrationGroup()
                {
                    AdministrationGroupId = 116,
                    TypeId = (int)AdministrationGroupTypes.IPA
                }
            };


            response = _component.SearchRequestingProvider(request, user);

            Assert.NotNull(response);
            Assert.NotEmpty(response.RequestingProviders);
            Assert.NotEqual(0, response.Total);

            if (response.Total == 1)
            {
                Assert.Equal(testNPI, response.RequestingProviders.First().RenderingProviderNPI);
            }

        }


        [Fact]
        public void GetServicesTest()
        {
            var beneficiaryId = 1069543; //BeneficiaryId with lob = 1
            var result = _component.GetServices(beneficiaryId);
            Assert.NotNull(result);
            Assert.NotEmpty(result.Procedures);
            Assert.True(result.AllowAutoSelect);

            beneficiaryId = 666669; //BeneficiaryId with lob = 2
            result = _component.GetServices(beneficiaryId);
            Assert.NotNull(result);
            Assert.NotEmpty(result.Procedures);
            Assert.False(result.AllowAutoSelect);
        }



        [Fact]
        public void SearchServicingProvider()
        {
            var request = new ServicingProviderSearchRequest()
            {
                BeneficiaryId = 1069543.ToString(),
                Specialty = "11" /*Cardiology*/,
                City = "3231" /*Yauco*/,
                Page = 1,
            };

            var user = new IdentityUser()
            {
                ActiveRole = new IdentityRole()
                {
                    Context = new IdentityRoleApplicationDomainContext()
                    {
                        Id = (int)ApplicationDomainContexts.Provider,
                        Name = nameof(ApplicationDomainContexts.Provider)
                    },
                    SubContext = new IdentityRoleApplicationDomainSubContext()
                    {
                        Id = (int)ApplicationDomainSubContexts.Hospital,
                        Name = nameof(ApplicationDomainSubContexts.Hospital)
                    },
                },
            };

            var report =  _component.SearchServicingProvider(request, user);
            Assert.NotNull(report);
        }

        [Fact]
        public void FilterServicingProvider()
        {
            var request = new ServicingProviderSearchRequest()
            {
                BeneficiaryId = 1069543.ToString(),
                Country = "10"
            };

            var report = _component.GetServicingProviderFilters(request);
            Assert.NotNull(report);
        }

        [Fact]
        public void SubmitClinicalConsultation()
        {
            var request1 = new SubmitClinicalConsultationRequest()
            {
                BeneficiaryId = "1069543",  //Beneficiario MA
                Purpose = "Record creado por Unit Test",
                ConsultationDate = DateTime.Now,
                AdditionalHealthPlanId = "17",
                isRecreate = false,
                OriginalClinicalConsultationId = null,
                RequestingProvider = new SubmitRequestingProvider()
                {
                    ProviderId = "32272",
                    BillingProviderId = "45219",
                    CityId = "3231"
                },
                Diagnoses = new List<SubmitDiagnosis>() { 
                    new SubmitDiagnosis() {
                        DiagnosisId = "44471",
                        IsPrimary = true,
                    },
                    new SubmitDiagnosis() {
                        DiagnosisId = "44465",
                        IsPrimary = false,
                    },
                    new SubmitDiagnosis() {
                        DiagnosisId = "44470",
                        IsPrimary = false,
                    },
                },
                ServicingProvider = new SubmitServicingProvider()
                {
                    ProviderId = "31848",
                    BillingProviderId = "60224",
                    CityId = "3231",
                    SpecialtyId = "11"
                },
                Service = new SubmitProcedureBundle()
                    {
                        ProcedureBundleId="1",
                        Units=6
                    } 
            };

            var user = new IdentityUserExtended()
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
                },
                Username = "jvelasco1",
                Name = "Juan C Velasco Cervilla"
            };

            var response1 = _component.SubmitClinicalConsultation(request1, user);
            Assert.NotNull(response1);
            Assert.NotEqual(0, response1.ClinicalConsultationId);

            var request2 = new SubmitClinicalConsultationRequest()
            {
                BeneficiaryId = "1052820",  //Beneficiario MA without pcp
                Purpose = "Record creado por Unit Test",
                ConsultationDate = DateTime.Now,
                AdditionalHealthPlanId = null,
                isRecreate = false,
                OriginalClinicalConsultationId = null,
                RequestingProvider = new SubmitRequestingProvider()
                {
                    ProviderId = "32272",
                    BillingProviderId = "45219",
                    CityId = "3231"
                },
                Diagnoses = new List<SubmitDiagnosis>() {
                    new SubmitDiagnosis() {
                        DiagnosisId = "44471",
                        IsPrimary = true,
                    },
                    new SubmitDiagnosis() {
                        DiagnosisId = "44465",
                        IsPrimary = false,
                    },
                    new SubmitDiagnosis() {
                        DiagnosisId = "44470",
                        IsPrimary = false,
                    },
                },
                ServicingProvider = new SubmitServicingProvider()
                {
                    SpecialtyId = "7",
                    AllowAnyContractedSpecialist = true,
                },
                Service = new SubmitProcedureBundle()
                {
                    ProcedureBundleId = "1",
                    Units = 6
                }
            };
            var response2 = _component.SubmitClinicalConsultation(request2, user);
            Assert.NotNull(response2);
            Assert.NotEqual(0, response2.ClinicalConsultationId);
        }
    }
}
