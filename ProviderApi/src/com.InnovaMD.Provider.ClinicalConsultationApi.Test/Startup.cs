using IdentityModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.PortalApi.Test
{
    [TestClass]
    public class Startup
    {
        [TestMethod]
        public async Task Main()
        {
            var client = new HttpClient();

            var disco = await client.GetDiscoveryDocumentAsync("https://dev-account.innovamd.com");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
                return;
            }

            var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "provider.portalapi",
                ClientSecret = "711534DG-H270-4058-80CB-1C89C192F69C",
                Scope = "provider_read"
            });

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                Console.WriteLine("Press Enter to exit.");
                Console.ReadLine();
                return;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            var apiClient = new HttpClient();
            apiClient.SetBearerToken(tokenResponse.AccessToken);

            var response = await apiClient.GetAsync("https://dev-account.innovamd.com/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}
