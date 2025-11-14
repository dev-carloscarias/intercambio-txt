

using com.InnovaMD.Provider.Business.Strategies;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.InnovaMD.Provider.Business.Factories
{
    public class ClinicalConsultationStrategyFactory
    {
        private IEnumerable<IClinicalConsultationStrategy> _strategies;

        public ClinicalConsultationStrategyFactory(IEnumerable<IClinicalConsultationStrategy> strategies)
        {
                _strategies = strategies;
        }

        public IClinicalConsultationStrategy GetStrategy(string name)
        {
            return _strategies.FirstOrDefault(x =>
                        x.Name.Equals($"{name}ClinicalConsultationStrategy", StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
