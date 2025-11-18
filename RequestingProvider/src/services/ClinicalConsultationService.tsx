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

    GetRequestingProvider(beneficiaryId: string) {
        return API.get<RequestingProviderOptionsResponse>(
            `api/createclinicalconsultation/requestingprovider/options/${beneficiaryId}`
        );
    }

    GetBeneficiaryInformation(beneficiaryId: string) {
        return API.get<BeneficiaryInformation>(
            `api/beneficiary/information/${beneficiaryId}`
        );
    }

    SearchRequestingProvider(
        beneficiaryId: string,
        page: number,
        searchTerm: string
    ) {
        return API.post<RequestingProviderSearchResponse>(
            `api/createclinicalconsultation/requestingprovider/search`,
            {
                beneficiaryId: beneficiaryId,
                page: page,
                search: searchTerm
            }
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

export interface RequestingProviderOptionsResponse {
    allowSearch: boolean;
    beneficiaryPcp: ProviderInterface;
    requestingProvider: RequestingProvider;
    noPCP: boolean;
    noPCPMessage: string;
}

export interface RequestingProviderSearchResponse {
    total: number;
    requestingProviders: ProviderInterface[];
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
