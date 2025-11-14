using com.InnovaMD.Provider.Business.Common;
using com.InnovaMD.Provider.Models.Common;
using com.InnovaMD.Utilities.DistributedCache;
using Microsoft.Extensions.Logging;
using System;

namespace com.InnovaMD.Provider.Business
{
    public class DocumentComponent : BusinessComponentBase, IDocumentComponent
    {
        private readonly IServerCache _serverCache;
        private readonly ILogger<DocumentComponent> _logger;

        public DocumentComponent(IServerCache serverCache, ILogger<DocumentComponent> logger)
        {
            _serverCache = serverCache;
            _logger = logger;
        }

        public Report RetrieveDocumentCache(Guid guid)
        {
            try
            {
                return _serverCache.Retrieve<Report>(guid.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving report with id {guid}", ex);
                return null;
            }
        }

        public CachedReport StoreReportInCache(Report report)
        {
            var key = Guid.NewGuid();

            _serverCache.Store(key.ToString(), report);

            return new CachedReport() { ReportId = key.ToString() };
        }

        public CachedReport StoreReportInCache(Report report, long? slidingExpiration)
        {
            var key = Guid.NewGuid();

            _serverCache.Store(key.ToString(), report, expiration: slidingExpiration);

            return new CachedReport() { ReportId = key.ToString() };
        }
    }
}
