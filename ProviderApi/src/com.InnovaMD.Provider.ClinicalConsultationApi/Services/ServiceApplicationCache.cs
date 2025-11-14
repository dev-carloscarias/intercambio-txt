using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace com.InnovaMD.Provider.PortalApi.Services
{
    public static class ServiceApplicationCache
    {
        public static void ConfigureRedisCache(this IServiceCollection services, IConfiguration configuration, SecretClient keyVaultClient)
        {
            var redisCacheConnectionStrings = keyVaultClient.GetSecretAsync(configuration.GetValue<string>("ConnectionStringOptions:AzureRedisCacheConnectionStringSecretIdentifier")).Result.Value.Value;
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisCacheConnectionStrings;
                options.InstanceName = "master";
            });
        }
    }
}
