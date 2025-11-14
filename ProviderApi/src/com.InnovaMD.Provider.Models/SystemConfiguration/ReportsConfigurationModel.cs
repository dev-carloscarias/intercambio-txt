using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Utilities.Configuration;
using com.InnovaMD.Utilities.Configuration.OptionModels;

namespace com.InnovaMD.Provider.Models.SystemConfiguration
{
    public class ReportsConfigurationModel : BaseSystemConfigurationOptionModel
    {
        public ReportsConfigurationModel(ConfigurationOptions options) : base(options)
        {
            ScopeId = (int)ConfigurationScopes.PowerBiReports;
            ApplicationDomainId = (int)ApplicationDomains.ProviderPortal;
        }

        public string ReportsAuthority => GetConfigValue(ConfigurationConstants.POWERBI_REPORTS_AUTHORITY, null);
        public string ReportsResource => GetConfigValue(ConfigurationConstants.POWERBI_REPORTS_RESOURCE, null);
        public string ReportsClientId => GetConfigValue(ConfigurationConstants.POWERBI_REPORTS_CLIENT_IDENTIFIER, null);
        public string ReportsClientSecret => GetConfigValue(ConfigurationConstants.POWERBI_REPORTS_CLIENT_SECRET, null);
        public string PowerBiApiBaseURL => GetConfigValue(ConfigurationConstants.POWERBI_REPORTS_API_BASEURL, "https://api.powerbi.com/");

        public string ClinicalConsultationReportsGroupId => GetConfigValue(ConfigurationConstants.PORTAL_CLINICAL_CONSULTATION_REPORT_GROUP_ID, null);
        public string ClinicalConsultationReportId => GetConfigValue(ConfigurationConstants.PORTAL_CLINICAL_CONSULTATION_REPORT_ID, null);
        public int ClinicalConsultationExportReportTimeout => GetConfigValue(ConfigurationConstants.PORTAL_CLINICAL_CONSULTATION_REPORT_EXPORT_TIMEOUT, 3);
        public string ClinicalConsultationExportReportName => GetConfigValue(ConfigurationConstants.PORTAL_CLINICAL_CONSULTATION_REPORT_EXPORT_FILENAME, "ClinicalConsultationForm.pdf");

        public int RetryAfter => GetConfigValue(ConfigurationConstants.POWERBI_REPORTS_RETRY_AFTER, -1);
    }
}
