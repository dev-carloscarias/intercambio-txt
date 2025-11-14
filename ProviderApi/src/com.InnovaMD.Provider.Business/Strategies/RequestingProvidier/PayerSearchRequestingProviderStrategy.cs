using com.InnovaMD.Provider.Data.ClinicalConsultations;
using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Data.ClinicalConsultations.Strategies;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Business.Strategies.RequestingProvidier
{
    internal class PayerSearchRequestingProviderStrategy : ISearchRequestingProviderStrategy
    {
        public string Name { get; set; } = "PayerSearchRequestingProviderStrategy";
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly IPayerRequestingProviderRepository _repository;

        public PayerSearchRequestingProviderStrategy(
            IBeneficiaryRepository beneficiaryRepository,
            IPayerRequestingProviderRepository payerRequestingProviderRepository)
        {
            _beneficiaryRepository = beneficiaryRepository;
            _repository = payerRequestingProviderRepository;
        }

        public RequestingProviderSearchResponse SearchRequestingProvider(int beneficiaryId, IdentityUser user, RequestingProviderSearchCriteria searchCriteria)
        {
            var response = new RequestingProviderSearchResponse();

            var beneficiaryInformation = _beneficiaryRepository.GetBeneficiaryInformation(beneficiaryId);
            
            var (providers, count) = _repository.SearchRequestingProviders(searchCriteria, beneficiaryInformation.LineOfBusinessId.Value, user.HealthPlan.HealthPlanId);
            response.RequestingProviders = providers;
            response.Total = count;

            return response;
        }
    }
}
