using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Models.ClinicalConsultations;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations.Queries
{
    internal static class QueriesProvider
    {
        public static string FindProviderId()
        {
            return @$"
                SELECT [RenderingProviderId] 
                  FROM [dbo].[ProviderDirectory]
                WHERE [ProviderAffiliationId] in @ProviderAffiliationIds
            ";
        }
    }
}
