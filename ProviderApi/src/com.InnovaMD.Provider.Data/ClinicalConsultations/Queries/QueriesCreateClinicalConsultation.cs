using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.Security;
using Hangfire.Annotations;
using Microsoft.PowerBI.Api.Models;
using System;
using static com.InnovaMD.Provider.Data.ClinicalConsultations.Columns;
using static System.Net.Mime.MediaTypeNames;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations.Queries
{
    internal static class QueriesCreateClinicalConsultation
    {
        public static string FindRequestingProviderByAffiliationIds()
        {
            return @$"
                SELECT DISTINCT 
                       p.[ProviderAffiliationId]
                      ,p.[RenderingProviderId]
                      ,p.[RenderingProviderNPI]
                      ,p.[RenderingProviderName]
                      ,p.[BillingProviderId]
                      ,p.[BillingProviderNPI]
                      ,p.[BillingProviderName]
                      ,p.[Email]
                      ,p.PhoneNumber
                      ,CASE WHEN p.IsMSOClinic = 1 THEN p.[ProviderLocationName] ELSE p.[BillingProviderName] END FacilityName
                      ,p.[CityId]
                      ,p.[City] AS [Name]
                      ,p.[SpecialtyId]
                      ,p.[IsPrimarySpecialty]
                      ,p.[SpecialtyName] AS [Name]                      
                 FROM [dbo].[ProviderDirectory] p
                      JOIN [dbo].[ProductNetworkLineOfBusiness] pnl ON pnl.[NetworkId] = p.[NetworkId]
                WHERE pnl.[LineOfBusinessId] = @LineOfBusinessId AND p.[ProviderAffiliationId] IN @ProviderAffiliationIds AND p.[IsPrimaryLocation] = 1
                ORDER BY p.[ProviderAffiliationId], p.[IsPrimarySpecialty] DESC, p.[SpecialtyName]
            ";
        }

        public static string SearchDiagnoses(bool count = false)
        {
            if (count)
            {
                return @$"
                    SELECT COUNT(*)
                    FROM  [diagnosissearch].[vDiagnosis] d
                    WHERE d.[Code] like @Code OR d.[Description] like @Description
                ";
            }

            return @$"
                SELECT 
                      d.[DiagnosisId]
		            , d.[Code]
		            , d.[Description]
                FROM  [diagnosissearch].[vDiagnosis] d
                    WHERE d.[Code] like @Code OR d.[Description] like @Description
                ORDER BY d.[Description], d.[Code]
                OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY
            ";
        }

        internal static string GetRecentDiagnoses()
        {
            return @$"
	            WITH Diagnoses AS (
		            SELECT 
			              d.DiagnosisId
			            , d.Code
			            , d.Description
			            , c.ClinicalConsultationDate
			            , ROW_NUMBER() OVER (PARTITION BY d.DiagnosisId ORDER BY c.ClinicalConsultationDate DESC) AS rn
		            FROM clinicalconsultation.ClinicalConsultationDiagnosis d 
			            INNER JOIN clinicalconsultation.ClinicalConsultation c ON c.ClinicalConsultationId = d.ClinicalConsultationId 
			            INNER JOIN clinicalconsultation.ClinicalConsultationBeneficiary b ON b.ClinicalConsultationId = c.ClinicalConsultationId
                        INNER JOIN [diagnosissearch].[vDiagnosis] vd ON vd.DiagnosisId = d.DiagnosisId -- Validates diagnosis is active
		            WHERE b.BeneficiaryId = @BeneficiaryId  
	            )
	            SELECT 
			          d.DiagnosisId
			        , d.Code
			        , d.Description
	            FROM Diagnoses d
	            WHERE d.rn = 1
	            ORDER BY d.ClinicalConsultationDate DESC, d.Description ASC
                OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY
            ";
        }
        internal static string GetServices()
        {
            return $@"SELECT [ProcedureBundleId]
                            ,[Description]
                            ,[LineOfBusinessId]
                            ,[ServiceRequestTypeId]
                            ,[DefaultUnits]
                            ,[MinimumUnits]
                            ,[MaximumUnits]
                            ,[ReferenceCode]
                            ,[StartDate]
                            ,[EndDate]
                            ,[SortOrder]
                            ,[ServiceTypeCode]
                          FROM [procedurebundle].[vProcedureBundle]
                          WHERE [LineOfBusinessId] = @LineOfBusinessId
                                AND [ServiceRequestTypeId] = {(int)ServicesRequestType.ServiceRequestTypeId}
                          ORDER BY SortOrder asc";
        }

        public static string SearchServicingProvider(ServicingProviderSearchCriteria criteria, bool count = false)
        {
            var query = @$"
                     FROM [dbo].[ProviderDirectory] p 
                             JOIN [dbo].[ProductNetworkLineOfBusiness] pnl ON pnl.[NetworkId] = p.[NetworkId]
                             JOIN [dbo].[ServicingProvider] sp ON sp.ProviderAffiliationId = p.ProviderAffiliationId
                    WHERE p.IsExcludeServicingProvider = 0 AND pnl.[LineOfBusinessId] = @LineOfBusinessId
                        {(!string.IsNullOrEmpty(criteria.ProviderName) ? " AND (p.[RenderingProviderName] LIKE @ProviderName OR (p.[BillingProviderName] LIKE @ProviderName AND p.IsMSOClinic = 0) OR (p.[ProviderLocationName] LIKE @ProviderName AND p.IsMSOClinic = 1))" : string.Empty)}
                        {(!string.IsNullOrEmpty(criteria.NPI) ? " AND (p.[RenderingProviderNPI] LIKE @ProviderNPI OR p.[BillingProviderNPI] LIKE @ProviderNPI)" : string.Empty)}
                        {(criteria.SpecialtyId.HasValue ? "AND p.[SpecialtyId] = @SpecialtyId" : string.Empty)}
                        {(criteria.AdministrationGroupId.HasValue ? " AND p.[AdministrationGroupId] = @AdministrationGroupId" : string.Empty)}
                        {(criteria.CityId.HasValue ? " AND p.[CityId] = @CityId" : string.Empty)}
                        {(criteria.StateId.HasValue ? " AND p.[StateId] = @StateId" : string.Empty)}
                        {(!string.IsNullOrEmpty(criteria.ZipCode) ? " AND p.[ZipCode] = @ZipCode" : string.Empty)}
                        {(criteria.SpecialtyId.HasValue ? " AND p.[SpecialtyId] = @SpecialtyId" : string.Empty)}
            ";

            if (count)
            {
                return @$"
                    SELECT COUNT(DISTINCT p.[ProviderAffiliationId]) {query}
                ";
            }

            return @$"
                DECLARE @Latitude float, 
		                @Longitude float;
                
				SELECT TOP 1 
					@Latitude = AVG(z.[Latitude]), 
					@Longitude = AVG(z.[Longitude])
				FROM [dbo].[vZipCode] z
		        WHERE z.[CountyId] = {(criteria.CityId.HasValue ? "@CityId" : "@BeneficiaryCityId")}
				GROUP BY z.[StateId], z.[CountyId]

                SELECT 
                       p.[ProviderAffiliationId]                                                                                                                                                                                                                                                                                        
                      ,p.[RenderingProviderId]
                      ,p.[RenderingProviderNPI]
                      ,MIN(CASE WHEN p.[IsMSOClinic] = 1 AND sp.[Priority] = 1 THEN p.[ProviderLocationName] ELSE p.[RenderingProviderName] END) [RenderingProviderName]
                      ,p.[BillingProviderId]
                      ,p.[BillingProviderNPI]
                      ,p.[BillingProviderName]
                      ,p.[Email]
                      ,p.[IsMSOClinic]
                      ,p.[IsCapitated]
                      ,p.[IsMSOPPN]
                      ,p.[IsBestPractice]
                      ,sp.[Priority]
                      ,MIN(p.PhoneNumber) as PhoneNumber
                      ,MIN(CASE WHEN p.IsMSOClinic = 1 THEN p.[ProviderLocationName] ELSE p.[BillingProviderName] END) FacilityName 
                      ,MIN(CASE WHEN (p.Latitude IS NULL OR p.Longitude IS NULL OR @Latitude IS NULL OR @Longitude IS NULL) THEN {int.MaxValue} ELSE [dbo].[udf-geo-Calc-Miles] (p.Latitude, p.Longitude, @Latitude, @Longitude) END) As Distance
                {query}
                GROUP BY 
                       p.[ProviderAffiliationId]
                      ,p.[RenderingProviderId]
                      ,p.[RenderingProviderNPI]
                      ,p.[RenderingProviderName]
                      ,p.[BillingProviderId]
                      ,p.[BillingProviderNPI]
                      ,p.[BillingProviderName]
                      ,p.[Email]
                      ,p.[IsMSOClinic]
                      ,p.[IsCapitated]
                      ,p.[IsMSOPPN]
                      ,p.[IsBestPractice]
                      ,sp.[Priority]
                ORDER BY [Distance], sp.[Priority], p.[RenderingProviderName], p.[ProviderAffiliationId]
                OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY
            ";
        }

        public static string GetServicingProvidersLocations()
        {
            return @$"
                SELECT DISTINCT 
                       p.[ProviderAffiliationId]
                      ,p.[CityId]
                      ,p.[City] AS [Name]
                      ,p.[ZipCode]
                FROM  [dbo].[ProviderDirectory] p
                      JOIN [dbo].[ProductNetworkLineOfBusiness] pnl ON pnl.[NetworkId] = p.[NetworkId]
                WHERE pnl.[LineOfBusinessId] = @LineOfBusinessId AND p.ProviderAffiliationId IN @ProviderAffiliationIds
                ORDER BY p.[ProviderAffiliationId], p.[City] 
            ";
        }

        public static string GetServicingNonPPNReason()
        {
            return @$"
                SELECT [ServicingNonPPNReasonId]
                  ,[LineOfBusinessId]
                  ,[Description]
                  FROM [clinicalconsultation].[ServicingNonPPNReason]
                  WHERE LineOfBusinessId = @LineOfBusinessId
            ";
        }

        public static string GetServicingProvidersSpecialties()
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
                ORDER BY p.[ProviderAffiliationId], p.[IsPrimarySpecialty] DESC, p.[SpecialtyName]
            ";
        }

        public static string GetServicingProviderCounties()
        {
            return $@"SELECT [CountyId]
                          ,[StateId]
                          ,[FIPSCode]
                          ,[Name]
                          ,[FIPSClassCode]
                      FROM [dbo].[vCounty]
                      WHERE StateId = @StateId
                    ORDER BY [Name]";
        }

        public static string GetServicingProviderZipCodes()
        {
            return $@"SELECT [ZipCodeId]
                          ,[CountryId]
                          ,[CountyId]
                          ,[StateId]
                          ,[ZipCode] as ZipCodeValue
                          ,[Latitude]
                          ,[Longitude]
                          ,[Region]
                          ,[RegionEnglish]
                          ,[RegionSpanish]
                      FROM [dbo].[vZipCode]
                      WHERE StateId = @StateId 
                      ORDER BY ZipCodeValue";
        }

        public static string GetServicingProviderAdminGroups()
        {
            return $@"SELECT [AdministrationGroupId]
                      ,[AdministrationGroupTypeId]
                      ,[Name]
                      ,[NPI]
                      ,[Number]
                      ,[Code]
                      ,[AdministrationGroupClassificationId]
                      ,[LineOfBusinessId]
                  FROM [dbo].[vAdministrationGroup]
                  WHERE [LineOfBusinessId] =  @LineOfBusinessId AND [AdministrationGroupTypeId] = @AdminGroupTypeId
                  ORDER BY [Name] ";
        }

        public static string GetServicingProviderSpecialties()
        {
            return $@"SELECT 
                           s.[SpecialtyId]
                          ,s.[Name]
                          ,s.[TaxonomyCode]
                          ,ISNULL(acs.[AllowAnyContractedSpecialist], 0) [AllowAnyContractedSpecialist]
                          ,s.[DefaultRoleId]
                          ,s.[ProviderEntityTypeId]
                          ,s.[IsDirectoryDisplay]
                      FROM [dbo].[vSpecialty] s
                        LEFT JOIN [clinicalconsultation].[SpecialtyAnyContractedSpecialist] acs ON acs.SpecialtyId = s.SpecialtyId AND acs.LineOfBusinessId = @LineOfBusinessId
                      ORDER BY s.[Name] ";
        }
        public static string GetServicingProviderStates()
        {
            return $@"SELECT [StateId]
                          ,[CountryId]
                          ,[FIPSCode]
                          ,[USPSCode]
                          ,[Name]
                          ,[GNISId]
                      FROM [dbo].[vState]
                      ORDER BY 
                        CASE WHEN StateId = @StateID THEN 0 ELSE 1 END, Name ";
        }

        public static string GetHealthPlans()
        {
            return
                @$"SELECT [AdditionalHealthPlanId], [AdditionalHealthPlanName]
                FROM [clinicalconsultation].[AdditionalHealthPlan];";

        }

        public static string[] GetBeneficiaryInformationForClinicalConsultation()
        {
            var queries = new string[3];

            queries[0] = $@"
                SELECT 
                       b.[BeneficiaryId]
                      ,b.[CardNumber]
                      ,b.[DisplayName] [Name]
                      ,b.[FirstName]
                      ,b.[MiddleName]
                      ,b.[LastName]
                      ,b.[BirthDate]
                      ,b.[LineOfBusinessId]
                      ,b.[LineOfBusiness] [LineOfBusinessShortName]
                      ,b.[HealthPlanId]
                      ,b.[HealthPlan] [HealthPlanName]
                      ,b.[ProductId]
                      ,b.[ProductName]
                      ,b.[IsReferralRequired]
                      ,b.[Brand]
                      ,b.[Identifier]
                      ,b.[BeneficiaryIdentifierId]
                  FROM [BeneficiaryProfile].[vBeneficiary] b
                 WHERE b.[BeneficiaryId] = @BeneficiaryId
            ";

            queries[1] = $@"
                 SELECT DISTINCT
                       a.[AddressTypeId]
                      ,a.[AddressLine1]
                      ,a.[AddressLine2]
                      ,a.[AddressLine3]
                      ,a.[City]
                      ,a.[USPSCode] [State]
                      ,a.[Place]
                      ,a.[ZIPCode]
                      ,a.[ZIP4Code]
                      ,a.[CountryFIPSCode]
                      ,a.[IsPrimary]
                  FROM [BeneficiaryProfile].[vBeneficiaryAddress] a
                  WHERE a.[BeneficiaryId] = @BeneficiaryId
            ";

            queries[2] = $@"
                SELECT DISTINCT
                       p.[PhoneTypeId]
                      ,p.[CountryCode]
                      ,p.[AreaCode]
                      ,p.[Exchange]
                      ,p.[Number]
                      ,p.[PhoneNumber]
                      ,p.[IsPrimary]
                  FROM [BeneficiaryProfile].[vBeneficiaryPhone] p
                 WHERE p.[BeneficiaryId] = @BeneficiaryId
            ";

            return queries;
        }

        public static string GetRequestingProviderInformationForClinicalConsultation()
        {
            return $@"
                SELECT DISTINCT
                       p.[ProviderAffiliationId]
                      ,p.[RenderingProviderId]
                      ,p.[BillingProviderId]
                      ,p.[RenderingProviderNPI] [RenderingNPI]
                      ,p.[BillingProviderNPI] [BillingNPI]
                      ,p.[RenderingProviderName] [Name]
	                  ,p.[FirstName]
	                  ,p.[MiddleName]
	                  ,p.[LastName]
                      ,p.[BillingProviderName]
	                  ,{(int)ClinicalConsultationProviderTypes.Requesting} ClinicalConsultationProviderTypeId
	                  ,p.[AddressId]
                      ,p.[AddressLine1]
                      ,p.[AddressLine2]
                      ,p.[City] [CountyName]
                      ,p.[State] [StateName]
                      ,p.[ZIPCode]
	                  ,p.[IsPrimaryLocation]
	                  ,p.[PhoneId]
                      ,p.[PhoneNumber]
                      ,p.[FaxId]
                      ,p.[FaxNumber]
                      ,p.[Email]
                      ,p.[IsMSOPPN] [IsPPN]
                      ,p.[IsBestPractice]
                      ,p.[IsPriority]
                      ,p.[IsCapitated]
	                  ,p.[IsMSOClinic]
	                  ,p.[SpecialtyId]
	                  ,p.[specialtyName] [Name]
	                  ,p.[IsPrimarySpecialty]
                      ,s.[ProviderEntityTypeId]
                  FROM [dbo].[ProviderDirectory] p
                       INNER JOIN [dbo].[vSpecialty] s ON s.[SpecialtyId] = p.[SpecialtyId]
	                WHERE	p.[RenderingProviderId] = @RenderingProviderId 
			                AND p.[BillingProviderId] = @BillingProviderId
			                AND p.[CityId] = @CityId
	                ORDER BY p.IsPrimaryLocation desc
              ";
        }

        public static string GetPCPInformationForClinicalConsultation()
        {
            return $@"
                DECLARE @ProviderAffiliationId INT, @LineOfBusinessId INT;

                SELECT TOP 1
                      @ProviderAffiliationId = b.PCPAffiliationId
                    , @LineOfBusinessId = b.LineOfBusinessId  
                  FROM BeneficiaryProfile.vBeneficiary b 
                 WHERE b.BeneficiaryId = @BeneficiaryId;

                SELECT DISTINCT
                       p.[ProviderAffiliationId]
                      ,p.[RenderingProviderId]
                      ,p.[BillingProviderId]
                      ,p.[RenderingProviderNPI] [RenderingNPI]
                      ,p.[BillingProviderNPI] [BillingNPI]
                      ,p.[RenderingProviderName] [Name]
	                  ,p.[FirstName]
	                  ,p.[MiddleName]
	                  ,p.[LastName]
                      ,p.[BillingProviderName]
	                  ,{(int)ClinicalConsultationProviderTypes.PCP} ClinicalConsultationProviderTypeId
	                  ,p.[AddressId]
                      ,p.[AddressLine1]
                      ,p.[AddressLine2]
                      ,p.[City] [CountyName]
                      ,p.[State] [StateName]
                      ,p.[ZIPCode]
	                  ,p.[IsPrimaryLocation]
	                  ,p.[PhoneId]
                      ,p.[PhoneNumber]
                      ,p.[FaxId]
                      ,p.[FaxNumber]
                      ,p.[Email]
                      ,p.[IsMSOPPN] [IsPPN]
                      ,p.[IsBestPractice]
                      ,p.[IsPriority]
                      ,p.[IsCapitated]
	                  ,p.[IsMSOClinic]
                      ,p.[AdministrationGroupId]
                      ,p.[AdministrationGroup] [Name]
	                  ,p.[SpecialtyId]
	                  ,p.[specialtyName] [Name]
	                  ,p.[IsPrimarySpecialty]
                      ,s.[ProviderEntityTypeId]
                  FROM [dbo].[ProviderDirectory] p
                       INNER JOIN [dbo].[vSpecialty] s ON s.[SpecialtyId] = p.[SpecialtyId]
                       INNER JOIN [dbo].[vAdministrationGroup] ag ON ag.[AdministrationGroupId] = p.[AdministrationGroupId]
	             WHERE p.[ProviderAffiliationId] = @ProviderAffiliationId 
                       AND ag.AdministrationGroupTypeId IN ({(int)AdministrationGroupTypes.IPA}, {(int)AdministrationGroupTypes.PMG}) 
                       AND ag.[LineOfBusinessId] = @LineOfBusinessId
	          ORDER BY p.IsPrimaryLocation desc
              ";
        }

        public static string GetServicingProviderInformationForClinicalConsultation()
        {
            return $@"
                SELECT DISTINCT                    
                       p.[ProviderAffiliationId]
                      ,p.[RenderingProviderId]
                      ,p.[BillingProviderId]
                      ,p.[RenderingProviderNPI] [RenderingNPI]
                      ,p.[BillingProviderNPI] [BillingNPI]
                      ,p.[RenderingProviderName] [Name]
	                  ,p.[FirstName]
	                  ,p.[MiddleName]
	                  ,p.[LastName]
                      ,p.[BillingProviderName]
	                  ,{(int)ClinicalConsultationProviderTypes.Servicing} [ClinicalConsultationProviderTypeId]
	                  ,p.[AddressId]
                      ,p.[AddressLine1]
                      ,p.[AddressLine2]
                      ,p.[City] [CountyName]
                      ,p.[State] [StateName]
                      ,p.[ZIPCode]
	                  ,p.[IsPrimaryLocation]
	                  ,p.[PhoneId]
                      ,p.[PhoneNumber]
                      ,p.[FaxId]
                      ,p.[FaxNumber]
                      ,p.[Email]
                      ,p.[IsMSOPPN] [IsPPN]
                      ,p.[IsBestPractice]
                      ,p.[IsPriority]
                      ,p.[IsCapitated]
	                  ,p.[IsMSOClinic]
                      ,sp.[Priority]
                      ,p.[AdministrationGroupId]
                      ,p.[AdministrationGroup] [Name]
	                  ,p.[SpecialtyId]
	                  ,p.[specialtyName] [Name]
	                  ,p.[IsPrimarySpecialty]
                      ,s.[ProviderEntityTypeId]
                  FROM [dbo].[ProviderDirectory] p
                       INNER JOIN [dbo].[ServicingProvider] sp ON sp.ProviderAffiliationId = p.ProviderAffiliationId
                       INNER JOIN [dbo].[vSpecialty] s ON s.[SpecialtyId] = p.[SpecialtyId]
	                WHERE	p.[RenderingProviderId] = @RenderingProviderId 
			                AND p.[BillingProviderId] = @BillingProviderId
			                AND p.[CityId] = @CityId
                            AND p.[SpecialtyId] = @SpecialtyId
	                ORDER BY p.IsPrimaryLocation desc
              ";
        }

        public static string GetServicingNonPPNReasonForClinicalConsultation()
        {
            return $@"
                SELECT 
                   r.[ServicingNonPPNReasonId] [NoPPNReasonId]
                  ,r.[Description] NoPPNReasonDescription
              FROM [clinicalconsultation].[ServicingNonPPNReason] r
                    WHERE r.[ServicingNonPPNReasonId] = @ServicingNonPPNReasonId";
        }

        public static string GetDiagnosesInformationForClinicalConsultation()
        {
            return $@"
                SELECT 
                      d.[DiagnosisId]
		            , d.[Code]
		            , d.[Description]
                FROM  [diagnosissearch].[vDiagnosis] d
                    WHERE d.[DiagnosisId] IN @DiagnosisIds";
        }

        public static string GetProcedureBundleInformationForClinicalConsultation()
        {
            return $@"
                SELECT 
                       [ProcedureBundleId]
                      ,[Description] 
                      ,[ReferenceCode]
                      ,[ServiceTypeCode]
                 FROM [procedurebundle].[vProcedureBundle]
                WHERE [ProcedureBundleId] = @ProcedureBundleId";
        }

        public static string GetSpeciltyInformationForClinicalConsultation()
        {
            return $@"
                SELECT 
                     s.[SpecialtyId]
                    ,s.[Name]
                    ,s.[ProviderEntityTypeId]
                    ,ISNULL(acs.[AllowAnyContractedSpecialist], 0) [AllowAnyContractedSpecialist]
                FROM [dbo].[vSpecialty] s
                LEFT JOIN [clinicalconsultation].[SpecialtyAnyContractedSpecialist] acs ON acs.SpecialtyId = s.SpecialtyId AND acs.LineOfBusinessId = @LineOfBusinessId
               WHERE s.[SpecialtyId] = @SpecialtyId";
        }

        public static string GetAdditionalHealthPlanInformationForClinicalConsultation()
        {
            return $@"
                SELECT 
                   [AdditionalHealthPlanId]
                  ,[AdditionalHealthPlanName]
              FROM [clinicalconsultation].[AdditionalHealthPlan]
             WHERE AdditionalHealthPlanId = @AdditionalHealthPlanId";
        }

        public static string GetClinicalConsultationSequenceNumber()
        {
            return "SELECT NEXT VALUE FOR [dbo].[ClinicalConsultationNumber]";
        }

        public static string GetClinicalReferralSequenceNumber()
        {
            return "SELECT NEXT VALUE FOR [dbo].[ClinicalReferralNumber]";
        }

        public static string InsertClinicalConsultation()
        {
            return $@"
             INSERT INTO [clinicalconsultation].[ClinicalConsultation]
                   ([ClinicalConsultationNumber]
                   ,[AdditionalHealthPlanId]
                   ,[AdditionalHealthPlanName]
                   ,[ClinicalConsultationDate]
                   ,[Purpose]
                   ,[IsConsultation]
                   ,[CreatedUserId]
                   ,[CreatedBy]
                   ,[ExpirationDate]
                   ,[EffectiveDate]
                   ,[AnyContractedSpecialist]
                   ,[ServicingProviderSpecialtyId]
                   ,[SourceIdentifier]
                   ,[IsRecreate]
                   ,[RecreatedSourceId])
             VALUES
                   (@ClinicalConsultationNumber
                   ,@AdditionalHealthPlanId
                   ,@AdditionalHealthPlanName
                   ,@ClinicalConsultationDate
                   ,@Purpose
                   ,@IsConsultation
                   ,@CreatedUserId
                   ,@CreatedBy
                   ,@ExpirationDate
                   ,@EffectiveDate
                   ,@AnyContractedSpecialist
                   ,@ServicingProviderSpecialtyId
                   ,@SourceIdentifier
                   ,@IsRecreate
                   ,@RecreatedSourceId);

             SELECT CAST(SCOPE_IDENTITY() as BIGINT);
            ";
        }

        public static string InsertClinicalConsultationBeneficiary()
        {
            return $@"
                INSERT INTO [clinicalconsultation].[ClinicalConsultationBeneficiary]
                    ([ClinicalConsultationId]
                    ,[BeneficiaryId]
                    ,[CardNumber]
                    ,[Name]
                    ,[FirstName]
                    ,[MiddleName]
                    ,[LastName]
                    ,[BirthDate]
                    ,[LineOfBusinessId]
                    ,[LineOfBusinessShortName]
                    ,[HealthPlanId]
                    ,[HealthPlanName]
                    ,[ProductId]
                    ,[ProductName]
                    ,[Brand]
                    ,[Identifier]
                    ,[BeneficiaryIdentifierId])
                VALUES
                    (@ClinicalConsultationId
                    ,@BeneficiaryId
                    ,@CardNumber
                    ,@Name
                    ,@FirstName
                    ,@MiddleName
                    ,@LastName
                    ,@BirthDate
                    ,@LineOfBusinessId
                    ,@LineOfBusinessShortName
                    ,@HealthPlanId
                    ,@HealthPlanName
                    ,@ProductId
                    ,@ProductName
                    ,@Brand
                    ,@Identifier
                    ,@BeneficiaryIdentifierId);

                SELECT CAST(SCOPE_IDENTITY() as BIGINT);
            ";
        }

        public static string InsertClinicalConsultationBeneficiaryAddress()
        {
            return $@"
                INSERT INTO [clinicalconsultation].[ClinicalConsultationBeneficiaryAddress]
                       ([ClinicalConsultationBeneficiaryId]
                       ,[AddressTypeId]
                       ,[AddressLine1]
                       ,[AddressLine2]
                       ,[AddressLine3]
                       ,[City]
                       ,[State]
                       ,[Place]
                       ,[ZIPCode]
                       ,[ZIP4Code]
                       ,[CountryFIPSCode]
                       ,[IsPrimary])
                 VALUES
                       (@ClinicalConsultationBeneficiaryId
                       ,@AddressTypeId
                       ,@AddressLine1
                       ,@AddressLine2
                       ,@AddressLine3
                       ,@City
                       ,@State
                       ,@Place
                       ,@ZIPCode
                       ,@ZIP4Code
                       ,@CountryFIPSCode
                       ,@IsPrimary);
            ";
        }

        public static string InsertClinicalConsultationBeneficiaryPhone()
        {
            return $@"
                INSERT INTO [clinicalconsultation].[ClinicalConsultationBeneficiaryPhone]
                           ([ClinicalConsultationBeneficiaryId]
                           ,[PhoneTypeId]
                           ,[CountryCode]
                           ,[AreaCode]
                           ,[Exchange]
                           ,[Number]
                           ,[PhoneNumber]
                           ,[IsPrimary])
                     VALUES
                           (@ClinicalConsultationBeneficiaryId
                           ,@PhoneTypeId
                           ,@CountryCode
                           ,@AreaCode
                           ,@Exchange
                           ,@Number
                           ,@PhoneNumber
                           ,@IsPrimary);
            ";
        }

        public static string InsertClinicalConsultationDiagnosis()
        {
            return $@"
                INSERT INTO [clinicalconsultation].[ClinicalConsultationDiagnosis]
                       ([ClinicalConsultationId]
                       ,[DiagnosisId]
                       ,[Code]
                       ,[Description]
                       ,[IsPrimary] )
                 VALUES
                       (@ClinicalConsultationId
                       ,@DiagnosisId
                       ,@Code
                       ,@Description
                       ,@IsPrimary);
            ";
        }

        public static string InsertClinicalConsultationProcedureBundle()
        {
            return $@"
                INSERT INTO [clinicalconsultation].[ClinicalConsultationProcedureBundle]
                       ([ClinicalConsultationId]
                       ,[ProcedureBundleId]
                       ,[Description]
                       ,[Units]
                       ,[ReferenceCode]
                       ,[ServiceTypeCode])
                 VALUES
                       (@ClinicalConsultationId
                       ,@ProcedureBundleId
                       ,@Description
                       ,@Units
                       ,@ReferenceCode
                       ,@ServiceTypeCode);
            ";
        }

        public static string InsertClinicalConsultationProvider()
        {
            return $@"
                INSERT INTO [clinicalconsultation].[ClinicalConsultationProvider]
                       ([ClinicalConsultationId]
                       ,[ProviderAffiliationId]
                       ,[RenderingProviderId]
                       ,[BillingProviderId]
                       ,[RenderingNPI]
                       ,[BillingNPI]
                       ,[Name]
                       ,[FirstName]
                       ,[MiddleName]
                       ,[LastName]
                       ,[ClinicalConsultationProviderTypeId]
                       ,[AddressId]
                       ,[FullAddress]
                       ,[AddressLine1]
                       ,[AddressLine2]
                       ,[CountyName]
                       ,[StateName]
                       ,[ZipCode]
                       ,[LocationCodeAddress]
                       ,[LocationAddress]
                       ,[PhoneId]
                       ,[PhoneNumber]
                       ,[FaxId]
                       ,[FaxNumber]
                       ,[Email]
                       ,[IsPPN]
                       ,[ClinicalConsultationProviderClassificationId]
                       ,[IsBestPractice]
                       ,[IsPriority]
                       ,[IsCapitated]
                       ,[AdministrationGroupId]
                       ,[AdministrationGroupName])
                 VALUES
                       (@ClinicalConsultationId
                       ,@ProviderAffiliationId
                       ,@RenderingProviderId
                       ,@BillingProviderId
                       ,@RenderingNPI
                       ,@BillingNPI
                       ,@Name
                       ,@FirstName
                       ,@MiddleName
                       ,@LastName
                       ,@ClinicalConsultationProviderTypeId
                       ,@AddressId
                       ,@FullAddress
                       ,@AddressLine1
                       ,@AddressLine2
                       ,@CountyName
                       ,@StateName
                       ,@ZipCode
                       ,@LocationCodeAddress
                       ,@LocationAddress
                       ,@PhoneId
                       ,@PhoneNumber
                       ,@FaxId
                       ,@FaxNumber
                       ,@Email
                       ,@IsPPN
                       ,@ClinicalConsultationProviderClassificationId
                       ,@IsBestPractice
                       ,@IsPriority
                       ,@IsCapitated
                       ,@AdministrationGroupId
                       ,@AdministrationGroupName);

                    SELECT CAST(SCOPE_IDENTITY() as BIGINT);
            ";
        }

        public static string InsertClinicalConsultationProviderSpecialty()
        {
            return $@"
                INSERT INTO [clinicalconsultation].[ClinicalConsultationProviderSpecialty]
                           ([ClinicalConsultationProviderId]
                           ,[SpecialtyId]
                           ,[Name] 
                           ,[ProviderEntityTypeId])
                     VALUES
                           (@ClinicalConsultationProviderId
                           ,@SpecialtyId
                           ,@Name 
                           ,@ProviderEntityTypeId);
            ";
        }

        public static string InsertClinicalConsultationServicingNonPPNReason()
        {
            return $@"
                INSERT INTO [clinicalconsultation].[ClinicalConsultationServicingNonPPNReason]
                       ([ClinicalConsultationId]
                       ,[NoPPNReasonId]
                       ,[NoPPNReasonDescription])
                 VALUES
                       (@ClinicalConsultationId
                       ,@NoPPNReasonId
                       ,@NoPPNReasonDescription)
            ";
        }

        internal static string[] GetClinicalConsutationInformationForRecreate()
        {
            var queries = new string[5];

            queries[0] = $@"
                SELECT
                      cc.ClinicalConsultationId
                    , cc.ClinicalConsultationNumber
	                , cc.ClinicalConsultationDate
                    , cc.Purpose
	                , cc.IsConsultation
                    , cc.AnyContractedSpecialist
	                , s.SpecialtyId
	                , s.Name
                    , s.AllowAnyContractedSpecialist
                    , cc.AdditionalHealthPlanId
                    , cc.AdditionalHealthPlanName
	                , ccppn.NoPPNReasonId ServicingNonPPNReasonId
	                , ccppn.NoPPNReasonDescription Description
                FROM clinicalconsultation.ClinicalConsultation cc
                    JOIN dbo.vSpecialty s ON s.SpecialtyId = cc.ServicingProviderSpecialtyId
                    LEFT JOIN clinicalconsultation.ClinicalConsultationServicingNonPPNReason ccppn ON ccppn.ClinicalConsultationId = cc.ClinicalConsultationId
                WHERE cc.ClinicalConsultationId = @ClinicalConsultationId
            ";

            queries[1] = $@"
                SELECT 
                      DiagnosisId
                    , Code
	                , Description
	                , IsPrimary
                FROM clinicalconsultation.ClinicalConsultationDiagnosis
               WHERE ClinicalConsultationId = @ClinicalConsultationId
            ";

            queries[2] = $@"
                SELECT
	                  ccpb.ProcedureBundleId
	                , ccpb.Units
	                , pb.Description
	                , pb.LineOfBusinessId
	                , pb.DefaultUnits
	                , pb.MinimumUnits
	                , pb.MaximumUnits
	                , pb.ReferenceCode
	                , pb.SortOrder
	                , pb.ServiceTypeCode
                FROM clinicalconsultation.ClinicalConsultationProcedureBundle ccpb
                INNER JOIN procedurebundle.vProcedureBundle pb ON pb.ProcedureBundleId = ccpb.ProcedureBundleId
                WHERE ccpb.ClinicalConsultationId = @ClinicalConsultationId
            ";

            queries[3] = $@"
                SELECT DISTINCT
	                  ccp.ProviderAffiliationId
	                , pd.RenderingProviderId
	                , pd.RenderingProviderNPI
	                , pd.RenderingProviderName
	                , pd.BillingProviderId
	                , pd.BillingProviderNPI
	                , pd.BillingProviderName 
	                , CASE WHEN pd.IsMSOClinic = 1 THEN pd.ProviderLocationName ELSE pd.BillingProviderName END FacilityName
                    , pd.PhoneNumber
                    , pd.Email
                    , pd.CityId 
	                , pd.City Name
	                , pd.ZipCode
                    , pd.SpecialtyId
                    , pd.SpecialtyName Name
                    , pd.IsPrimarySpecialty
	                , pd_a.CityId 
	                , pd_a.City Name
	                , pd_a.ZipCode
                FROM clinicalconsultation.ClinicalConsultationProvider ccp
                  JOIN dbo.ProviderDirectory pd ON pd.ProviderAffiliationId = ccp.ProviderAffiliationId
                  JOIN dbo.ProductNetworkLineOfBusiness pnlob ON pnlob.NetworkId = pd.NetworkId AND pnlob.LineOfBusinessId = @LineOfBusinessId 
                  LEFT JOIN dbo.ProviderDirectory pd_a ON pd_a.ProviderAffiliationId = ccp.ProviderAffiliationId AND pd_a.AddressId = ccp.AddressId
                WHERE ccp.ClinicalConsultationId = @ClinicalConsultationId AND ccp.ClinicalConsultationProviderTypeId = {(int)ClinicalConsultationProviderTypes.Requesting}
            ";

            queries[4] = $@"
                SELECT DISTINCT
                      ccp.ProviderAffiliationId
                    , pd.RenderingProviderId
                    , pd.RenderingProviderNPI
                    , CASE WHEN pd.IsMSOClinic = 1 AND sp.Priority = 1 THEN pd.ProviderLocationName ELSE pd.RenderingProviderName END RenderingProviderName
                    , pd.BillingProviderId
                    , pd.BillingProviderNPI
                    , pd.BillingProviderName 
                    , CASE WHEN pd.IsMSOClinic = 1 THEN pd.ProviderLocationName ELSE pd.BillingProviderName END FacilityName
                    , pd.PhoneNumber
                    , pd.Email
                    , pd.IsMSOPPN
                    , pd.IsBestPractice
                    , pd.IsMSOClinic
                    , pd.IsCapitated
                    , sp.Priority
                    , CASE WHEN sp.Priority = {(int)ServicingProviderPriority.Other} THEN 0 ELSE 1 END PreferredNetwork
                    , pd.CityId 
                    , pd.City Name
                    , pd.ZipCode
                    , pd.SpecialtyId
                    , pd.SpecialtyName Name
                    , pd.IsPrimarySpecialty
	                , pd_a.CityId 
	                , pd_a.City Name
	                , pd_a.ZipCode
                FROM clinicalconsultation.ClinicalConsultationProvider ccp
                  JOIN dbo.ProviderDirectory pd ON pd.ProviderAffiliationId = ccp.ProviderAffiliationId
                  JOIN dbo.ProductNetworkLineOfBusiness pnlob ON pnlob.NetworkId = pd.NetworkId AND pnlob.LineOfBusinessId = @LineOfBusinessId 
                  JOIN dbo.ServicingProvider sp ON sp.ProviderAffiliationId = ccp.ProviderAffiliationId
                  LEFT JOIN dbo.ProviderDirectory pd_a ON pd_a.ProviderAffiliationId = ccp.ProviderAffiliationId AND pd_a.AddressId = ccp.AddressId
                WHERE ccp.ClinicalConsultationId = @ClinicalConsultationId AND ccp.ClinicalConsultationProviderTypeId = {(int)ClinicalConsultationProviderTypes.Servicing}
            ";


            return queries;
        }

        public static string FindConsultationBeneficiaryId()
        {
            return @$"
               SELECT ccb.BeneficiaryId                  
                 FROM clinicalconsultation.ClinicalConsultationBeneficiary ccb
                WHERE ccb.ClinicalConsultationId = @ClinicalConsultationId
            ";
        }

        public static string GetRecentSuggestionsBaseData()
        {
            return @$"WITH Suggestion AS (
                            SELECT  ccb.[ClinicalConsultationId]   
	                             ,cc.ClinicalConsultationNumber
	                              ,Purpose
	                              ,cp.Name as 'ProviderName'                 	  
	                              ,ccd.Code +' ' +ccd.Description 'Diagnosis'
	                              ,cc.ClinicalConsultationDate
	                              ,s.SpecialtyId
	                              ,s.Name as 'Specialty'	  
	                              ,cc.CreatedDate
                                  ,cc.AnyContractedSpecialist
	                              , ROW_NUMBER() OVER (PARTITION BY s.SpecialtyId  ORDER BY cc.CreatedDate DESC) AS rn
                              FROM [clinicalconsultation].[ClinicalConsultationBeneficiary]ccb  
                              join  [clinicalconsultation].[ClinicalConsultation]cc on(ccb.ClinicalConsultationId=cc.ClinicalConsultationId)
                              join [clinicalconsultation].ClinicalConsultationDiagnosis ccd on (ccd.ClinicalConsultationId=cc.ClinicalConsultationId)
                              join [dbo].vSpecialty s on (cc.ServicingProviderSpecialtyId=s.SpecialtyId)
                              left join clinicalconsultation.ClinicalConsultationProvider cp on(cp.ClinicalConsultationId=cc.ClinicalConsultationId and cp.ClinicalConsultationProviderTypeId = {(int)ClinicalConsultationProviderTypes.Servicing})

                              where ccb.BeneficiaryId=@BeneficiaryId and ccd.IsPrimary = 1 
                              group by ccb.ClinicalConsultationId,ccb.BeneficiaryId,cc.Purpose,ccb.Name,cc.CreatedDate,
                              ccb.LineOfBusinessId,cc.ClinicalConsultationDate,ccd.Description,s.SpecialtyId,s.Name,ccd.DiagnosisId,cp.Name,cc.ClinicalConsultationNumber,ccd.Code,cc.AnyContractedSpecialist

                            )
                                        select s.ClinicalConsultationId
                                        ,s.ClinicalConsultationNumber
                                        ,s.Purpose
                                        ,s.ProviderName
                                        ,s.Diagnosis
                                        ,s.ClinicalConsultationDate
                                        ,s.SpecialtyId
                                        ,s.Specialty
                                        ,s.CreatedDate
                                         ,s.AnyContractedSpecialist
                            from Suggestion s
                            WHERE s.rn = 1
                            ORDER BY s.ClinicalConsultationDate Desc
                            OFFSET @Offset ROWS FETCH NEXT @Fetch ROWS ONLY";
        }
    }
}
