using com.InnovaMD.Provider.Data.ClinicalConsultations;
using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Data.ClinicalConsultations.Strategies;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.Models.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Business.Strategies.RequestingProvidier
{
    internal class AdministrationGroupSearchRequestingProviderStrategy : ISearchRequestingProviderStrategy
    {
        public string Name { get; set; } = "AdministrationGroupSearchRequestingProviderStrategy";
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly IAdministrationGroupRequestingProviderRepository _repository;

        public AdministrationGroupSearchRequestingProviderStrategy(
            IBeneficiaryRepository beneficiaryRepository,
            IAdministrationGroupRequestingProviderRepository administrationGroupRequestingProviderRepository)
        {
            _beneficiaryRepository = beneficiaryRepository;
            _repository = administrationGroupRequestingProviderRepository;
        }

        public RequestingProviderSearchResponse SearchRequestingProvider(int beneficiaryId, IdentityUser user, RequestingProviderSearchCriteria searchCriteria)
        {
            var response = new RequestingProviderSearchResponse();

            var beneficiaryInformation = _beneficiaryRepository.GetBeneficiaryInformation(beneficiaryId);
            var administrationGroup = user.AdministrationGroup;

            if (administrationGroup.TypeId == (int)AdministrationGroupTypes.SIPA
                || administrationGroup.TypeId == (int)AdministrationGroupTypes.SPMG
                || administrationGroup.TypeId == (int)AdministrationGroupTypes.MSO)
            {
                var (providers, count) = _repository.SearchRequestingProviderWithHierarchy(searchCriteria, administrationGroup.AdministrationGroupId, beneficiaryInformation.LineOfBusinessId.Value);
                response.RequestingProviders = providers;
                response.Total = count;
            }
            else
            {
                var (providers, count) = _repository.SearchRequestingProviders(searchCriteria, administrationGroup.AdministrationGroupId, beneficiaryInformation.LineOfBusinessId.Value);
                response.RequestingProviders = providers;
                response.Total = count;
            }

            return response;
        }
    }
}
