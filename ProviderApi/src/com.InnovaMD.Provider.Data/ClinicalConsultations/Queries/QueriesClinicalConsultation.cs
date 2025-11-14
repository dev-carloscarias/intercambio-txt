using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Models.ClinicalConsultations;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations.Queries
{
    internal static class QueriesClinicalConsultation
    {
        public static string GetBeneficiaryClinicalConsultations(BeneficiaryClinicalConsultationSearchCriteria criteria)
        {
            return @$"SELECT 
                        r.[ClinicalConsultationId]
                       ,r.[ClinicalConsultationNumber]
                       ,r.[ClinicalConsultationDate]
                       ,r.[IsConsultation]
                       ,r.[CreatedBy]
                       ,r.[AnyContractedSpecialist]
                       ,r.[ServicingProviderSpecialtyId] SpecialtyId
                       ,s.[Name]
                       ,r.[CreatedDate]
                       ,vl.[ViewedOn]
                       ,sp.[Name] 
                       ,rp.[Name]
                       ,p.[Description]
                   FROM [ClinicalConsultation].[ClinicalConsultation] r 
                        INNER JOIN [ClinicalConsultation].[ClinicalConsultationBeneficiary] b ON b.[ClinicalConsultationId] = r.[ClinicalConsultationId]
                        LEFT JOIN [ClinicalConsultation].[ClinicalConsultationProvider] sp ON sp.[ClinicalConsultationId] = r.[ClinicalConsultationId] AND sp.[ClinicalConsultationProviderTypeId] = {(int) ClinicalConsultationProviderTypes.Servicing}
                        INNER JOIN [ClinicalConsultation].[ClinicalConsultationProvider] rp ON rp.[ClinicalConsultationId] = r.[ClinicalConsultationId] AND rp.[ClinicalConsultationProviderTypeId] = {(int)ClinicalConsultationProviderTypes.Requesting}
                        INNER JOIN [ClinicalConsultation].[ClinicalConsultationProcedureBundle] p ON p.[ClinicalConsultationId] = r.[ClinicalConsultationId]
                        LEFT JOIN [dbo].[vSpecialty] s ON s.[SpecialtyId] = r.[ServicingProviderSpecialtyId]
                        LEFT JOIN [ClinicalConsultation].[ViewLog] vl ON vl.[ClinicalConsultationId] = r.[ClinicalConsultationId] AND vl.UserId = @UserId
                  WHERE b.[BeneficiaryId] = @BeneficiaryId
                        {(!string.IsNullOrEmpty(criteria.ProviderName) ? " AND (sp.[Name] like @ProviderName OR rp.[Name] like @ProviderName)" : string.Empty)}
                        {(!string.IsNullOrEmpty(criteria.ClinicalConsultationNumber) ? " AND r.[ClinicalConsultationNumber] = @ClinicalConsultationNumber" : string.Empty)}
                ORDER BY r.[CreatedDate] DESC, r.[ClinicalConsultationId]
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";
        }

        public static string GetBeneficiaryClinicalConsultationsCount(BeneficiaryClinicalConsultationSearchCriteria criteria)
        {
            return @$"SELECT 
                       COUNT(r.[ClinicalConsultationId]) Total
                   FROM [ClinicalConsultation].[ClinicalConsultation] r 
                        INNER JOIN [ClinicalConsultation].[ClinicalConsultationBeneficiary] b ON b.[ClinicalConsultationId] = r.[ClinicalConsultationId]
                        INNER JOIN [ClinicalConsultation].[ClinicalConsultationProvider] rp ON rp.[ClinicalConsultationId] = r.[ClinicalConsultationId] AND rp.[ClinicalConsultationProviderTypeId] = {(int)ClinicalConsultationProviderTypes.Requesting}
                        LEFT JOIN [ClinicalConsultation].[ClinicalConsultationProvider] sp ON sp.[ClinicalConsultationId] = r.[ClinicalConsultationId] AND sp.[ClinicalConsultationProviderTypeId] = {(int)ClinicalConsultationProviderTypes.Servicing}
                  WHERE b.[BeneficiaryId] = @BeneficiaryId
                        {(!string.IsNullOrEmpty(criteria.ProviderName) ? " AND (sp.[Name] like @ProviderName OR rp.[Name] like @ProviderName)" : string.Empty)}
                        {(!string.IsNullOrEmpty(criteria.ClinicalConsultationNumber) ? " AND r.[ClinicalConsultationNumber] = @ClinicalConsultationNumber" : string.Empty)}";
        }

        public static string GetClinicalConsultationDetail() 
        {
            return $@"SELECT cc.ClinicalConsultationId 
                            ,cc.ClinicalConsultationNumber
                            ,cc.CreatedBy
                            ,cc.CreatedDate
                            ,cc.ClinicalConsultationDate
                            ,cc.ExpirationDate
                            ,cc.Purpose as Purpose
                            ,cc.[AnyContractedSpecialist]
                            ,cc.AdditionalHealthPlanId
                            ,cc.AdditionalHealthPlanName
                            ,cc.[ServicingProviderSpecialtyId] SpecialtyId
                            ,s.[Name]
                            ,p.[Description]
                            ,p.[Units]
                      FROM [clinicalconsultation].[ClinicalConsultation] cc
                           INNER JOIN [ClinicalConsultation].[ClinicalConsultationProcedureBundle] p ON p.[ClinicalConsultationId] = cc.[ClinicalConsultationId]
                           LEFT JOIN [dbo].[vSpecialty] s ON s.[SpecialtyId] = cc.[ServicingProviderSpecialtyId]
                      WHERE cc.ClinicalConsultationId =  @ClinicalConsultationId ;";
        }
        public static string GetClinicalConsultationDetailProviders()
        {
            return $@"select cp.ClinicalConsultationProviderId ,
                            cp.ClinicalConsultationId ,
                            cp.ProviderAffiliationId ,
                            cp.RenderingProviderId ,
                            cp.BillingProviderId ,
                            cp.RenderingNPI ,
                            cp.BillingNPI ,
                            cp.Name ,
                            cp.FirstName ,
                            cp.MiddleName ,
                            cp.LastName ,
                            cp.AddressId ,
                            cp.FullAddress ,
                            cp.AddressLine1 ,
                            cp.AddressLine2 ,
                            cp.CountyName ,
                            cp.StateName ,
                            cp.ZipCode ,
                            cp.LocationCodeAddress ,
                            cp.LocationAddress ,
                            cp.PhoneId ,
                            cp.PhoneNumber ,
                            cp.FaxId ,
                            cp.FaxNumber ,
                            cp.Email ,
                            cp.IsPPN ,
                            cp.IsBestPractice ,
                            cp.IsPriority ,
                            cp.IsCapitated ,
                            cp.ClinicalConsultationProviderTypeId,
                            ccpt.Name as ProviderType,
                            cps.Name as Specialty
                    FROM [clinicalconsultation].[ClinicalConsultation] cc
                    INNER JOIN [clinicalconsultation].[ClinicalConsultationProvider] cp on cc.ClinicalConsultationId = cp.ClinicalConsultationId
                    LEFT JOIN [clinicalconsultation].[ClinicalConsultationProviderSpecialty] cps on cp.ClinicalConsultationProviderId = cps.ClinicalConsultationProviderId
                    INNER JOIN [clinicalconsultation].ClinicalConsultationProviderType ccpt on cp.ClinicalConsultationProviderTypeId = ccpt.ClinicalConsultationProviderTypeId
                    WHERE  cc.ClinicalConsultationId =  @ClinicalConsultationId ;";
        }

        public static string GetClinicalConsultationDetailDiagnosis()
        {
            return $@"SELECT   ccd.Code
                               ,ccd.[Description]
                               ,ccd.IsPrimary
                          FROM [clinicalconsultation].[ClinicalConsultation] cc
                          INNER JOIN [clinicalconsultation].ClinicalConsultationDiagnosis ccd on cc.ClinicalConsultationId = ccd.ClinicalConsultationId
                          where cc.ClinicalConsultationId =  @ClinicalConsultationId 
                          order by IsPrimary Desc, [ClinicalConsultationDiagnosisId] Desc";
        }

        public static string GetLineOfBusinessFromClinicalConsultationId()
        {
            return
                @$"SELECT ccb.[LineOfBusinessId]
                   FROM [clinicalconsultation].[ClinicalConsultationBeneficiary] ccb
                   WHERE ccb.[ClinicalConsultationId] =  @ClinicalConsultationId";

        }

        public static string InsertViewLog()
        {
            return $@"INSERT INTO [clinicalconsultation].[ViewLog] ([ClinicalConsultationId],[UserId],[ViewedOn])
                        SELECT @ClinicalConsultationId, @UserId, GETDATE()
                        WHERE NOT EXISTS (
                                        SELECT 1 FROM [clinicalconsultation].[ViewLog]
                                         WHERE  ClinicalConsultationId =  @ClinicalConsultationId 
                                         AND UserId = @UserId);";
        }
    }
}
