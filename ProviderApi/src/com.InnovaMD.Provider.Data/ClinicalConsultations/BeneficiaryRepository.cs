using com.InnovaMD.Provider.Data.ClinicalConsultations.Queries;
using com.InnovaMD.Provider.Data.Common;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.Common;
using com.InnovaMD.Provider.Models.Log;
using com.InnovaMD.Utilities.DistributedCache;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations
{
    public class BeneficiaryRepository : RepositoryBase, IBeneficiaryRepository
    {
        private readonly IServerCache _cache;
        private readonly ILogger<BeneficiaryRepository> _logger;

        public BeneficiaryRepository(ConnectionStringOptions connectionStringOptions, IServerCache cache, ILogger<BeneficiaryRepository> logger) : base(connectionStringOptions)
        {
            _cache = cache;
            _logger = logger;
        }

        public BeneficiaryInformation GetBeneficiaryInformation(int beneficiaryId)
        {
            var cacheKey = $"beneficiaryinfo:{beneficiaryId}";
            try
            {
                var beneficiaryData = _cache?.Retrieve<BeneficiaryInformation>(cacheKey,appendPrefix: true);
                if (beneficiaryData != null)
                {
                    return beneficiaryData;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"An Error occurred searching for {cacheKey} in cache");
            }

            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var beneficiaryInfo = dao.Find<BeneficiaryInformation>(
                        QueriesBeneficiary.FindBeneficiaryInformation(),
                        parameters: new
                        {
                            BeneficiaryId = beneficiaryId
                        }).FirstOrDefault();

                    if (beneficiaryInfo != null)
                    {
                        beneficiaryInfo.Networks = dao.Find<int>(
                            QueriesBeneficiary.FindBeneficiaryNetworks(),
                             parameters: new
                             {
                                 ProductId = beneficiaryInfo.ProductId,
                                 LineOfBusinessId = beneficiaryInfo.LineOfBusinessId
                             }
                        );
                    }

                    try
                    {
                        _cache?.Store(cacheKey, JsonConvert.SerializeObject(beneficiaryInfo), appendPrefix: true);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, $"An Error occurred storing {cacheKey} in cache");
                    }


                    return beneficiaryInfo;
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
