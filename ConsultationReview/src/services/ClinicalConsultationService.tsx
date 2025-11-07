import { BeneficiaryInformation } from '../models/BeneficiaryInformation';
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
