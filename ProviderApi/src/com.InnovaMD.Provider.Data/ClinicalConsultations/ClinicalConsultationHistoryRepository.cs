using Azure;
using com.InnovaMD.Provider.Data.ClinicalConsultations.Queries;
using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Data.Common;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Filters;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.Models.Common;
using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations
{
    public class ClinicalConsultationHistoryRepository : RepositoryBase ,IClinicalConsultationHistoryRepository
    {
        public ClinicalConsultationHistoryRepository(ConnectionStringOptions connectionStringOptions) : base(connectionStringOptions)
        {
        }

        public (IEnumerable<ClinicalConsultation>, int) GetBeneficiaryClinicalConsultations(BeneficiaryClinicalConsultationSearchCriteria searchCriteria, int userId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var clinicalConsultations = new List<ClinicalConsultation>();
                    var count = 0;
                    var parameters = new
                    {
                        BeneficiaryId = searchCriteria.BeneficiaryId,
                        ProviderName = !string.IsNullOrEmpty(searchCriteria.ProviderName) ? new Dapper.DbString() { Value = $"%{searchCriteria.ProviderName}%", IsAnsi = true, Length = searchCriteria.ProviderName.Length + 2 } : null,
                        ClinicalConsultationNumber = !string.IsNullOrEmpty(searchCriteria.ClinicalConsultationNumber) ? new Dapper.DbString() { Value = searchCriteria.ClinicalConsultationNumber, IsAnsi = true, Length = searchCriteria.ClinicalConsultationNumber.Length } : null,
                        UserId = userId,
                        Offset = searchCriteria.Offset,
                        PageSize = searchCriteria.PageSize
                    };

                    count = dao.Find<int>(
                        QueriesClinicalConsultation.GetBeneficiaryClinicalConsultationsCount(searchCriteria), 
                        parameters: parameters).FirstOrDefault();

                    if (count > 0)
                    {
                        var currentClinicalConsultation = new ClinicalConsultation();
                        var currentProcedureBundles = new List<ClinicalConsultationProcedureBundle>();

                        dao.Find<ClinicalConsultation, ClinicalConsultationProviderSpecialty, ClinicalConsultationProvider, ClinicalConsultationProvider, ClinicalConsultationProcedureBundle, ClinicalConsultation>(
                            QueriesClinicalConsultation.GetBeneficiaryClinicalConsultations(searchCriteria),
                            (clinicalConsultation, servicingSpecialty, servicing, requesting, procedureBundle) =>
                            {
                                if (currentClinicalConsultation.ClinicalConsultationId != clinicalConsultation.ClinicalConsultationId)
                                {
                                    if (currentClinicalConsultation.ClinicalConsultationId > 0)
                                    {
                                        currentClinicalConsultation.Procedures = currentProcedureBundles;
                                        clinicalConsultations.Add(currentClinicalConsultation);
                                    }

                                    clinicalConsultation.Servicing = servicing;
                                    clinicalConsultation.Requesting = requesting;
                                    clinicalConsultation.ServicingSpecialty = servicingSpecialty;
                                    currentClinicalConsultation = clinicalConsultation;

                                    currentProcedureBundles = new List<ClinicalConsultationProcedureBundle>();
                                }

                                currentProcedureBundles.Add(procedureBundle);

                                return clinicalConsultation;
                            },
                            parameters: parameters,
                            splitOn: "ClinicalConsultationId,SpecialtyId,Name,Name,Description");

                        if (currentClinicalConsultation.ClinicalConsultationId > 0)
                        {
                            currentClinicalConsultation.Procedures = currentProcedureBundles;
                            clinicalConsultations.Add(currentClinicalConsultation);
                        }
                    }

                    return (clinicalConsultations, count);
                }
            }
        }

        public ClinicalConsultation GetClinicalConsultationDetail(int clinicalConsultationId, int userId) 
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var queries = new string[] {
                        QueriesClinicalConsultation.GetClinicalConsultationDetail(),
                        QueriesClinicalConsultation.GetClinicalConsultationDetailDiagnosis(),
                        QueriesClinicalConsultation.GetClinicalConsultationDetailProviders()
                    };

                    var parameters = new
                    {
                        ClinicalConsultationId = clinicalConsultationId,
                        UserId = userId
                    };

                    var gridReader = dao.FindMultiple(queries, parameters);

                    if (gridReader != null)
                    {
                        var consultation = gridReader.Read<ClinicalConsultation, AdditionalHealthPlan, ClinicalConsultationProviderSpecialty, ClinicalConsultationProcedureBundle, ClinicalConsultation>(
                            (cc, ahp, s, p) =>
                        {
                            cc.Procedures = new List<ClinicalConsultationProcedureBundle>() { p };
                            cc.AdditionalHealthPlan = ahp;
                            cc.ServicingSpecialty = s;
                            return cc;
                        }, splitOn: "ClinicalConsultationId,AdditionalHealthPlanId,SpecialtyId,Description").FirstOrDefault();

                        if(consultation != null)
                        {
                            consultation.Diagnosis = gridReader.Read<ClinicalConsultationDiagnosis>().ToList();

                            var providers = gridReader.Read<ClinicalConsultationProvider>();
                            consultation.Requesting = providers.FirstOrDefault(x => x.ClinicalConsultationProviderTypeId == ((int)ClinicalConsultationProviderTypes.Requesting));
                            consultation.Servicing = providers.FirstOrDefault(x => x.ClinicalConsultationProviderTypeId == ((int)ClinicalConsultationProviderTypes.Servicing));
                        }

                        conn.ExecuteScalar<int>(QueriesClinicalConsultation.InsertViewLog(), parameters);

                        return consultation;
                    }
                    return null;
                }
            }
        }

        public int GetLineOfBusinessFromClinicalConsultationId(int clinicalConsultationId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameter = new { ClinicalConsultationId = clinicalConsultationId };

                    var query = QueriesClinicalConsultation.GetLineOfBusinessFromClinicalConsultationId();

                    var lineOfBusinessId = dao.Find<int>(query, parameter).FirstOrDefault();
                    return lineOfBusinessId;
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
