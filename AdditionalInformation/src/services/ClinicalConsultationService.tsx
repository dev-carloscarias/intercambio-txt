import API from './ApiConfiguration';

export default class ClinicalConsultationService {
    private static instance: ClinicalConsultationService;

    private constructor() {}

    public static getInstance(): ClinicalConsultationService {
        if (!ClinicalConsultationService.instance) {
            ClinicalConsultationService.instance =
                new ClinicalConsultationService();
        }

        return ClinicalConsultationService.instance;
    }

    GetBeneficiaryInformation(beneficiaryId: string) {
        return API.get<BeneficiaryInformation>(
            `api/beneficiary/information/${beneficiaryId}`
        );
    }

    getConfigurations() {
        return API.get<ConsultationConfigurations>(
            `api/clinicalconsultation/configurations`
        );
    }

    getHealthPlans() {
        return API.get<HealthPlanDto[]>(`api/clinicalconsultation/healthplans`);
    }

    logAuditEvent(
        auditEventType: string,
        auditEventGroup: string,
        eventData?: any,
        requestUrl?: string
    ) {
        return API.post(
            `api/Log/audit`,
            {
                auditEventType: auditEventType,
                auditEventGroup: auditEventGroup,
                eventData: eventData,
                requestUrl: requestUrl
            },
            {
                headers: {
                    'Content-Type': 'application/json'
                }
            }
        );
    }
}

export interface BeneficiaryInformation {
    displayName: string;
    beneficiaryId: number;
    beneficiaryIdProtected: string;
    cardNumber: string;
    identifier: string;
    lineOfBusinessId: number;
    lineOfBusinessIdProtected: string;
}

export interface ConsultationConfigurations {
    consultationDaysBackAllowed: number;
    createRequestConsultationMaximumAllowedMessage: string;
    requestConsultationMaximumAllowedValue: number;
}

export type HealthPlanDto = {
    additionalHealthPlanId: number;
    additionalHealthPlanName: string;
    additionalHealthPlanIdProtected?: string;
};
