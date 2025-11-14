using Azure.Core;
using com.InnovaMD.Provider.Business.Common;
using com.InnovaMD.Provider.Business.Factories;
using com.InnovaMD.Provider.Data.ClinicalConsultations;
using com.InnovaMD.Provider.Data.ClinicalConsultations.SearchCriteria;
using com.InnovaMD.Provider.Models.ClinicalConsultations;
using com.InnovaMD.Provider.Models.ClinicalConsultations.Response;
using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Provider.Models.SystemConfiguration;
using com.InnovaMD.Utilities.Dates;
using Microsoft.Extensions.Logging;
using System;
using System.Text.RegularExpressions;


namespace com.InnovaMD.Provider.Business
{
    public class BeneficiaryComponent : BusinessComponentBase, IBeneficiaryComponent
    {
        private readonly ILogger<BeneficiaryComponent> _logger;
        private readonly IBeneficiaryRepository _beneficiaryRepository;
        private readonly ClinicalConsultationConfigurationModel _clinicalConsultationModel;

        public BeneficiaryComponent(
            ILogger<BeneficiaryComponent> logger,
            IBeneficiaryRepository beneficiaryRepository,
            ClinicalConsultationConfigurationModel clinicalConsultationModel)
        {
            _logger = logger;
            _beneficiaryRepository = beneficiaryRepository;
            _clinicalConsultationModel = clinicalConsultationModel;
        }

        public BeneficiaryInformation GetBeneficiaryBasicInformation(int beneficiaryId)
        {
            var beneficiaryInfo = _beneficiaryRepository.GetBeneficiaryInformation(beneficiaryId);

            var basicInformation = new BeneficiaryInformation()
            {
                DisplayName = beneficiaryInfo.DisplayName,
                BeneficiaryId = beneficiaryId,
                CardNumber = beneficiaryInfo.CardNumber,
                Identifier = beneficiaryInfo.Identifier,
                LineOfBusinessId = beneficiaryInfo.LineOfBusinessId
            };

            return basicInformation;
        }



        public ClinicalConsultationCreateButton CheckCreateButtonPermissions(int beneficiaryId, IdentityUser user)
        {

            var response = new ClinicalConsultationCreateButton();

            var beneficiaryInfo = _beneficiaryRepository.GetBeneficiaryInformation(beneficiaryId);

            _clinicalConsultationModel.LineOfBusinessId = beneficiaryInfo.LineOfBusinessId;

            if (user.ActiveRole.Context.Id == (int)ApplicationDomainContexts.Provider)
            {
                SetProviderPermissionForCreate(user, response);
            }
            if (user.ActiveRole.Context.Id == (int)ApplicationDomainContexts.AdministrationGroup)
            {
                SetAdminGroupPermissionForCreate(user, response);
            }
            if (user.ActiveRole.Context.Id == (int)ApplicationDomainContexts.Payer)
            {
                response.Message = _clinicalConsultationModel.CreateButtonPayerRepresentativeMessage;
                response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonPayerRepresentativeStartDate, _clinicalConsultationModel.CreateButtonPayerRepresentativeEndDate);
            }

            if (response.Disabled)
            {
                _clinicalConsultationModel.LineOfBusinessId = null; //Next key is not by line of business
                response.GlobalMessage = _clinicalConsultationModel.CreateDisabledGlobalMessage;
            }
            return response;
        }

        #region Private
        private void SetAdminGroupPermissionForCreate(IdentityUser user, ClinicalConsultationCreateButton response)
        {
            switch (user.AdministrationGroup.TypeId)
            {
                case (int)AdministrationGroupTypes.IPA:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonAdministrationGroupIPAMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonAdministrationGroupIPAStartDate, _clinicalConsultationModel.CreateButtonAdministrationGroupIPAEndDate);
                        break;
                    }
                case (int)AdministrationGroupTypes.SIPA:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonAdministrationGroupSIPAMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonAdministrationGroupSIPAStartDate, _clinicalConsultationModel.CreateButtonAdministrationGroupSIPAEndDate);
                        break;
                    }
                case (int)AdministrationGroupTypes.PMG:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonAdministrationGroupPMGMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonAdministrationGroupPMGStartDate, _clinicalConsultationModel.CreateButtonAdministrationGroupPMGEndDate);
                        break;
                    }
                case (int)AdministrationGroupTypes.SPMG:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonAdministrationGroupSPMGMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonAdministrationGroupSPMGStartDate, _clinicalConsultationModel.CreateButtonAdministrationGroupSPMGEndDate);
                        break;
                    }
                case (int)AdministrationGroupTypes.MSO:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonAdministrationGroupMSOMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonAdministrationGroupMSOStartDate, _clinicalConsultationModel.CreateButtonAdministrationGroupMSOEndDate);
                        break;
                    }
            }
        }

        private void SetProviderPermissionForCreate(IdentityUser user, ClinicalConsultationCreateButton response)
        {
            switch (user.ActiveRole.SubContext.Id)
            {
                case (int)ApplicationDomainSubContexts.Specialist:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonProviderSpecialistMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonProviderSpecialistStartDate, _clinicalConsultationModel.CreateButtonProviderSpecialistEndDate);
                        break;
                    }
                case (int)ApplicationDomainSubContexts.PCP:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonProviderPCPMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonProviderPCPStartDate, _clinicalConsultationModel.CreateButtonProviderPCPEndDate);
                        break;
                    }
                case (int)ApplicationDomainSubContexts.GroupPractice:
                    {
                        response.Message = checkValidDates(_clinicalConsultationModel.CreateButtonProviderGroupPracticeStartDate, _clinicalConsultationModel.CreateButtonProviderGroupPracticeEndDate) ? _clinicalConsultationModel.CreateButtonProviderGroupPracticeMessage : "";
                        break;
                    }
                case (int)ApplicationDomainSubContexts.Ancillary:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonProviderAncillaryMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonProviderAncillaryStartDate, _clinicalConsultationModel.CreateButtonProviderAncillaryEndDate);
                        break;
                    }
                case (int)ApplicationDomainSubContexts.Hospital:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonProviderHospitalMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonProviderHospitalStartDate, _clinicalConsultationModel.CreateButtonProviderHospitalEndDate);
                        break;
                    }
                case (int)ApplicationDomainSubContexts.Pharmacy:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonProviderPharmacyMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonProviderPharmacyStartDate, _clinicalConsultationModel.CreateButtonProviderPharmacyEndDate);
                        break;
                    }
                case (int)ApplicationDomainSubContexts.Dental:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonProviderDentalMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonProviderDentalStartDate, _clinicalConsultationModel.CreateButtonProviderDentalEndDate);
                        break;
                    }
                case (int)ApplicationDomainSubContexts.MentalHealthCare:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonProviderMentalHealthCareMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonProviderMentalHealthCareStartDate, _clinicalConsultationModel.CreateButtonProviderMentalHealthCareEndDate);
                        break;
                    }
                case (int)ApplicationDomainSubContexts.CAMP:
                    {
                        response.Message = _clinicalConsultationModel.CreateButtonProviderCampMessage;
                        response.Disabled = checkValidDates(_clinicalConsultationModel.CreateButtonProviderCampStartDate, _clinicalConsultationModel.CreateButtonProviderCampEndDate);
                        break;
                    }

            }
        }

        //Check of valid dates for action disabled in Create Clinical consultation button
        private bool checkValidDates(DateTime? startDate, DateTime? endDate)
        {
            if (startDate.HasValue)//With Start Date is valid
            {
                var now = DateTime.UtcNow;
                var checkStartDateWithNow = startDate.Value.FromSystemTimezoneToUTC() <= now;

                if (checkStartDateWithNow && !endDate.HasValue) //with start date valid ,always disabled
                {
                    return true;
                }
                if (checkStartDateWithNow && now <= endDate.Value.FromSystemTimezoneToUTC())//Valid Range with start date and end date 
                {
                    return true;
                }
                return false;
            }
            return false;
        }

        #endregion

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
