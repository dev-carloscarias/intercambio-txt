using com.InnovaMD.Utilities.Configuration;
using com.InnovaMD.Utilities.Configuration.OptionModels;

namespace com.InnovaMD.Provider.Models.SystemConfiguration
{
    public class GlobalConfigurationModel : BaseSystemConfigurationOptionModel
    {
        public GlobalConfigurationModel(ConfigurationOptions options) : base(options)
        {
            ScopeId = (int)ConfigurationScopes.Global;

        }

        public bool ReferralsShowDisclamerPrint => GetConfigValue(ConfigurationConstants.REFERRALS_SHOW_DISCLAIMER_PRINT, false);
        public string ReferralsDisclamerMessage => GetConfigValue(ConfigurationConstants.REFERRALS_DISCLAIMER_MESSAGE, default(string));

        public int DefaultStateID => GetConfigValue(ConfigurationConstants.DEFAULT_STATE_ID, 0);
        public int AdministrationGroupTypes => GetConfigValue(ConfigurationConstants.ADMINISTRATION_GROUP_TYPE, 0);



    }
}
