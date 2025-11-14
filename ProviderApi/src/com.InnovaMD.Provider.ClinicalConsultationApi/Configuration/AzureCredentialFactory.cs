using Azure.Identity;
using Microsoft.Extensions.Configuration;

namespace com.InnovaMD.Provider.PortalApi.Configuration
{
    public class AzureCredentialFactory
    {
        public static DefaultAzureCredential ConfigureCredentials(IConfiguration configuration)
        {
            var defaultAzureCredentialOptions = configuration.GetSection("DefaultAzureCredentialOptions").Get<DefaultAzureCredentialOptions>();
            return new DefaultAzureCredential(defaultAzureCredentialOptions);
        }
    }
}
