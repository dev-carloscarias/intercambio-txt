import { FiltersInterface } from '../models/FiltersInterface';
import {
    ProviderInterface,
    ServicingProviderInterface
} from '../models/ProviderInterface';
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

    SearchServicingProvider(
        beneficiaryId: string,
        searchTerm: string,
        page: number,
        filters: FiltersInterface,
        signal: any
    ) {
        return API.post<RequestingProviderSearchResponse>(
            `api/createclinicalconsultation/servicingprovider/search`,
            {
                beneficiaryId: beneficiaryId,
                page: page,
                search: searchTerm
                //TODO : include filters
            },
            { signal: signal }
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

export interface RequestingProviderSearchResponse {
    total: number;
    servicingProviders: ServicingProviderInterface[];
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
