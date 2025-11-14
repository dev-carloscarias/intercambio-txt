using com.InnovaMD.Provider.Business.Strategies.RequestingProvidier;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.InnovaMD.Provider.Business.Factories
{
    public class SearchRequestingProviderStrategyFactory
    {
        private IEnumerable<ISearchRequestingProviderStrategy> _strategies;

        public SearchRequestingProviderStrategyFactory(IEnumerable<ISearchRequestingProviderStrategy> strategies)
        {
                _strategies = strategies;
        }

        public ISearchRequestingProviderStrategy GetStrategy(string name)
        {
            return _strategies.FirstOrDefault(x =>
                        x.Name.Equals($"{name}SearchRequestingProviderStrategy", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
