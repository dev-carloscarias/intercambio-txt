declare module '*.scss' {
    export const content: { [className: string]: string };
    export default content;
}

declare module '*.png';
declare module '*.jpg';
declare module '*.svg';

declare module '@provider-portal/i18n';
declare module '@provider-portal/icons';
declare module '@provider-portal/components';
declare module '@provider-portal/oidc';

interface CityInterface {
    providerAffiliationId?: number;
    name: string;
    zipCode?: string;
    cityId?: number;
    cityIdProtected?: string;
}

interface ProviderInterface {
    providerAffiliationId?: number;
    renderingProviderId?: number;
    renderingProviderNPI?: string;
    renderingProviderName?: string;
    phoneNumber?: string;
    email?: string;
    specialties?: SpecialtyInterface[];
    selectedSpecialty?: SpecialtyInterface;
    billingProviderId?: number;
    billingProviderNPI?: string;
    billingProviderName?: string;
    providerLocationName?: string;
    facilityName?: string;
    cities?: CityInterface[];
    selectedCity?: CityInterface;
    renderingProviderIdProtected?: string;
    billingProviderIdProtected?: string;
    preferredNetwork?: boolean;
    anyContractedSpecialist?: boolean;
}

interface ProcedureInterface {
    code: string;
    procedureBundleId: number;
    description: string;
    lineOfBusinessId: number;
    defaultUnits: number;
    serviceRequestTypeId: number;
    minimumUnits: number;
    maximumUnits: number;
    referenceCode: string;
    startDate: Date;
    endDate: Date;
    sortOrder: number;
    serviceTypeCode: string;
    lineOfBusinessIdProtected: string;
    procedureBundleIdProtected: string;
}

interface ProceduresInterface {
    allowAutoSelect: boolean;
    procedures: Array<ProcedureInterface>;
    errorMessageRequired: string;
    errorMessageMaximum: string;
    errorMessageMinimum: string;
}

interface DiagnosisInterface {
    diagnosisId: number;
    diagnosisIdProtected: string;
    code: string;
    description: string;
    isPrimary?: boolean;
    isRecent?: boolean;
}

interface DiagnosesInterface extends Array<DiagnosisInterface> {}

interface ConsultationProvider extends ProviderInterface {}

interface RequestingProvider extends ProviderInterface {}

interface Consultation {
    requestingProvider: RequestingProvider;
    consultationProvider?: ConsultationProvider;
    diagnoses: DiagnosesInterface;
    procedure: ProcedureInterface;
    procedureQty: number;
    date: Date;
    healthPlan?: HealthPlanInterface;
    reason?: string;
    referralReason?: ServicingNonPPNReasonInterface;
    isRecreate?: boolean;
    originalConsultationId?: number;
    originalConsultationIdProtected?: string;
}

interface HealthPlanInterface {
    additionalHealthPlanId: number;
    additionalHealthPlanName: string;
    additionalHealthPlanIdProtected?: string;
}

interface ServicingNonPPNReasonInterface {
    servicingNonPPNReasonId: number;
    servicingNonPPNReasonIdProtected: string;
    lineOfBusinessId: number;
    description: string;
}

interface SpecialtyInterface {
    specialtyId: number;
    specialtyIdProtected: string;
    name: string;
    isPrimarySpecialty?: boolean;
    allowAnyContractedSpecialist?: boolean;
    providerAffiliationId?: number;
}

interface BeneficiaryInformation {
    displayName: string;
    beneficiaryId: number;
    beneficiaryIdProtected: string;
    cardNumber: string;
    identifier: string;
    lineOfBusinessId: number;
    lineOfBusinessIdProtected: string;
}
