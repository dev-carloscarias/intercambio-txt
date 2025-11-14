import { FiltersInterface } from '../models/FiltersInterface';
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

    getPPNConfigurations() {
        return API.get<ConsultationConfigurations>(
            `api/createclinicalconsultation/ppnreasons/configurations`
        );
    }

    GetBeneficiaryInformation(beneficiaryId: string) {
        return API.get<BeneficiaryInformation>(
            `api/beneficiary/information/${beneficiaryId}`
        );
    }

    GetServicingNonPPNReason(beneficiaryId: string) {
        return API.get<ServicingNonPPNReasonResponse>(
            `api/createclinicalconsultation/servicingprovider/servicingNonPPNReason/${beneficiaryId}`
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
                search: searchTerm,
                specialty: filters?.specialty?.specialtyIdProtected,
                city: filters?.city?.countyIdProtected,
                administrationGroup:
                    filters?.group?.administrationGroupIdProtected,
                country: filters?.country?.stateIdProtected,
                zipCode: filters?.zipCode?.zipCodeValueProtected
            },
            { signal: signal }
        );
    }

    GetServicingProviderFilters(beneficiaryId: string, CountryId: number) {
        return API.get<ServicingProviderFiltersResponse>(
            `api/createclinicalconsultation/servicingprovider/filters`,
            {
                params: { beneficiaryId: beneficiaryId, Country: CountryId }
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
export interface ServicingNonPPNReasonResponse {
    servicingNonPPNReasons: ServicingNonPPNReasonInterface[];
}
export interface ServicingNonPPNReasonInterface {
    servicingNonPPNReasonId: number;
    servicingNonPPNReasonIdProtected: string;
    lineOfBusinessId: number;
    description: string;
}
export interface ConsultationConfigurations {
    consultationDaysBackAllowed: number;
    createRequestConsultationMaximumAllowedMessage: string;
    requestConsultationMaximumAllowedValue: number;
    ruleOutsMessage: string;
    ppnReason: string;
}
export interface ServicingProviderFiltersResponse {
    specialty: Specialty[];
    state: State[];
    county: County[];
    zipCode: ZipCode[];
    administrationGroup: AdministrationGroup[];
}
export interface Specialty {
    specialtyId: number;
    name: string;
    taxonomyCode?: string;
    allowAnyContractedSpecialist?: boolean;
    defaultRoleId?: number;
    providerEntityTypeId?: number;
    isDirectoryDisplay?: boolean;
    specialtyIdProtected: string;
}

export interface State {
    stateId: number;
    countryId: number;
    fipsCode?: string;
    uspsCode?: string;
    name: string;
    gnisId?: number;
    stateIdProtected: string;
}

export interface County {
    countyId: number;
    stateId: number;
    countryId: number;
    fipsCode?: string;
    uspsCode?: string;
    name: string;
    gnisId?: number;
    countyIdProtected: string;
}

export interface ZipCode {
    zipCodeId: number;
    countryId: number;
    countyId: number;
    stateId: number;
    zipCodeValue: string;
    latitude: number;
    longitude: number;
    region: string;
    regionEnglish: string;
    regionSpanis: string;
    zipCodeValueProtected: string;
}

export interface AdministrationGroup {
    administrationGroupId: number;
    administrationGroupTypeId: number;
    name: string;
    npi?: string;
    number?: string;
    code?: string;
    isActive?: boolean;
    administrationGroupClassificationId?: number;
    lineOfBusinessId?: number;
    administrationGroupIdProtected: string;
}
