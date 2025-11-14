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
    internal class ProviderSearchRequestingProviderStrategy : ISearchRequestingProviderStrategy
    {
        public string Name { get; set; } = "ProviderSearchRequestingProviderStrategy";
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly IProviderRepository _providerRepository;
        private readonly IProviderRequestingProviderRepository _repository;

        public ProviderSearchRequestingProviderStrategy(
            IBeneficiaryRepository beneficiaryRepository,
            IProviderRepository providerRepository,
            IProviderRequestingProviderRepository providerRequestingProviderRepository)
        {
            _beneficiaryRepository = beneficiaryRepository;
            _repository = providerRequestingProviderRepository;
            _providerRepository = providerRepository;

        }

        public RequestingProviderSearchResponse SearchRequestingProvider(int beneficiaryId, IdentityUser user, RequestingProviderSearchCriteria searchCriteria)
        {
            var response = new RequestingProviderSearchResponse();

            var beneficiaryInformation = _beneficiaryRepository.GetBeneficiaryInformation(beneficiaryId);
            List<int> allowedSubContext = [(int)ApplicationDomainSubContexts.GroupPractice, (int)ApplicationDomainSubContexts.CAMP];
            if (allowedSubContext.Contains(user.ActiveRole.SubContext.Id))
            {
                var billingProviderId = user.ProviderId ?? _providerRepository.FindProvider(user.ProviderAffiliations);
                var (providers, count) = _repository.SearchRequestingProviders(searchCriteria, beneficiaryInformation.LineOfBusinessId.Value, billingProviderId);
                response.RequestingProviders = providers;
                response.Total = count;
            }
            else
            {
                var (providers, count) = _repository.SearchRequestingProviders(searchCriteria, beneficiaryInformation.LineOfBusinessId.Value);
                response.RequestingProviders = providers;
                response.Total = count;
            }

            return response;
        }
    }
}
