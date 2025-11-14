using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Models.ClinicalConsultations;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations.Queries
{
    internal static class QueriesBeneficiary
    {
        public static string FindBeneficiaryInformation()
        {
            return @$"
                SELECT b.[BeneficiaryId]
                      ,b.[BeneficiaryIdentifierId]
                      ,b.[Identifier]
                      ,b.[CardNumber]
                      ,b.[FirstName]
                      ,b.[MiddleName]
                      ,b.[LastName]
                      ,b.[DisplayName]
                      ,b.[ProductId]
                      ,b.[LineOfBusinessId]
                      ,b.[LineOfBusiness]
                      ,b.[HealthPlanId]
                      ,b.[PCPAffiliationId]
                      ,b.[RenderingProviderId] [PCPRenderingProviderId]
                      ,b.[BillingProviderId] [PCPBillingProviderId]
                      ,ba.[CityId]
                      ,ba.[ZipCode]
                  FROM [BeneficiaryProfile].[vBeneficiary] b
                       LEFT JOIN [BeneficiaryProfile].[vBeneficiaryAddress] ba ON ba.[BeneficiaryId] = b.[BeneficiaryId] AND ba.[AddressTypeId] = {(int)AddressType.Home}
                WHERE b.[BeneficiaryId] = @BeneficiaryId
            ";
        }


        public static string FindBeneficiaryNetworks()
        {
            return @$"
                SELECT [NetworkId]
                  FROM [dbo].[ProductNetworkLineOfBusiness]
                WHERE [ProductId] = @ProductId AND [LineOfBusinessId] = @LineOfBusinessId
            ";
        }
    }
}
