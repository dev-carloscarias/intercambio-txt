using com.InnovaMD.Provider.Data.ClinicalConsultations.Queries;
using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Data.Common;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Filters;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.Models.Common;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Data.SqlClient;
using Pipelines.Sockets.Unofficial.Arenas;
using System;
using System.Collections.Generic;
using System.Linq;
using static com.InnovaMD.Provider.Data.ClinicalConsultations.Columns;
using ProviderSpecialty = com.InnovaMD.Provider.Models.ClinicalConsultations.ProviderSpecialty;
using com.InnovaMD.Utilities.Dates;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations
{
    public class CreateClinicalConsultationRepository : RepositoryBase, ICreateClinicalConsultationRepository
    {
        public CreateClinicalConsultationRepository(ConnectionStringOptions connectionStringOptions) : base(connectionStringOptions)
        {
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

        public RequestingProvider FindRequestingProvider(int lineOfBusinessId, params int[] providerAffiliationIds)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    RequestingProvider provider = null;
                    List<City> cities = null;
                    List<ProviderSpecialty> specialties = null;

                    dao.Find<RequestingProvider, City, ProviderSpecialty, RequestingProvider>(
                        QueriesCreateClinicalConsultation.FindRequestingProviderByAffiliationIds(),
                        (requesting, city, specialty) =>
                        {
                            if (provider == null)
                            {
                                cities = [];
                                specialties = [];

                                provider = requesting;
                                provider.Cities = cities;
                                provider.Specialties = specialties;
                            }

                            if (requesting.ProviderAffiliationId == provider.ProviderAffiliationId)
                            {
                                if (!cities.Any((c) => { return c.CityId == city.CityId && c.ZipCode == city.ZipCode; }))
                                {
                                    cities.Add(city);
                                }

                                if (!specialties.Any((s) => { return s.SpecialtyId == specialty.SpecialtyId; }))
                                {
                                    specialties.Add(specialty);
                                }
                            }
                            return requesting;
                        },
                        parameters: new
                        {
                            ProviderAffiliationIds = providerAffiliationIds,
                            LineOfBusinessId = lineOfBusinessId,
                        },
                        splitOn: "ProviderAffiliationId,CityId,SpecialtyId");

                    provider.Cities = provider.Cities?.OrderBy(c => c.Name).ToList();

                    return provider;
                }
            }
        }

        public (int total, IEnumerable<Diagnosis> diagnoses) SearchDiagnosis(DiagnosisSearchCriteria criteria)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        Code = !string.IsNullOrEmpty(criteria.Code) ? new Dapper.DbString() { Value = $"%{criteria.Code}%", IsAnsi = true, Length = criteria.Code.Length + 2 } : null,
                        Description = !string.IsNullOrEmpty(criteria.Description) ? new Dapper.DbString() { Value = $"%{criteria.Description}%", IsAnsi = true, Length = criteria.Description.Length + 2 } : null,
                        Offset = criteria.Offset,
                        Fetch = criteria.Fetch
                    };

                    var count = dao.ExecuteScalar<int>(QueriesCreateClinicalConsultation.SearchDiagnoses(true), parameters);
                    var diagnosis = new List<Diagnosis>();

                    if (count > 0)
                    {
                        diagnosis = dao.Find<Diagnosis>(
                            QueriesCreateClinicalConsultation.SearchDiagnoses(),
                            parameters: parameters).ToList();
                    }

                    return (count, diagnosis);
                }
            }
        }

        public IEnumerable<Diagnosis> GetRecentDiagnoses(int beneficiaryId, int maxAmount)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        BeneficiaryId = beneficiaryId,
                        Offset = 0,
                        Fetch = maxAmount
                    };

                    var diagnosis = dao.Find<Diagnosis>(
                                               QueriesCreateClinicalConsultation.GetRecentDiagnoses(),
                                               parameters: parameters).ToList();

                    return diagnosis;
                }
            }

        }

        public IEnumerable<ProcedureBundle> GetServices(int lineOfBusinessId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        LineOfBusinessId = lineOfBusinessId,
                    };
                    var services = dao.Find<ProcedureBundle>(QueriesCreateClinicalConsultation.GetServices(), parameters);

                    return services;
                }
            }
        }

        public (IEnumerable<ServicingProvider>, int?) SearchServicingProvider(ServicingProviderSearchCriteria criteria)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        ProviderNPI = !string.IsNullOrEmpty(criteria.NPI) ? new Dapper.DbString() { Value = $"{criteria.NPI}%", IsAnsi = true, Length = criteria.NPI.Length + 1 } : null,
                        ProviderName = !string.IsNullOrEmpty(criteria.ProviderName) ? new Dapper.DbString() { Value = $"%{criteria.ProviderName}%", IsAnsi = true, Length = criteria.ProviderName.Length + 2 } : null,
                        BeneficiaryId = criteria.BeneficiaryId,
                        LineOfBusinessId = criteria.LineOfBusinessId,
                        AdministrationGroupId = criteria.AdministrationGroupId,
                        StateId = criteria.StateId,
                        CityId = criteria.CityId,
                        ZipCode = !string.IsNullOrEmpty(criteria.ZipCode) ? new Dapper.DbString() { Value = criteria.ZipCode, IsAnsi = true, Length = criteria.ZipCode.Length } : null,
                        SpecialtyId = criteria.SpecialtyId,
                        BeneficiaryCityId = criteria.BeneficiaryCityId,
                        Offset = criteria.Offset,
                        Fetch = criteria.Fetch
                    };

                    var isLoadMore = criteria.Page > 1;

                    int? count = !isLoadMore ? dao.ExecuteScalar<int>(QueriesCreateClinicalConsultation.SearchServicingProvider(criteria, true), parameters) : null;
                    var providers = new List<ServicingProvider>();

                    if (isLoadMore || count > 0)
                    {
                        providers = dao.Find<ServicingProvider>(
                            QueriesCreateClinicalConsultation.SearchServicingProvider(criteria),
                            parameters: parameters).ToList();

                        if (providers != null && providers.Count > 0)
                        {
                            var reader = dao.FindMultiple(new string[] { QueriesCreateClinicalConsultation.GetServicingProvidersLocations(), QueriesCreateClinicalConsultation.GetServicingProvidersSpecialties() }, parameters: new
                            {
                                LineOfBusinessId = criteria.LineOfBusinessId,
                                ProviderAffiliationIds = providers.Select((p) => { return p.ProviderAffiliationId; })
                            });

                            if (reader != null)
                            {
                                var cities = reader.Read<City>();
                                var specialties = reader.Read<ProviderSpecialty>();

                                foreach (var p in providers)
                                {
                                    p.Cities = cities.Where(c => { return c.ProviderAffiliationId == p.ProviderAffiliationId; }).ToList();
                                    p.Specialties = specialties.Where(s => { return s.ProviderAffiliationId == p.ProviderAffiliationId; }).ToList();
                                    p.PreferredNetwork = (p.Priority == (int)ServicingProviderPriority.Other) ? false : true;
                                }
                            }
                        }
                    }

                    return (providers, count);
                }
            }
        }

        public ServicingProviderFiltersResponse GetServicingProviderFiltersDefaults(ServicingProviderSearchCriteria filter, int adminGroupTypeId)
        {
            var response = new ServicingProviderFiltersResponse();
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        LineOfBusinessId = filter.LineOfBusinessId,
                        StateId = filter.StateId,
                        AdminGroupTypeId = adminGroupTypeId
                    };

                    var reader = dao.FindMultiple(new string[] { QueriesCreateClinicalConsultation.GetServicingProviderStates(),
                                                                 QueriesCreateClinicalConsultation.GetServicingProviderZipCodes(),
                                                                 QueriesCreateClinicalConsultation.GetServicingProviderAdminGroups(),
                                                                 QueriesCreateClinicalConsultation.GetServicingProviderCounties(),
                                                                 QueriesCreateClinicalConsultation.GetServicingProviderSpecialties()}, parameters);

                    if (reader != null)
                    {
                        response.State = reader.Read<State>().AsEnumerable();
                        response.ZipCode = reader.Read<ZipCode>().AsEnumerable();
                        response.AdministrationGroup = reader.Read<AdministrationGroup>().AsEnumerable();
                        response.County = reader.Read<County>().AsEnumerable();
                        response.Specialty = reader.Read<Models.ClinicalConsultations.Filters.Specialty>().AsEnumerable();

                    }
                }
            }
            return response;
        }

        public ServicingProviderFiltersResponse UpdateServicingProviderFilters(ServicingProviderSearchCriteria filter)
        {
            var response = new ServicingProviderFiltersResponse();
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        LineOfBusinessId = filter.LineOfBusinessId,
                        StateId = filter.StateId,
                    };

                    var reader = dao.FindMultiple(new string[] { QueriesCreateClinicalConsultation.GetServicingProviderStates(),
                                                                 QueriesCreateClinicalConsultation.GetServicingProviderZipCodes(),
                                                                 QueriesCreateClinicalConsultation.GetServicingProviderCounties()}, parameters);

                    if (reader != null)
                    {
                        response.State = reader.Read<State>().AsEnumerable();
                        response.ZipCode = reader.Read<ZipCode>().AsEnumerable();
                        response.County = reader.Read<County>().AsEnumerable();
                    }
                }
            }
            return response;
        }

        public ServicingNonPPNReasonResponse GetServicingNonPPNReason(int lineOfBusinessid)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var reasons = new List<ServicingNonPPNReason>();
                    reasons = dao.Find<ServicingNonPPNReason>(
                        QueriesCreateClinicalConsultation.GetServicingNonPPNReason(),
                        parameters: new
                        {
                            LineOfBusinessId = lineOfBusinessid,
                        }).ToList();

                    return new ServicingNonPPNReasonResponse
                    {
                        ServicingNonPPNReasons = reasons
                    };
                }
            }
        }

        public Models.ClinicalConsultations.ClinicalConsultationBeneficiary GetBeneficiaryInformationForClinicalConsultation(int beneficiaryId)
        {
            Models.ClinicalConsultations.ClinicalConsultationBeneficiary beneficiary = null;
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        BeneficiaryId = beneficiaryId
                    };

                    var reader = dao.FindMultiple(QueriesCreateClinicalConsultation.GetBeneficiaryInformationForClinicalConsultation(), parameters);

                    if (reader != null)
                    {
                        beneficiary = reader.Read<Models.ClinicalConsultations.ClinicalConsultationBeneficiary>().FirstOrDefault();
                        if (beneficiary != null)
                        {
                            beneficiary.Addresses = reader.Read<Models.ClinicalConsultations.ClinicalConsultationBeneficiaryAddress>().AsEnumerable();
                            beneficiary.Phones = reader.Read<Models.ClinicalConsultations.ClinicalConsultationBeneficiaryPhone>().AsEnumerable();
                        }
                    }
                    return beneficiary;
                }
            }
        }

        public Models.ClinicalConsultations.ClinicalConsultationProvider GetPCPInformationForClinicalConsultation(int beneficiaryId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    Models.ClinicalConsultations.ClinicalConsultationProvider provider = null;
                    List<Models.ClinicalConsultations.ClinicalConsultationProviderSpecialty> specialties = null;
                    List<AdministrationGroup> administrationGroups = null;

                    dao.Find<Models.ClinicalConsultations.ClinicalConsultationProvider, AdministrationGroup, Models.ClinicalConsultations.ClinicalConsultationProviderSpecialty, Models.ClinicalConsultations.ClinicalConsultationProvider>(
                        QueriesCreateClinicalConsultation.GetPCPInformationForClinicalConsultation(),
                        (pcp, adminGroup, specialty) =>
                        {
                            if (provider == null)
                            {
                                specialties = [];
                                administrationGroups = [];

                                provider = pcp;
                                provider.Specialties = specialties;
                                provider.AdministrationGroups = administrationGroups;
                            }

                            if (!specialties.Any((s) => { return s.SpecialtyId == specialty.SpecialtyId; }))
                            {
                                specialties.Add(specialty);
                            }

                            if (!administrationGroups.Any((a) => { return a.AdministrationGroupId == adminGroup.AdministrationGroupId; }))
                            {
                                administrationGroups.Add(adminGroup);
                            }

                            return pcp;
                        },
                        parameters: new
                        {
                            BeneficiaryId = beneficiaryId
                        },
                        splitOn: "ProviderAffiliationId,AdministrationGroupId,SpecialtyId");


                    return provider;
                }
            }
        }

        public Models.ClinicalConsultations.ClinicalConsultationProvider GetRequestingProviderInformationForClinicalConsultation(int renderingProviderId, int billingProviderId, int cityId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    Models.ClinicalConsultations.ClinicalConsultationProvider provider = null;
                    List<Models.ClinicalConsultations.ClinicalConsultationProviderSpecialty> specialties = null;

                    dao.Find<Models.ClinicalConsultations.ClinicalConsultationProvider, Models.ClinicalConsultations.ClinicalConsultationProviderSpecialty, Models.ClinicalConsultations.ClinicalConsultationProvider>(
                        QueriesCreateClinicalConsultation.GetRequestingProviderInformationForClinicalConsultation(),
                        (requesting, specialty) =>
                        {
                            if (provider == null)
                            {
                                specialties = [];

                                provider = requesting;
                                provider.Specialties = specialties;
                            }

                            if (!specialties.Any((s) => { return s.SpecialtyId == specialty.SpecialtyId; }))
                            {
                                specialties.Add(specialty);
                            }

                            return requesting;
                        },
                        parameters: new
                        {
                            RenderingProviderId = renderingProviderId,
                            BillingProviderId = billingProviderId,
                            CityId = cityId,
                        },
                        splitOn: "ProviderAffiliationId,SpecialtyId");


                    return provider;
                }
            }
        }

        public IEnumerable<Models.ClinicalConsultations.ClinicalConsultationDiagnosis> GetDiagnosesInformationForClinicalConsultation(IEnumerable<int> diagnosisIds)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        DiagnosisIds = diagnosisIds
                    };

                    var diagnoses = dao.Find<Models.ClinicalConsultations.ClinicalConsultationDiagnosis>(
                            QueriesCreateClinicalConsultation.GetDiagnosesInformationForClinicalConsultation(),
                            parameters: parameters).ToList();

                    return diagnoses;
                }
            }
        }

        public Models.ClinicalConsultations.ClinicalConsultationProcedureBundle GetProcedureBundleInformationForClinicalConsultation(int procedureBundleId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        ProcedureBundleId = procedureBundleId
                    };

                    var procedureBundle = dao.Find<Models.ClinicalConsultations.ClinicalConsultationProcedureBundle>(
                            QueriesCreateClinicalConsultation.GetProcedureBundleInformationForClinicalConsultation(),
                            parameters: parameters).FirstOrDefault();

                    return procedureBundle;
                }
            }
        }

        public Models.ClinicalConsultations.ClinicalConsultationProvider GetServicingProviderInformationForClinicalConsultation(int renderingProviderId, int billingProviderId, int cityId, int specialtyId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    Models.ClinicalConsultations.ClinicalConsultationProvider provider = null;
                    List<Models.ClinicalConsultations.ClinicalConsultationProviderSpecialty> specialties = null;
                    List<AdministrationGroup> administrationGroups = null;

                    dao.Find<Models.ClinicalConsultations.ClinicalConsultationProvider, AdministrationGroup, Models.ClinicalConsultations.ClinicalConsultationProviderSpecialty, Models.ClinicalConsultations.ClinicalConsultationProvider>(
                        QueriesCreateClinicalConsultation.GetServicingProviderInformationForClinicalConsultation(),
                        (servicing, adminGroup, specialty) =>
                        {
                            if (provider == null)
                            {
                                specialties = [];
                                administrationGroups = [];

                                provider = servicing;
                                provider.Specialties = specialties;
                                provider.AdministrationGroups = administrationGroups;
                            }

                            if (!specialties.Any((s) => { return s.SpecialtyId == specialty.SpecialtyId; }))
                            {
                                specialties.Add(specialty);
                            }

                            if (!administrationGroups.Any((a) => { return a.AdministrationGroupId == adminGroup.AdministrationGroupId; }))
                            {
                                administrationGroups.Add(adminGroup);
                            }

                            return servicing;
                        },
                        parameters: new
                        {
                            RenderingProviderId = renderingProviderId,
                            BillingProviderId = billingProviderId,
                            CityId = cityId,
                            SpecialtyId = specialtyId
                        },
                        splitOn: "ProviderAffiliationId,AdministrationGroupId,SpecialtyId").FirstOrDefault();


                    return provider;
                }
            }
        }

        public Models.ClinicalConsultations.ClinicalConsultationServicingNonPPNReason GetServicingNonPPNReasonForClinicalConsultation(int servicingNonPPNReasonId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        ServicingNonPPNReasonId = servicingNonPPNReasonId,
                    };

                    var reason = dao.Find<Models.ClinicalConsultations.ClinicalConsultationServicingNonPPNReason>(
                            QueriesCreateClinicalConsultation.GetServicingNonPPNReasonForClinicalConsultation(),
                            parameters: parameters).FirstOrDefault();

                    return reason;
                }
            }
        }

        public Specialty GetSpeciltyInformationForClinicalConsultation(int specialtyId, int lineOfBusinessId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        SpecialtyId = specialtyId,
                        LineOfBusinessId = lineOfBusinessId
                    };

                    var specialty = dao.Find<Specialty>(
                            QueriesCreateClinicalConsultation.GetSpeciltyInformationForClinicalConsultation(),
                            parameters: parameters).FirstOrDefault();

                    return specialty;
                }
            }
        }

        public AdditionalHealthPlan GetAdditionalHealthPlanInformationForClinicalConsultation(int additionalHealthPlanId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        AdditionalHealthPlanId = additionalHealthPlanId
                    };

                    var additionalHealthPlan = dao.Find<AdditionalHealthPlan>(
                            QueriesCreateClinicalConsultation.GetAdditionalHealthPlanInformationForClinicalConsultation(),
                            parameters: parameters).FirstOrDefault();

                    return additionalHealthPlan;
                }
            }
        }

        public IEnumerable<AdditionalHealthPlan> GetHealthPlans()
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {

                    var query = QueriesCreateClinicalConsultation.GetHealthPlans();

                    var healtPlans = dao.Find<AdditionalHealthPlan>(query).ToList();
                    return healtPlans;
                }
            }
        }

        public long Insert(Models.ClinicalConsultations.ClinicalConsultation clinicalConsultation)
        {
            using (var connection = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                connection.Open();

                using (var dao = new Dao(connection))
                {
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            var clinicalConsultationId = dao.ExecuteScalar<long>(
                                QueriesCreateClinicalConsultation.InsertClinicalConsultation(),
                                parameters: new
                                {
                                    ClinicalConsultationNumber = new Dapper.DbString() { Value = clinicalConsultation.ClinicalConsultationNumber, IsAnsi = true, Length = Columns.ClinicalConsultation.CLINICAL_CONSULTATION_NUMBER },
                                    AdditionalHealthPlanId = clinicalConsultation.AdditionalHealthPlan?.AdditionalHealthPlanId,
                                    AdditionalHealthPlanName = new Dapper.DbString() { Value = clinicalConsultation.AdditionalHealthPlan?.AdditionalHealthPlanName, IsAnsi = true, Length = Columns.ClinicalConsultation.ADDITIONAL_HEALTH_PLAN_NAME },
                                    ClinicalConsultationDate = clinicalConsultation.ClinicalConsultationDate,
                                    Purpose = new Dapper.DbString() { Value = clinicalConsultation.Purpose, IsAnsi = true, Length = Columns.ClinicalConsultation.PURPOSE },
                                    IsConsultation = clinicalConsultation.IsConsultation,
                                    CreatedUserId = clinicalConsultation.CreatedUserId,
                                    CreatedBy = clinicalConsultation.CreatedBy,
                                    ExpirationDate = clinicalConsultation.ExpirationDate,
                                    EffectiveDate = clinicalConsultation.EffectiveDate,
                                    AnyContractedSpecialist = clinicalConsultation.AnyContractedSpecialist,
                                    ServicingProviderSpecialtyId = clinicalConsultation.ServicingSpecialty?.SpecialtyId,
                                    SourceIdentifier = new Dapper.DbString() { Value = clinicalConsultation.SourceIdentifier, IsAnsi = true, Length = Columns.ClinicalConsultation.SOURCE_IDENTIFIER },
                                    IsRecreate = clinicalConsultation.IsRecreate,
                                    RecreatedSourceId = clinicalConsultation.OriginalClinicalConsultationId,
                                },
                                transaction: transaction);


                            var clinicalConsultationBeneficiaryId = dao.ExecuteScalar<long>(
                                QueriesCreateClinicalConsultation.InsertClinicalConsultationBeneficiary(),
                                parameters: new
                                {
                                    ClinicalConsultationId = clinicalConsultationId,
                                    BeneficiaryId = clinicalConsultation.Beneficiary.BeneficiaryId,
                                    CardNumber = new Dapper.DbString() { Value = clinicalConsultation.Beneficiary.CardNumber, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiary.CARD_NUMBER },
                                    Name = new Dapper.DbString() { Value = clinicalConsultation.Beneficiary.Name, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiary.NAME },
                                    FirstName = new Dapper.DbString() { Value = clinicalConsultation.Beneficiary.FirstName, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiary.FIRST_NAME },
                                    MiddleName = new Dapper.DbString() { Value = clinicalConsultation.Beneficiary.MiddleName, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiary.MIDDLE_NAME },
                                    LastName = new Dapper.DbString() { Value = clinicalConsultation.Beneficiary.LastName, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiary.LAST_NAME },
                                    BirthDate = clinicalConsultation.Beneficiary.BirthDate,
                                    LineOfBusinessId = clinicalConsultation.Beneficiary.LineOfBusinessId,
                                    LineOfBusinessShortName = new Dapper.DbString() { Value = clinicalConsultation.Beneficiary.LineOfBusinessShortName, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiary.LINE_OF_BUSINESS_SHORT_NAME },
                                    HealthPlanId = clinicalConsultation.Beneficiary.HealthPlanId,
                                    HealthPlanName = new Dapper.DbString() { Value = clinicalConsultation.Beneficiary.HealthPlanName, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiary.HEALTH_PLAN_NAME },
                                    ProductId = clinicalConsultation.Beneficiary.ProductId,
                                    ProductName = new Dapper.DbString() { Value = clinicalConsultation.Beneficiary.ProductName, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiary.PRODUCT_NAME },
                                    Brand = new Dapper.DbString() { Value = clinicalConsultation.Beneficiary.Brand, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiary.BRAND },
                                    Identifier = new Dapper.DbString() { Value = clinicalConsultation.Beneficiary.Identifier, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiary.IDENTIFIER },
                                    BeneficiaryIdentifierId = clinicalConsultation.Beneficiary.BeneficiaryIdentifierId,
                                },
                                transaction: transaction);

                            clinicalConsultation.Beneficiary.Addresses?.ToList().ForEach(address =>
                            {
                                dao.Execute(
                                    QueriesCreateClinicalConsultation.InsertClinicalConsultationBeneficiaryAddress(),
                                    parameters: new
                                    {
                                        ClinicalConsultationBeneficiaryId = clinicalConsultationBeneficiaryId,
                                        AddressTypeId = address.AddressTypeId,
                                        AddressLine1 = new Dapper.DbString() { Value = address.AddressLine1, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryAddress.ADDRESS_LINE1 },
                                        AddressLine2 = new Dapper.DbString() { Value = address.AddressLine2, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryAddress.ADDRESS_LINE2 },
                                        AddressLine3 = new Dapper.DbString() { Value = address.AddressLine3, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryAddress.ADDRESS_LINE3 },
                                        City = new Dapper.DbString() { Value = address.City, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryAddress.CITY },
                                        State = new Dapper.DbString() { Value = address.State, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryAddress.STATE },
                                        Place = new Dapper.DbString() { Value = address.Place, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryAddress.PLACE },
                                        ZIPCode = new Dapper.DbString() { Value = address.ZIPCode, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryAddress.ZIPCODE },
                                        ZIP4Code = new Dapper.DbString() { Value = address.ZIP4Code, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryAddress.ZIP4CODE },
                                        CountryFIPSCode = new Dapper.DbString() { Value = address.CountryFIPSCode, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryAddress.COUNTRY_FIPS_CODE },
                                        IsPrimary = address.IsPrimary,
                                    },
                                    transaction: transaction);
                            });

                            clinicalConsultation.Beneficiary.Phones?.ToList().ForEach(phone =>
                            {
                                dao.Execute(
                                    QueriesCreateClinicalConsultation.InsertClinicalConsultationBeneficiaryPhone(),
                                    parameters: new
                                    {
                                        ClinicalConsultationBeneficiaryId = clinicalConsultationBeneficiaryId,
                                        PhoneTypeId = phone.PhoneTypeId,
                                        CountryCode = new Dapper.DbString() { Value = phone.CountryCode, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryPhone.COUNTRY_CODE },
                                        AreaCode = new Dapper.DbString() { Value = phone.AreaCode, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryPhone.AREA_CODE },
                                        Exchange = new Dapper.DbString() { Value = phone.Exchange, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryPhone.EXCHANGE },
                                        Number = new Dapper.DbString() { Value = phone.Number, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryPhone.NUMBER },
                                        PhoneNumber = new Dapper.DbString() { Value = phone.PhoneNumber, IsAnsi = true, Length = Columns.ClinicalConsultationBeneficiaryPhone.PHONE_NUMBER },
                                        IsPrimary = phone.IsPrimary,
                                    },
                                    transaction: transaction);
                            });

                            clinicalConsultation.Diagnosis.ToList().ForEach(diagnosis =>
                            {
                                dao.Execute(
                                    QueriesCreateClinicalConsultation.InsertClinicalConsultationDiagnosis(),
                                    parameters: new
                                    {
                                        ClinicalConsultationId = clinicalConsultationId,
                                        DiagnosisId = diagnosis.DiagnosisId,
                                        Code = new Dapper.DbString() { Value = diagnosis.Code, IsAnsi = true, Length = Columns.ClinicalConsultationDiagnosis.CODE },
                                        Description = new Dapper.DbString() { Value = diagnosis.Description, IsAnsi = true, Length = Columns.ClinicalConsultationDiagnosis.DESCRIPTION },
                                        IsPrimary = diagnosis.IsPrimary,
                                    },
                                    transaction: transaction);
                            });

                            clinicalConsultation.Procedures.ToList().ForEach(procedure =>
                            {
                                dao.Execute(
                                    QueriesCreateClinicalConsultation.InsertClinicalConsultationProcedureBundle(),
                                    parameters: new
                                    {
                                        ClinicalConsultationId = clinicalConsultationId,
                                        ProcedureBundleId = procedure.ProcedureBundleId,
                                        Description = new Dapper.DbString() { Value = procedure.Description, IsAnsi = true, Length = Columns.ClinicalConsultationProcedureBundle.DESCRIPTION },
                                        Units = procedure.Units,
                                        ReferenceCode = new Dapper.DbString() { Value = procedure.ReferenceCode, IsAnsi = true, Length = Columns.ClinicalConsultationProcedureBundle.REFERENCE_CODE },
                                        ServiceTypeCode = new Dapper.DbString() { Value = procedure.ServiceTypeCode, IsAnsi = true, Length = Columns.ClinicalConsultationProcedureBundle.SERVICE_TYPE_CODE },
                                    },
                                    transaction: transaction);
                            });

                            if (clinicalConsultation.PCP != null)
                            {
                                var clinicalConsultationProviderIdPCP = dao.ExecuteScalar<long>(
                                    QueriesCreateClinicalConsultation.InsertClinicalConsultationProvider(),
                                    parameters: new
                                    {
                                        ClinicalConsultationId = clinicalConsultationId,
                                        ProviderAffiliationId = clinicalConsultation.PCP.ProviderAffiliationId,
                                        RenderingProviderId = clinicalConsultation.PCP.RenderingProviderId,
                                        BillingProviderId = clinicalConsultation.PCP.BillingProviderId,
                                        RenderingNPI = new Dapper.DbString() { Value = clinicalConsultation.PCP.RenderingNPI, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.RENDERING_NPI },
                                        BillingNPI = new Dapper.DbString() { Value = clinicalConsultation.PCP.BillingNPI, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.BILLING_NPI },
                                        Name = new Dapper.DbString() { Value = clinicalConsultation.PCP.Name, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.NAME },
                                        FirstName = new Dapper.DbString() { Value = clinicalConsultation.PCP.FirstName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.FIRST_NAME },
                                        MiddleName = new Dapper.DbString() { Value = clinicalConsultation.PCP.MiddleName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.MIDDLE_NAME },
                                        LastName = new Dapper.DbString() { Value = clinicalConsultation.PCP.LastName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.LAST_NAME },
                                        ClinicalConsultationProviderTypeId = clinicalConsultation.PCP.ClinicalConsultationProviderTypeId,
                                        AddressId = clinicalConsultation.PCP.AddressId,
                                        FullAddress = new Dapper.DbString() { Value = clinicalConsultation.PCP.FullAddress, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.FULL_ADDRESS },
                                        AddressLine1 = new Dapper.DbString() { Value = clinicalConsultation.PCP.AddressLine1, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.ADDRESS_LINE1 },
                                        AddressLine2 = new Dapper.DbString() { Value = clinicalConsultation.PCP.AddressLine2, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.ADDRESS_LINE2 },
                                        CountyName = new Dapper.DbString() { Value = clinicalConsultation.PCP.CountyName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.COUNTY_NAME },
                                        StateName = new Dapper.DbString() { Value = clinicalConsultation.PCP.StateName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.COUNTY_NAME },
                                        ZipCode = new Dapper.DbString() { Value = clinicalConsultation.PCP.ZipCode, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.ZIP_CODE },
                                        LocationCodeAddress = new Dapper.DbString() { Value = clinicalConsultation.PCP.LocationCodeAddress, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.LOCATION_CODE_ADDRESS },
                                        LocationAddress = new Dapper.DbString() { Value = clinicalConsultation.PCP.LocationAddress, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.LOCATION_ADDRESS },
                                        PhoneId = clinicalConsultation.PCP.PhoneId,
                                        PhoneNumber = new Dapper.DbString() { Value = clinicalConsultation.PCP.PhoneNumber, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.PHONE_NUMBER },
                                        FaxId = clinicalConsultation.PCP.FaxId,
                                        FaxNumber = new Dapper.DbString() { Value = clinicalConsultation.PCP.FaxNumber, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.FAX_NUMBER },
                                        Email = new Dapper.DbString() { Value = clinicalConsultation.PCP.Email, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.EMAIL },
                                        IsPPN = clinicalConsultation.PCP.IsPPN,
                                        ClinicalConsultationProviderClassificationId = clinicalConsultation.PCP.ClinicalConsultationProviderClassificationId,
                                        IsBestPractice = clinicalConsultation.PCP.IsBestPractice,
                                        IsPriority = clinicalConsultation.PCP.IsPriority,
                                        IsCapitated = clinicalConsultation.PCP.IsCapitated,
                                        AdministrationGroupId = clinicalConsultation.PCP.AdministrationGroupId,
                                        AdministrationGroupName = new Dapper.DbString() { Value = clinicalConsultation.PCP.AdministrationGroupName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.ADMINISTRATION_GROUP_NAME },
                                    },
                                    transaction: transaction);

                                clinicalConsultation.PCP.Specialties?.ToList().ForEach(specialty =>
                                {
                                    dao.Execute(
                                        QueriesCreateClinicalConsultation.InsertClinicalConsultationProviderSpecialty(),
                                        parameters: new
                                        {
                                            ClinicalConsultationProviderId = clinicalConsultationProviderIdPCP,
                                            SpecialtyId = specialty.SpecialtyId,
                                            Name = new Dapper.DbString() { Value = specialty.Name, IsAnsi = true, Length = Columns.ClinicalConsultationProviderSpecialty.NAME },
                                            ProviderEntityTypeId = specialty.ProviderEntityTypeId,
                                        },
                                        transaction: transaction);
                                });
                            }


                            var clinicalConsultationProviderIdRequesting = dao.ExecuteScalar<long>(
                                 QueriesCreateClinicalConsultation.InsertClinicalConsultationProvider(),
                                 parameters: new
                                 {
                                     ClinicalConsultationId = clinicalConsultationId,
                                     ProviderAffiliationId = clinicalConsultation.Requesting.ProviderAffiliationId,
                                     RenderingProviderId = clinicalConsultation.Requesting.RenderingProviderId,
                                     BillingProviderId = clinicalConsultation.Requesting.BillingProviderId,
                                     RenderingNPI = new Dapper.DbString() { Value = clinicalConsultation.Requesting.RenderingNPI, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.RENDERING_NPI },
                                     BillingNPI = new Dapper.DbString() { Value = clinicalConsultation.Requesting.BillingNPI, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.BILLING_NPI },
                                     Name = new Dapper.DbString() { Value = clinicalConsultation.Requesting.Name, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.NAME },
                                     FirstName = new Dapper.DbString() { Value = clinicalConsultation.Requesting.FirstName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.FIRST_NAME },
                                     MiddleName = new Dapper.DbString() { Value = clinicalConsultation.Requesting.MiddleName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.MIDDLE_NAME },
                                     LastName = new Dapper.DbString() { Value = clinicalConsultation.Requesting.LastName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.LAST_NAME },
                                     ClinicalConsultationProviderTypeId = clinicalConsultation.Requesting.ClinicalConsultationProviderTypeId,
                                     AddressId = clinicalConsultation.Requesting.AddressId,
                                     FullAddress = new Dapper.DbString() { Value = clinicalConsultation.Requesting.FullAddress, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.FULL_ADDRESS },
                                     AddressLine1 = new Dapper.DbString() { Value = clinicalConsultation.Requesting.AddressLine1, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.ADDRESS_LINE1 },
                                     AddressLine2 = new Dapper.DbString() { Value = clinicalConsultation.Requesting.AddressLine2, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.ADDRESS_LINE2 },
                                     CountyName = new Dapper.DbString() { Value = clinicalConsultation.Requesting.CountyName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.COUNTY_NAME },
                                     StateName = new Dapper.DbString() { Value = clinicalConsultation.Requesting.StateName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.COUNTY_NAME },
                                     ZipCode = new Dapper.DbString() { Value = clinicalConsultation.Requesting.ZipCode, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.ZIP_CODE },
                                     LocationCodeAddress = new Dapper.DbString() { Value = clinicalConsultation.Requesting.LocationCodeAddress, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.LOCATION_CODE_ADDRESS },
                                     LocationAddress = new Dapper.DbString() { Value = clinicalConsultation.Requesting.LocationAddress, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.LOCATION_ADDRESS },
                                     PhoneId = clinicalConsultation.Requesting.PhoneId,
                                     PhoneNumber = new Dapper.DbString() { Value = clinicalConsultation.Requesting.PhoneNumber, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.PHONE_NUMBER },
                                     FaxId = clinicalConsultation.Requesting.FaxId,
                                     FaxNumber = new Dapper.DbString() { Value = clinicalConsultation.Requesting.FaxNumber, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.FAX_NUMBER },
                                     Email = new Dapper.DbString() { Value = clinicalConsultation.Requesting.Email, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.EMAIL },
                                     IsPPN = clinicalConsultation.Requesting.IsPPN,
                                     ClinicalConsultationProviderClassificationId = clinicalConsultation.Requesting.ClinicalConsultationProviderClassificationId,
                                     IsBestPractice = clinicalConsultation.Requesting.IsBestPractice,
                                     IsPriority = clinicalConsultation.Requesting.IsPriority,
                                     IsCapitated = clinicalConsultation.Requesting.IsCapitated,
                                     AdministrationGroupId = clinicalConsultation.Requesting.AdministrationGroupId,
                                     AdministrationGroupName = new Dapper.DbString() { Value = clinicalConsultation.Requesting.AdministrationGroupName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.ADMINISTRATION_GROUP_NAME },
                                 },
                                 transaction: transaction);

                            clinicalConsultation.Requesting.Specialties?.ToList().ForEach(specialty =>
                            {
                                dao.Execute(
                                    QueriesCreateClinicalConsultation.InsertClinicalConsultationProviderSpecialty(),
                                    parameters: new
                                    {
                                        ClinicalConsultationProviderId = clinicalConsultationProviderIdRequesting,
                                        SpecialtyId = specialty.SpecialtyId,
                                        Name = new Dapper.DbString() { Value = specialty.Name, IsAnsi = true, Length = Columns.ClinicalConsultationProviderSpecialty.NAME },
                                        ProviderEntityTypeId = specialty.ProviderEntityTypeId,
                                    },
                                    transaction: transaction);
                            });


                            if (clinicalConsultation.Servicing != null)
                            {
                                var clinicalConsultationProviderIdServicing = dao.ExecuteScalar<long>(
                                    QueriesCreateClinicalConsultation.InsertClinicalConsultationProvider(),
                                    parameters: new
                                    {
                                        ClinicalConsultationId = clinicalConsultationId,
                                        ProviderAffiliationId = clinicalConsultation.Servicing.ProviderAffiliationId,
                                        RenderingProviderId = clinicalConsultation.Servicing.RenderingProviderId,
                                        BillingProviderId = clinicalConsultation.Servicing.BillingProviderId,
                                        RenderingNPI = new Dapper.DbString() { Value = clinicalConsultation.Servicing.RenderingNPI, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.RENDERING_NPI },
                                        BillingNPI = new Dapper.DbString() { Value = clinicalConsultation.Servicing.BillingNPI, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.BILLING_NPI },
                                        Name = new Dapper.DbString() { Value = clinicalConsultation.Servicing.Name, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.NAME },
                                        FirstName = new Dapper.DbString() { Value = clinicalConsultation.Servicing.FirstName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.FIRST_NAME },
                                        MiddleName = new Dapper.DbString() { Value = clinicalConsultation.Servicing.MiddleName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.MIDDLE_NAME },
                                        LastName = new Dapper.DbString() { Value = clinicalConsultation.Servicing.LastName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.LAST_NAME },
                                        ClinicalConsultationProviderTypeId = clinicalConsultation.Servicing.ClinicalConsultationProviderTypeId,
                                        AddressId = clinicalConsultation.Servicing.AddressId,
                                        FullAddress = new Dapper.DbString() { Value = clinicalConsultation.Servicing.FullAddress, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.FULL_ADDRESS },
                                        AddressLine1 = new Dapper.DbString() { Value = clinicalConsultation.Servicing.AddressLine1, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.ADDRESS_LINE1 },
                                        AddressLine2 = new Dapper.DbString() { Value = clinicalConsultation.Servicing.AddressLine2, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.ADDRESS_LINE2 },
                                        CountyName = new Dapper.DbString() { Value = clinicalConsultation.Servicing.CountyName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.COUNTY_NAME },
                                        StateName = new Dapper.DbString() { Value = clinicalConsultation.Servicing.StateName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.COUNTY_NAME },
                                        ZipCode = new Dapper.DbString() { Value = clinicalConsultation.Servicing.ZipCode, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.ZIP_CODE },
                                        LocationCodeAddress = new Dapper.DbString() { Value = clinicalConsultation.Servicing.LocationCodeAddress, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.LOCATION_CODE_ADDRESS },
                                        LocationAddress = new Dapper.DbString() { Value = clinicalConsultation.Servicing.LocationAddress, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.LOCATION_ADDRESS },
                                        PhoneId = clinicalConsultation.Servicing.PhoneId,
                                        PhoneNumber = new Dapper.DbString() { Value = clinicalConsultation.Servicing.PhoneNumber, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.PHONE_NUMBER },
                                        FaxId = clinicalConsultation.Servicing.FaxId,
                                        FaxNumber = new Dapper.DbString() { Value = clinicalConsultation.Servicing.FaxNumber, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.FAX_NUMBER },
                                        Email = new Dapper.DbString() { Value = clinicalConsultation.Servicing.Email, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.EMAIL },
                                        IsPPN = clinicalConsultation.Servicing.IsPPN,
                                        ClinicalConsultationProviderClassificationId = clinicalConsultation.Servicing.ClinicalConsultationProviderClassificationId,
                                        IsBestPractice = clinicalConsultation.Servicing.IsBestPractice,
                                        IsPriority = clinicalConsultation.Servicing.IsPriority,
                                        IsCapitated = clinicalConsultation.Servicing.IsCapitated,
                                        AdministrationGroupId = clinicalConsultation.Servicing.AdministrationGroupId,
                                        AdministrationGroupName = new Dapper.DbString() { Value = clinicalConsultation.Servicing.AdministrationGroupName, IsAnsi = true, Length = Columns.ClinicalConsultationProvider.ADMINISTRATION_GROUP_NAME },
                                    },
                                    transaction: transaction);

                                clinicalConsultation.Servicing.Specialties?.ToList().ForEach(specialty =>
                                {
                                    dao.Execute(
                                        QueriesCreateClinicalConsultation.InsertClinicalConsultationProviderSpecialty(),
                                        parameters: new
                                        {
                                            ClinicalConsultationProviderId = clinicalConsultationProviderIdServicing,
                                            SpecialtyId = specialty.SpecialtyId,
                                            Name = new Dapper.DbString() { Value = specialty.Name, IsAnsi = true, Length = Columns.ClinicalConsultationProviderSpecialty.NAME },
                                            ProviderEntityTypeId = specialty.ProviderEntityTypeId,
                                        },
                                        transaction: transaction);
                                });
                            }

                            if (clinicalConsultation.ServicingNonPPNReason != null)
                            {
                                dao.Execute(
                                    QueriesCreateClinicalConsultation.InsertClinicalConsultationServicingNonPPNReason(),
                                    parameters: new
                                    {
                                        ClinicalConsultationId = clinicalConsultationId,
                                        NoPPNReasonId = clinicalConsultation.ServicingNonPPNReason.NoPPNReasonId,
                                        NoPPNReasonDescription = new Dapper.DbString() { Value = clinicalConsultation.ServicingNonPPNReason.NoPPNReasonDescription, IsAnsi = true, Length = Columns.ClinicalConsultationServicingNonPPNReason.NO_PPN_REASON_DESCRIPTION },
                                    },
                                    transaction: transaction);
                            }

                            transaction.Commit();

                            return clinicalConsultationId;
                        }
                        catch (Exception)
                        {
                            transaction.Rollback();
                            throw;
                        }
                    }
                }
            }
        }

        public int GetSequenceNumber(bool isConsultation)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var sequence = "";
                    if (isConsultation)
                    {
                        sequence = QueriesCreateClinicalConsultation.GetClinicalConsultationSequenceNumber();
                    }
                    else
                    {
                        sequence = QueriesCreateClinicalConsultation.GetClinicalReferralSequenceNumber();
                    }

                    return dao.Find<int>(sequence).First();
                }
            }

        }

        public RecreateClinicalConsultation GetClinicalConsultationForRecreate(int clinicalConsultationId, int lineOfBusinessId)
        {

            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    RecreateClinicalConsultation consultation = null;
                    var parameters = new
                    {
                        ClinicalConsultationId = clinicalConsultationId,
                        LineOfBusinessId = lineOfBusinessId
                    };

                    var reader = dao.FindMultiple(QueriesCreateClinicalConsultation.GetClinicalConsutationInformationForRecreate(), parameters);

                    if (reader != null)
                    {
                        consultation = setConsultationDataFromReader(reader);
                    }
                    return consultation;
                }
            }
        }

       

        public int FindConsultationBeneficiaryId(int clinicalConsultationId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var query = QueriesCreateClinicalConsultation.FindConsultationBeneficiaryId();

                    var parameters = new
                    {
                        ClinicalConsultationId = clinicalConsultationId,
                    };

                    var beneficiaryId = dao.Find<int>(query, parameters).FirstOrDefault();

                    return beneficiaryId;
                }
            }
        }

        public IEnumerable<SuggestionsResponse> GetRecentSuggestions(int beneficiaryId, int maxAmount)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {

                    IEnumerable<SuggestionsResponse> suggestions = null;
                    var parameters = new
                    {
                        BeneficiaryId = beneficiaryId,
                        Offset = 0,
                        Fetch = maxAmount
                    };

                    suggestions = dao.Find<SuggestionsResponse>(
                                               QueriesCreateClinicalConsultation.GetRecentSuggestionsBaseData(),
                                               parameters: parameters).ToList();


                   

                    return suggestions;
                }
            }
        }

                               

        #region Private
        private static RecreateClinicalConsultation setConsultationDataFromReader(Dapper.SqlMapper.GridReader reader)
        {
            RecreateClinicalConsultation consultation = SetConsultationBaseFields(reader);
            if (consultation != null)
            {
                consultation.Diagnoses = reader.Read<Models.ClinicalConsultations.ClinicalConsultationDiagnosis>().AsEnumerable();

                consultation.Procedure = reader.Read<Models.ClinicalConsultations.ClinicalConsultationProcedureBundle>().FirstOrDefault();

                var specialties = new List<ProviderSpecialty>();
                var cities = new List<City>();

                SetRequestingProvider(consultation, reader, specialties, cities);

                specialties = new List<ProviderSpecialty>();
                cities = new List<City>();

                SetSercivingProvider(consultation, reader, specialties, cities);
            }

            return consultation;
        }

        private static RecreateClinicalConsultation SetConsultationBaseFields(Dapper.SqlMapper.GridReader reader)
        {
            return reader.Read<RecreateClinicalConsultation, ProviderSpecialty, AdditionalHealthPlan, ServicingNonPPNReason, RecreateClinicalConsultation>(
                (clinicalConsultation, servicingSpecialty, additionalHealthPlan, nonPPNReason) =>
                {
                    clinicalConsultation.ServicingSpecialty = servicingSpecialty;

                    if (additionalHealthPlan != null && additionalHealthPlan.AdditionalHealthPlanId != 0)
                    {
                        clinicalConsultation.AdditionalHealthPlan = additionalHealthPlan;
                    }

                    if (nonPPNReason != null && nonPPNReason.ServicingNonPPNReasonId != 0)
                    {
                        clinicalConsultation.ServicingNonPPNReason = nonPPNReason;
                    }

                    return clinicalConsultation;
                }, splitOn: "ClinicalConsultationId,SpecialtyId,AdditionalHealthPlanId,ServicingNonPPNReasonId"
            ).FirstOrDefault();
        }

        private static void SetRequestingProvider(RecreateClinicalConsultation consultation, Dapper.SqlMapper.GridReader reader, List<ProviderSpecialty> specialties, List<City> cities)
        {
            reader.Read<RequestingProvider, City, ProviderSpecialty, City, RequestingProvider>(
                (provider, city, specialty, selectedCity) =>
                {
                    if (consultation.RequestingProvider == null)
                    {
                        consultation.RequestingProvider = provider;
                        consultation.RequestingProvider.Cities = cities;
                        consultation.RequestingProvider.Specialties = specialties;
                        consultation.RequestingCity = selectedCity;
                    }

                    SetCityAndSpecialties(specialties, cities, city, specialty);

                    return provider;
                }, splitOn: "ProviderAffiliationId,CityId,SpecialtyId,CityId");
        }       

        private static void SetSercivingProvider(RecreateClinicalConsultation consultation, Dapper.SqlMapper.GridReader reader, List<ProviderSpecialty> specialties, List<City> cities)
        {
            reader.Read<ServicingProvider, City, ProviderSpecialty, City, ServicingProvider>(
                (provider, city, specialty, selectedCity) =>
                {
                    if (consultation.ServicingProvider == null)
                    {
                        consultation.ServicingProvider = provider;
                        consultation.ServicingProvider.Cities = cities;
                        consultation.ServicingProvider.Specialties = specialties;
                        consultation.ServicingCity = selectedCity;
                    }

                    SetCityAndSpecialties(specialties, cities, city, specialty);

                    return provider;
                }, splitOn: "ProviderAffiliationId,CityId,SpecialtyId,CityId");
        }

        private static void SetCityAndSpecialties(List<ProviderSpecialty> specialties, List<City> cities, City city, ProviderSpecialty specialty)
        {
            if (!specialties.Any((s) => { return s.SpecialtyId == specialty.SpecialtyId; }))
            {
                specialties.Add(specialty);
            }

            if (!cities.Any((c) => { return c.CityId == city.CityId && c.ZipCode == city.ZipCode; }))
            {
                cities.Add(city);
            }
        }
        #endregion
    }
}
