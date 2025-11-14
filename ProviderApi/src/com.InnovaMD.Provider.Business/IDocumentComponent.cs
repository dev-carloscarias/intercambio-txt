using com.InnovaMD.Provider.Models.Common;
using System;

namespace com.InnovaMD.Provider.Business
{
    public interface IDocumentComponent
    {
        Report RetrieveDocumentCache(Guid guid);
        CachedReport StoreReportInCache(Report report);
        CachedReport StoreReportInCache(Report report, long? slidingExpiration);
    }
}
