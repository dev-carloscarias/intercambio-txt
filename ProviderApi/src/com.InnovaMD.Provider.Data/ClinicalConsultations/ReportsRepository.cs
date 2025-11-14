using com.InnovaMD.Provider.Data.Common;
using com.InnovaMD.Provider.Models.Common;
using com.InnovaMD.Provider.Models.SystemConfiguration;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations
{
    public class ReportsRepository : RepositoryBase, IReportsRepository
    {
        private ReportsConfigurationModel _reportsConfigurationModel;
        private readonly IDistributedCache _cache;

        public ReportsRepository(ReportsConfigurationModel reportsConfigurationModel,
            IDistributedCache cache, ConnectionStringOptions connectionStringOptions) : base(connectionStringOptions)
        {
            _reportsConfigurationModel = reportsConfigurationModel;
            _cache = cache;
        }

        public async Task<string> GetAzureAppAccessToken()
        {
            try
            {
                var cacheKey = $"{_reportsConfigurationModel.ReportsAuthority}-{_reportsConfigurationModel.ReportsClientId}";
                var cachedTokenInfo = _cache?.GetString(cacheKey);
                if (cachedTokenInfo != null)
                {
                    var jo = JObject.Parse(cachedTokenInfo);
                    string accessToken = (string)jo["AccessToken"];
                    DateTime expires = (DateTime)jo["ExpiresOn"];

                    if (expires > DateTime.Now)
                    {
                        return accessToken;
                    }
                }

                var app = ConfidentialClientApplicationBuilder
                            .Create(_reportsConfigurationModel.ReportsClientId)
                            .WithClientSecret(_reportsConfigurationModel.ReportsClientSecret)
                            .WithAuthority(_reportsConfigurationModel.ReportsAuthority)
                            .Build();

                var scopes = new string[] { $"{_reportsConfigurationModel.ReportsResource}/.default" };

                var authenticationResult = await app.AcquireTokenForClient(scopes)
                    .ExecuteAsync()
                    .ConfigureAwait(false);

                if (authenticationResult.AccessToken != null)
                {
                    _cache?.SetString(cacheKey, JsonConvert.SerializeObject(new
                    {
                        authenticationResult.AccessToken,
                        authenticationResult.ExpiresOn
                    }), new DistributedCacheEntryOptions());
                }
                else
                {
                    throw new Exception($"The authorization did not return a token.");
                }


                return authenticationResult.AccessToken;
            }
            catch (Exception e)
            {
                throw new Exception($"An unexpected error ocurred for GetAzureAppAccessTokn on [{_reportsConfigurationModel.ReportsAuthority}].", e);
            }
        }
         

        #region IDisposable
        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
