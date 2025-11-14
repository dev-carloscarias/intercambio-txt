using com.InnovaMD.Provider.Data.ClinicalConsultations.Queries;
using com.InnovaMD.Provider.Data.Common;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.Common;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations
{
    public class ProviderRepository : RepositoryBase, IProviderRepository
    {
        private readonly IDistributedCache _cache;

        public ProviderRepository(ConnectionStringOptions connectionStringOptions, IDistributedCache cache) : base(connectionStringOptions)
        {
            _cache = cache;
        }

        public int FindProvider(IEnumerable<int> providerAffiliationIds)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var providerId = dao.Find<int>(
                        QueriesProvider.FindProviderId(),
                        parameters: new
                        {
                            ProviderAffiliationIds = providerAffiliationIds
                        }).FirstOrDefault();

                    return providerId;
                }
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
