using com.InnovaMD.Provider.Business.Common;
using com.InnovaMD.Provider.Data.ClinicalConsultations;
using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Requests;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Provider.Models.SystemConfiguration;
using com.InnovaMD.Utilities.Dates;
using com.InnovaMD.Utilities.Schema.SchemaModels.Definitions;
using Microsoft.Extensions.Logging;
using Microsoft.PowerBI.Api;
using Microsoft.PowerBI.Api.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace com.InnovaMD.Provider.Business
{
    public class ClinicalConsultationHistoryComponent : BusinessComponentBase, IClinicalConsultationHistoryComponent
    {
        private readonly ILogger<ClinicalConsultationHistoryComponent> _logger;
        private readonly IClinicalConsultationHistoryRepository _clinicalConsultationRepository;
        private readonly ClinicalConsultationConfigurationModel _clinicalConsultationModel;
        private readonly ReportsConfigurationModel _reportsConfigurationModel;
        private readonly GlobalConfigurationModel _globalConfigurationModel;
        private readonly IReportsRepository _reportRepository;
        private readonly IBeneficiaryRepository _beneficiaryRepository;

        public ClinicalConsultationHistoryComponent(
            ILogger<ClinicalConsultationHistoryComponent> logger,
            IClinicalConsultationHistoryRepository clinicalConsultationRepository,
            IReportsRepository reportRepository,
            ClinicalConsultationConfigurationModel clinicalConsultationModel,
            ReportsConfigurationModel reportsConfigurationModel,
            GlobalConfigurationModel globalConfigurationModel,
            IBeneficiaryRepository beneficiaryRepository)
        {
            _logger = logger;
            _clinicalConsultationRepository = clinicalConsultationRepository;
            _reportRepository = reportRepository;

            _clinicalConsultationModel = clinicalConsultationModel;
            _reportsConfigurationModel = reportsConfigurationModel;
            _globalConfigurationModel = globalConfigurationModel;
            _beneficiaryRepository = beneficiaryRepository;
        }

        public BeneficiaryClinicalConsultationsResponse GetBeneficiaryClinicalConsultations(BeneficiaryClinicalConsultationsRequest request, IdentityUser user)
        {
            var response = new BeneficiaryClinicalConsultationsResponse();
            var searchCriteria = new BeneficiaryClinicalConsultationSearchCriteria()
            {
                BeneficiaryId = int.Parse(request.BeneficiaryId),
                Page = request.Page,
                PageSize = _clinicalConsultationModel.ClinicalConsultationHistoryPageSize,
            };

            if (request.Search != null)
            {
                if (Regex.Match(request.Search.ToUpper(), "^(RF|CN)*[0-9]+$").Success)
                {
                    searchCriteria.ClinicalConsultationNumber = request.Search.ToUpper();
                }
                else
                {
                    searchCriteria.ProviderName = request.Search;
                }
            }

            var (consultations, total) = _clinicalConsultationRepository.GetBeneficiaryClinicalConsultations(searchCriteria, user.UserId);
            response.ClinicalConsultations = consultations;
            response.TotalCount = total;
            return response;
        }

        public ClinicalConsultation GetClinicalConsultationDetail(int clinicalConsultationId, IdentityUser user)
        {
            return _clinicalConsultationRepository.GetClinicalConsultationDetail(clinicalConsultationId, user.UserId);
        }

        public async Task<Models.Common.Report> GetClinicalConsultationForm(int clinicalConsultationId)
        {
            //Obtain configurations that do not depend on the LineOfBusiness first.
            var powerBiApiBaseURL = _reportsConfigurationModel.PowerBiApiBaseURL;

            //Obtain beneficiary line of business, to get the correct configurations
            var lineOfBusinessId = _clinicalConsultationRepository.GetLineOfBusinessFromClinicalConsultationId(clinicalConsultationId);
            _reportsConfigurationModel.LineOfBusinessId = lineOfBusinessId;
            _globalConfigurationModel.LineOfBusinessId = lineOfBusinessId;

            // Authentication and client creation
            var accessToken = await _reportRepository.GetAzureAppAccessToken();
            var client = new PowerBIClient(
                new Uri(powerBiApiBaseURL),
                new TokenCredentials(accessToken, "Bearer"));

            //Send export request
            var parameters = new PaginatedReportExportConfiguration()
            {
                ParameterValues =
                        [
                            new() {Name = "ClinicalConsultationId", Value= clinicalConsultationId.ToString()},
                            new() {Name = "Disclaimer", Value = _globalConfigurationModel.ReferralsShowDisclamerPrint ? _globalConfigurationModel.ReferralsDisclamerMessage : string.Empty},
                            new() {Name = "ReportDateTime", Value = DateTime.UtcNow.FromUtcToSystemTimezone().ToString("MM/dd/yyyy hh:mm:ss tt")},
                        ]
            };
            var request = new ExportReportRequest
            {
                Format = FileFormat.PDF,
                PaginatedReportConfiguration = parameters,
            };
            var groupId = Guid.Parse(_reportsConfigurationModel.ClinicalConsultationReportsGroupId);
            var reportId = Guid.Parse(_reportsConfigurationModel.ClinicalConsultationReportId);

            var export = await client.Reports.ExportToFileInGroupAsync(
                groupId,
                reportId,
                request
            );

            //Polling to verify export status
            Export exportStatus = null;
            var timeOutInMinutes = _reportsConfigurationModel.ClinicalConsultationExportReportTimeout;
            var startTime = DateTime.UtcNow;
            do
            {
                if (DateTime.UtcNow.Subtract(startTime).TotalMinutes > timeOutInMinutes)
                {
                    return null;
                }
                var httpMessage =
                    await client.Reports.GetExportToFileStatusInGroupWithHttpMessagesAsync(groupId, reportId, export.Id);

                exportStatus = httpMessage.Body;
                if (exportStatus.Status == ExportState.Running || exportStatus.Status == ExportState.NotStarted)
                {
                    var headerRetryAfter = httpMessage.Response.Headers.RetryAfter;
                    var configRetryAfter = _reportsConfigurationModel.RetryAfter;

                    var retryAfterInSec = configRetryAfter > 0 ? configRetryAfter : headerRetryAfter.Delta.Value.Seconds;

                    await Task.Delay(retryAfterInSec * 1000);
                }
            }
            // While not in a terminal state, keep polling
            while (exportStatus.Status != ExportState.Succeeded && exportStatus.Status != ExportState.Failed);


            //Obtain pdf
            if (exportStatus.Status == ExportState.Succeeded)
            {
                var httpMessage =
                    await client.Reports.GetFileOfExportToFileInGroupWithHttpMessagesAsync(groupId, reportId, export.Id);

                return new Models.Common.Report
                {
                    Content = httpMessage.Body?.ReadAllBytes(),
                    FileName = _reportsConfigurationModel.ClinicalConsultationExportReportName,
                    ContentType = "application/pdf"
                };
            }

            return null;
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
