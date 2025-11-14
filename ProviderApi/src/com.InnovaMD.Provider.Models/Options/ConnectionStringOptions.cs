namespace com.InnovaMD.Provider.Models.Common
{
    public class ConnectionStringOptions
    {
        public string ClinicalConsultation { get; set; }
        public string AzureSeriviceBus { get; set; }
        public string RedisCacheConnectionString { get; set; }
        public bool IsAzureEnvironment { get; set; }
        public string AzureHCSSDBConnectionStringSecretIdentifier { get; set; }
        public string AzureSeriviceBusConnStringSecretIdentifier { get; set; }
    }
}
