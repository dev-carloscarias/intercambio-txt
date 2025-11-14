using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Data.ClinicalConsultations.Strategies.Queries;
using com.InnovaMD.Provider.Data.Common;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.Common;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations.Strategies
{
    public class ProviderRequestingProviderRepository : RepositoryBase, IProviderRequestingProviderRepository
    {
        public ProviderRequestingProviderRepository(ConnectionStringOptions connectionStringOptions) : base(connectionStringOptions)
        {
        }

        public (IEnumerable<RequestingProvider> providers, int count) SearchRequestingProviders(RequestingProviderSearchCriteria criteria, int lineOfBusinessId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        LineOfBusinessId = lineOfBusinessId,
                        ProviderNPI = !string.IsNullOrEmpty(criteria.NPI) ? new Dapper.DbString() { Value = $"{criteria.NPI}%", IsAnsi = true, Length = criteria.NPI.Length + 1 } : null,
                        ProviderName = !string.IsNullOrEmpty(criteria.ProviderName) ? new Dapper.DbString() { Value = $"%{criteria.ProviderName}%", IsAnsi = true, Length = criteria.ProviderName.Length + 2 } : null,
                        Offset = criteria.Offset,
                        Fetch = criteria.Fetch
                    };

                    var count = dao.ExecuteScalar<int>(ProviderQueriesRequestingProvider.SearchRequestingProviders(criteria, true), parameters);
                    var providers = new List<RequestingProvider>();

                    if (count > 0)
                    {
                        providers = dao.Find<RequestingProvider>(
                            ProviderQueriesRequestingProvider.SearchRequestingProviders(criteria),
                            parameters: parameters).ToList();

                        if (providers != null && providers.Count > 0)
                        {
                            var reader = dao.FindMultiple(new string[] { ProviderQueriesRequestingProvider.GetProvidersLocations(), ProviderQueriesRequestingProvider.GetProvidersSpecialties() }, parameters: new
                            {
                                LineOfBusinessId = lineOfBusinessId,
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
                                }
                            }
                        }
                    }

                    return (providers, count);
                }
            }
        }

        public (IEnumerable<RequestingProvider> providers, int count) SearchRequestingProviders(RequestingProviderSearchCriteria criteria, int lineOfBusinessId, int billingProviderId)
        {
            using (var conn = new SqlConnection(connectionStringOptions.ClinicalConsultation))
            {
                using (var dao = new Dao(conn))
                {
                    var parameters = new
                    {
                        LineOfBusinessId = lineOfBusinessId,
                        BillingProviderId = billingProviderId,
                        ProviderNPI = !string.IsNullOrEmpty(criteria.NPI) ? new Dapper.DbString() { Value = $"{criteria.NPI}%", IsAnsi = true, Length = criteria.NPI.Length + 1 } : null,
                        ProviderName = !string.IsNullOrEmpty(criteria.ProviderName) ? new Dapper.DbString() { Value = $"%{criteria.ProviderName}%", IsAnsi = true, Length = criteria.ProviderName.Length + 2 } : null,
                        Offset = criteria.Offset,
                        Fetch = criteria.Fetch
                    };

                    var count = dao.ExecuteScalar<int>(ProviderQueriesRequestingProvider.SearchRequestingProviderByBilling(criteria, true), parameters);
                    var providers = new List<RequestingProvider>();

                    if (count > 0)
                    {
                        providers = dao.Find<RequestingProvider>(
                            ProviderQueriesRequestingProvider.SearchRequestingProviderByBilling(criteria),
                            parameters: parameters).ToList();

                        if (providers != null && providers.Count > 0)
                        {
                            var reader = dao.FindMultiple(new string[] { ProviderQueriesRequestingProvider.GetProvidersLocations(), ProviderQueriesRequestingProvider.GetProvidersSpecialties() }, parameters: new
                            {
                                LineOfBusinessId = lineOfBusinessId,
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
                                }
                            }
                        }
                    }

                    return (providers, count);
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
