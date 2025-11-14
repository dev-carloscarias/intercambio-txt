using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.InnovaMD.Provider.Data.ClinicalConsultations
{
    public static class Columns
    {
        public static class ClinicalConsultation
        {
            public static int CLINICAL_CONSULTATION_NUMBER = 20;
            public static int ADDITIONAL_HEALTH_PLAN_NAME = 100;
            public static int PURPOSE = 500;
            public static int CREATED_BY = 150;
            public static int SOURCE_IDENTIFIER = 3;
        }

        public static class ClinicalConsultationBeneficiary
        {
            public static int CARD_NUMBER = 50;
            public static int NAME = 152;
            public static int FIRST_NAME = 50;
            public static int MIDDLE_NAME = 50;
            public static int LAST_NAME = 50;
            public static int LINE_OF_BUSINESS_SHORT_NAME = 10;
            public static int HEALTH_PLAN_NAME = 100;
            public static int PRODUCT_NAME = 100;
            public static int BRAND = 10;
            public static int IDENTIFIER = 50;
        }
        
        public static class ClinicalConsultationBeneficiaryAddress
        {
            public static int ADDRESS_LINE1 = 100;
            public static int ADDRESS_LINE2 = 100;
            public static int ADDRESS_LINE3 = 100;
            public static int CITY = 100;
            public static int STATE = 2;
            public static int PLACE = 100;
            public static int ZIPCODE = 5;
            public static int ZIP4CODE = 4;
            public static int COUNTRY_FIPS_CODE = 2;
        }
        public static class ClinicalConsultationBeneficiaryPhone
        {
            public static int COUNTRY_CODE = 5;
            public static int AREA_CODE = 5;
            public static int EXCHANGE = 5;
            public static int NUMBER = 6;
            public static int PHONE_NUMBER = 20; 
        }
        public static class ClinicalConsultationDiagnosis
        {
            public static int CODE = 50;
            public static int DESCRIPTION = 300;
        }

        public static class ClinicalConsultationProcedureBundle
        {
           public static int DESCRIPTION = 100; 
           public static int REFERENCE_CODE = 20;
           public static int SERVICE_TYPE_CODE = 20;
        }

        public static class ClinicalConsultationProvider
        {
           public static int RENDERING_NPI = 10; 
           public static int BILLING_NPI = 10;
           public static int NAME = 152;
           public static int FIRST_NAME = 25;
           public static int MIDDLE_NAME = 25;
           public static int LAST_NAME = 25;
           public static int FULL_ADDRESS = 200;
           public static int ADDRESS_LINE1 = 100;
           public static int ADDRESS_LINE2 = 100;
           public static int COUNTY_NAME = 50;
           public static int STATE_NAME = 50;
           public static int ZIP_CODE = 5;
           public static int LOCATION_CODE_ADDRESS = 100;
           public static int LOCATION_ADDRESS = 200;
           public static int PHONE_NUMBER = 15;
           public static int FAX_NUMBER = 15;
           public static int EMAIL = 100;
           public static int ADMINISTRATION_GROUP_NAME = 100;
        }

        public static class ClinicalConsultationProviderSpecialty
        {
            public static int NAME = 100;
        }

        public static class ClinicalConsultationServicingNonPPNReason
        {
            public static int NO_PPN_REASON_DESCRIPTION = 200;
        }
    }
}
