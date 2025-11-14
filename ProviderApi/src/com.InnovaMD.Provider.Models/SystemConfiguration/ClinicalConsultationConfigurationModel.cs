using com.InnovaMD.Provider.Models.Security;
using com.InnovaMD.Utilities.Configuration;
using com.InnovaMD.Utilities.Configuration.OptionModels;
using System;

namespace com.InnovaMD.Provider.Models.SystemConfiguration
{
    public class ClinicalConsultationConfigurationModel : BaseSystemConfigurationOptionModel
    {

        public static string formatForDateTime = "yyyy-MM-dd hh:mm:ss tt";
        public ClinicalConsultationConfigurationModel(ConfigurationOptions options) : base(options)
        {
            this.ScopeId = (int)ConfigurationScopes.ClinicalConsultation;
            this.ApplicationDomainId = (int)ApplicationDomains.ProviderPortal;
        }


        public int ClinicalConsultationHistoryPageSize => GetConfigValue(ConfigurationConstants.CLINICAL_CONSULTATION_HISTORY_SEARCH_PAGE_SIZE, 10);
        //TODO change default date time
        public DateTime? CreateButtonProviderSpecialistStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_SPECIALIST_STARTDATE,formatForDateTime, null);
        public DateTime? CreateButtonProviderSpecialistEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_SPECIALIST_ENDDATE,formatForDateTime, null);
        public string CreateButtonProviderSpecialistMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_SPECIALIST_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonProviderPCPStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_PCP_STARTDATE,formatForDateTime, null);
        public DateTime? CreateButtonProviderPCPEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_PCP_ENDDATE, formatForDateTime, null);
        public string CreateButtonProviderPCPMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_PCP_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonAdministrationGroupIPAStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_IPA_STARTDATE,formatForDateTime,null);
        public DateTime? CreateButtonAdministrationGroupIPAEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_IPA_ENDDATE, formatForDateTime, null);
        public string CreateButtonAdministrationGroupIPAMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_IPA_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonAdministrationGroupSIPAStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_SIPA_STARTDATE,formatForDateTime,null);
        public DateTime? CreateButtonAdministrationGroupSIPAEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_SIPA_ENDDATE, formatForDateTime, null);
        public string CreateButtonAdministrationGroupSIPAMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_SIPA_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonAdministrationGroupPMGStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_PMG_STARTDATE,formatForDateTime,null);
        public DateTime? CreateButtonAdministrationGroupPMGEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_PMG_ENDDATE, formatForDateTime, null);
        public string CreateButtonAdministrationGroupPMGMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_PMG_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonAdministrationGroupSPMGStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_SPMG_STARTDATE,formatForDateTime,null);
        public DateTime? CreateButtonAdministrationGroupSPMGEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_SPMG_ENDDATE, formatForDateTime, null);
        public string CreateButtonAdministrationGroupSPMGMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_SPMG_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonAdministrationGroupMSOStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_MSO_STARTDATE,formatForDateTime,null);
        public DateTime? CreateButtonAdministrationGroupMSOEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_MSO_ENDDATE, formatForDateTime, null);
        public string CreateButtonAdministrationGroupMSOMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_ADMINISTRATIONGROUP_MSO_MESSAGE, string.Empty);


        //TODO change default date time
        public DateTime? CreateButtonProviderGroupPracticeStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_GROUP_PRACTICE_STARTDATE, formatForDateTime, null);
        public DateTime? CreateButtonProviderGroupPracticeEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_GROUP_PRACTICE_ENDDATE, formatForDateTime, null);
        public string CreateButtonProviderGroupPracticeMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_GROUP_PRACTICE_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonProviderAncillaryStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_ANCILLARY_STARTDATE, formatForDateTime, null);
        public DateTime? CreateButtonProviderAncillaryEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_ANCILLARY_ENDDATE, formatForDateTime, null);
        public string CreateButtonProviderAncillaryMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_ANCILLARY_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonProviderHospitalStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_HOSPITAL_STARTDATE, formatForDateTime, null);
        public DateTime? CreateButtonProviderHospitalEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_HOSPITAL_ENDDATE, formatForDateTime, null);
        public string CreateButtonProviderHospitalMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_HOSPITAL_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonProviderPharmacyStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_PHARMACY_STARTDATE, formatForDateTime, null);
        public DateTime? CreateButtonProviderPharmacyEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_PHARMACY_ENDDATE, formatForDateTime, null);
        public string CreateButtonProviderPharmacyMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_PHARMACY_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonProviderDentalStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_DENTAL_STARTDATE, formatForDateTime, null);
        public DateTime? CreateButtonProviderDentalEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_DENTAL_ENDDATE, formatForDateTime, null);
        public string CreateButtonProviderDentalMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_DENTAL_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonProviderMentalHealthCareStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_MENTAL_HEALTH_CARE_STARTDATE, formatForDateTime, null);
        public DateTime? CreateButtonProviderMentalHealthCareEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_MENTAL_HEALTH_CARE_ENDDATE, formatForDateTime, null);
        public string CreateButtonProviderMentalHealthCareMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_MENTAL_HEALTH_CARE_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonProviderCampStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_CAMP_STARTDATE, formatForDateTime, null);
        public DateTime? CreateButtonProviderCampEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_CAMP_ENDDATE, formatForDateTime, null);
        public string CreateButtonProviderCampMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PROVIDER_CAMP_MESSAGE, string.Empty);
        //TODO change default date time
        public DateTime? CreateButtonPayerRepresentativeStartDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PAYER_REPRESENTATIVE_STARTDATE, formatForDateTime, null);
        public DateTime? CreateButtonPayerRepresentativeEndDate => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PAYER_REPRESENTATIVE_ENDDATE, formatForDateTime, null);
        public string CreateButtonPayerRepresentativeMessage => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_PAYER_REPRESENTATIVE_MESSAGE, string.Empty);

        public int SearchRequestingProviderPageSize => GetConfigValue(ConfigurationConstants.SEARCH_REQUESTING_PROVIDER_PAGE_SIZE, 10);

        public string CancelCreateConsultationMessage => GetConfigValue(ConfigurationConstants.CANCEL_CREATE_CONSULTATION_MESSAGE, "");

        public string CreateDisabledGlobalMessage => GetConfigValue(ConfigurationConstants.CREATE_DISABLED_GLOBAL_MESSAGE, "");

        public int MaxNumberOfRecentDiagnoses => GetConfigValue(ConfigurationConstants.MAX_NUMBER_OF_RECENT_DIAGNOSES, 6);

        public int SearchDiagnosisPageSize => GetConfigValue(ConfigurationConstants.SEARCH_DIAGNOSIS_PAGE_SIZE, 10);

        public bool AllowProcedureBundleAutoSelect => GetConfigValue(ConfigurationConstants.ALLOW_PROCEDURE_BUNDLE_AUTO_SELECT, false);

        public string NoPCPMessage => GetConfigValue(ConfigurationConstants.NO_PCP_MESSAGE, "");

        public string SERVICE_QUANTITY_MAXIMUM => GetConfigValue(ConfigurationConstants.SERVICE_QUANTITY_MAXIMUM, "");
        public string SERVICE_QUANTITY_REQUIRED => GetConfigValue(ConfigurationConstants.SERVICE_QUANTITY_REQUIRED, ""); 
        public string SERVICE_QUANTITY_MINIMUM => GetConfigValue(ConfigurationConstants.SERVICE_QUANTITY_MINIMUM, "");
        public string CreateConsultationCompleteStepsValidationMsg => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_COMPLETE_STEPS_VALIDATION_MSG, "");
        public int SearchServicingProviderPageSize => GetConfigValue(ConfigurationConstants.SEARCH_SERVICING_PROVIDER_PAGE_SIZE, 10);
        public int CreateConsultationDaysBackAllowed => GetConfigValue(ConfigurationConstants.CONSULTATION_DAYS_BACK_ALLOWED, 5);
        public string CreateRequestConsultationMaximumAllowedMessage => GetConfigValue(ConfigurationConstants.REQUEST_CONSULTATION_MAXIMUM_ALLOWED_MESSAGE, "");
        public int CreateRequestConsultationMaximumAllowedValue => GetConfigValue(ConfigurationConstants.REQUEST_CONSULTATION_MAXIMUM_ALLOWED_VALUE, 500);
        public string RuleOutsMessage => GetConfigValue(ConfigurationConstants.RULES_OUT_MESSAGE, "");
        public string PPN_REASON => GetConfigValue(ConfigurationConstants.PPN_REASON, "");
        public string CreateConsultationSubmitValidationMsg => GetConfigValue(ConfigurationConstants.CREATE_CONSULTATION_SUBMIT_VALIDATION_MSG, "");
        public int CreateClinicalConsultationExpirationDate => GetConfigValue(ConfigurationConstants.PROVIDER_PORTAL_CONSULTATION_EXPIRATION_DATE, 90);

    }
}
