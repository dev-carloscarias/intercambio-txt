using com.InnovaMD.Provider.Data.ClinicalConsultations;
using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using Microsoft.Extensions.DependencyInjection;
using Pipelines.Sockets.Unofficial.Arenas;
using System;
using Xunit;


namespace com.InnovaMD.Provider.Data.Test.Repositories
{
    public class ClinicalConsultationRepositoryUnitTest
    {
        private readonly IClinicalConsultationHistoryRepository _repository;
        private readonly ICreateClinicalConsultationRepository _createClinicalConsultationRepository;

        public ClinicalConsultationRepositoryUnitTest()
        {
            Startup.Start();
            _repository = Startup.ServiceProvider.GetService<IClinicalConsultationHistoryRepository>();
            _createClinicalConsultationRepository = Startup.ServiceProvider.GetService<ICreateClinicalConsultationRepository>();
        }

        [Fact]
        public void GetBeneficiaryClinicalConsultationsTest()
        {
            var (clinicalConsultations, total) = _repository.GetBeneficiaryClinicalConsultations(new BeneficiaryClinicalConsultationSearchCriteria()
            {
                BeneficiaryId = 175463,
                Page = 1,
                PageSize = 10,
            }, 6383);

            Assert.NotEmpty(clinicalConsultations);
            Assert.NotEqual(0, total);
        }

        [Fact]
        public void GetClinicalConsultationDetail()
        {
            var result = _repository.GetClinicalConsultationDetail(35,6383);

                Assert.NotNull(result);
        }
        [Fact]
        public void GetServices()
        {
            var result = _createClinicalConsultationRepository.GetServices(1);
            Assert.NotNull(result);
        }

        [Fact]
        public void GetSequenceNumber()
        {
            bool isClinicalConsultation = true;
            var result = "";
            if (isClinicalConsultation)
            {
                result = $@"CN{DateTime.Now.ToString("yyyyMMdd")}{_createClinicalConsultationRepository.GetSequenceNumber(isClinicalConsultation)}";
            }
            else
            {
                result = $@"RF{DateTime.Now.ToString("yyyyMMdd")}{_createClinicalConsultationRepository.GetSequenceNumber(isClinicalConsultation)}";
            }
           

            Assert.NotNull(result);
        }
    }
}
