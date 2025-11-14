using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations.Strategies.Queries
{
    internal static class ProviderQueriesRequestingProvider
    {
        public static string SearchRequestingProviderByBilling(RequestingProviderSearchCriteria criteria, bool count = false)
        {
            if (count)
            {
                return @$"
                    SELECT COUNT(DISTINCT p.[ProviderAffiliationId])
                        FROM [dbo].[ProviderDirectory] p 
                             JOIN [dbo].[ProductNetworkLineOfBusiness] pnl ON pnl.[NetworkId] = p.[NetworkId]
                    WHERE pnl.[LineOfBusinessId] = @LineOfBusinessId AND p.[BillingProviderId] = @BillingProviderId AND p.RenderingProviderEntityTypeId = {(int)ProviderEntityTypes.Individual}
                        {(!string.IsNullOrEmpty(criteria.ProviderName) ? " AND (p.[RenderingProviderName] LIKE @ProviderName OR (p.[BillingProviderName] LIKE @ProviderName AND p.IsMSOClinic = 0) OR (p.[ProviderLocationName] LIKE @ProviderName AND p.IsMSOClinic = 1))" : string.Empty)}
                        {(!string.IsNullOrEmpty(criteria.NPI) ? " AND (p.[RenderingProviderNPI] LIKE @ProviderNPI OR p.[BillingProviderNPI] LIKE @ProviderNPI)" : string.Empty)}
                ";
            }

            return @$"
                SELECT 
                       p.[ProviderAffiliationId]
                      ,p.[RenderingProviderId]
                      ,p.[RenderingProviderNPI]
                      ,p.[RenderingProviderName]
                      ,p.[BillingProviderId]
                      ,p.[BillingProviderNPI]
                      ,p.[BillingProviderName]
                      ,p.[Email]
                      ,MIN(p.PhoneNumber) as PhoneNumber
                      ,MIN(CASE WHEN p.IsMSOClinic = 1 THEN p.[ProviderLocationName] ELSE p.[BillingProviderName] END) FacilityName
                FROM [dbo].[ProviderDirectory] p 
                     JOIN [dbo].[ProductNetworkLineOfBusiness] pnl ON pnl.[NetworkId] = p.[NetworkId]
                WHERE pnl.[LineOfBusinessId] = @LineOfBusinessId AND p.[BillingProviderId] = @BillingProviderId AND p.RenderingProviderEntityTypeId = {(int)ProviderEntityTypes.Individual}
                        {(!string.IsNullOrEmpty(criteria.ProviderName) ? " AND (p.[RenderingProviderName] LIKE @ProviderName OR (p.[BillingProviderName] LIKE @ProviderName AND p.IsMSOClinic = 0) OR (p.[ProviderLocationName] LIKE @ProviderName AND p.IsMSOClinic = 1))" : string.Empty)}
                        {(!string.IsNullOrEmpty(criteria.NPI) ? " AND (p.[RenderingProviderNPI] LIKE @ProviderNPI OR p.[BillingProviderNPI] LIKE @ProviderNPI)" : string.Empty)}
                GROUP BY 
                       p.[ProviderAffiliationId]
                      ,p.[RenderingProviderId]
                      ,p.[RenderingProviderNPI]
                      ,p.[RenderingProviderName]
                      ,p.[BillingProviderId]
                      ,p.[BillingProviderNPI]
                      ,p.[BillingProviderName]
                      ,p.[Email]
                ORDER BY p.[RenderingProviderName], p.[ProviderAffiliationId]
                OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY
            ";
        }

        public static string SearchRequestingProviders(RequestingProviderSearchCriteria criteria, bool count = false)
        {
            if (count)
            {
                return @$"
                    SELECT COUNT(DISTINCT p.[ProviderAffiliationId])
                        FROM [dbo].[ProviderDirectory] p 
                             JOIN [dbo].[ProductNetworkLineOfBusiness] pnl ON pnl.[NetworkId] = p.[NetworkId]
                    WHERE pnl.[LineOfBusinessId] = @LineOfBusinessId AND p.RenderingProviderEntityTypeId = {(int)ProviderEntityTypes.Individual}
                        {(!string.IsNullOrEmpty(criteria.ProviderName) ? " AND (p.[RenderingProviderName] LIKE @ProviderName OR (p.[BillingProviderName] LIKE @ProviderName AND p.IsMSOClinic = 0) OR (p.[ProviderLocationName] LIKE @ProviderName AND p.IsMSOClinic = 1))" : string.Empty)}
                        {(!string.IsNullOrEmpty(criteria.NPI) ? " AND (p.[RenderingProviderNPI] LIKE @ProviderNPI OR p.[BillingProviderNPI] LIKE @ProviderNPI)" : string.Empty)}
                ";
            }

            return @$"
                SELECT  
                       p.[ProviderAffiliationId]
                      ,p.[RenderingProviderId]
                      ,p.[RenderingProviderNPI]
                      ,p.[RenderingProviderName]
                      ,p.[BillingProviderId]
                      ,p.[BillingProviderNPI]
                      ,p.[BillingProviderName]
                      ,p.[Email]
                      ,MIN(p.PhoneNumber) as PhoneNumber
                      ,MIN(CASE WHEN p.IsMSOClinic = 1 THEN p.[ProviderLocationName] ELSE p.[BillingProviderName] END) FacilityName
                FROM [dbo].[ProviderDirectory] p 
                     JOIN [dbo].[ProductNetworkLineOfBusiness] pnl ON pnl.[NetworkId] = p.[NetworkId]
                WHERE  pnl.[LineOfBusinessId] = @LineOfBusinessId AND p.RenderingProviderEntityTypeId = {(int)ProviderEntityTypes.Individual}
                        {(!string.IsNullOrEmpty(criteria.ProviderName) ? " AND (p.[RenderingProviderName] LIKE @ProviderName OR (p.[BillingProviderName] LIKE @ProviderName AND p.IsMSOClinic = 0) OR (p.[ProviderLocationName] LIKE @ProviderName AND p.IsMSOClinic = 1))" : string.Empty)}
                        {(!string.IsNullOrEmpty(criteria.NPI) ? " AND (p.[RenderingProviderNPI] LIKE @ProviderNPI OR p.[BillingProviderNPI] LIKE @ProviderNPI)" : string.Empty)}
                GROUP BY 
                       p.[ProviderAffiliationId]
                      ,p.[RenderingProviderId]
                      ,p.[RenderingProviderNPI]
                      ,p.[RenderingProviderName]
                      ,p.[BillingProviderId]
                      ,p.[BillingProviderNPI]
                      ,p.[BillingProviderName]
                      ,p.[Email]
                ORDER BY p.[RenderingProviderName], p.[ProviderAffiliationId]
                OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY
            ";
        }

        public static string GetProvidersLocations()
        {
            return @$"
                SELECT DISTINCT 
                       p.[ProviderAffiliationId]
                      ,p.[CityId]
                      ,p.[City] AS [Name]
                FROM  [dbo].[ProviderDirectory] p
                      JOIN [dbo].[ProductNetworkLineOfBusiness] pnl ON pnl.[NetworkId] = p.[NetworkId]
                WHERE pnl.[LineOfBusinessId] = @LineOfBusinessId AND p.ProviderAffiliationId IN @ProviderAffiliationIds
                ORDER BY p.[ProviderAffiliationId], p.[City] 
            ";
        }

        public static string GetProvidersSpecialties()
        {
            return @$"
                SELECT DISTINCT 
                       p.[ProviderAffiliationId]
                      ,p.[SpecialtyId]
                      ,p.[IsPrimarySpecialty]
                      ,p.[SpecialtyName] AS [Name]
                FROM  [dbo].[ProviderDirectory] p
                      JOIN [dbo].[ProductNetworkLineOfBusiness] pnl ON pnl.[NetworkId] = p.[NetworkId]
                WHERE pnl.[LineOfBusinessId] = @LineOfBusinessId AND p.ProviderAffiliationId IN @ProviderAffiliationIds
                ORDER BY p.[ProviderAffiliationId], p.[IsPrimarySpecialty], p.[SpecialtyName]
            ";
        }
    }
}
