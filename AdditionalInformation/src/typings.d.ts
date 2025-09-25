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
    name: string;
    zipCode: string;
}

interface ProviderInterface {
    npi: string;
    name: string;
    phone: string;
    email: string;
    specialties: string[];
    billing?: string;
    facility?: string;
    city?: CityInterface[];
    selectedCity?: CityInterface;
    adminGroup?: string;
}

interface ProcedureInterface {
    code: string;
    description: string;
}

interface ProceduresInterface extends Array<ProcedureInterface> {}

interface DiagnosisInterface {
    code: string;
    description: string;
    isPrimary?: boolean;
}

interface DiagnosesInterface extends Array<DiagnosisInterface> {}

interface ConsultationProvider extends ProviderInterface {}

interface RequestingProvider extends ProviderInterface {}

interface Consultation {
    requestingProvider?: {
        billing: RequestingProvider;
        rendering: RequestingProvider;
    };
    consultationProvider?: ConsultationProvider;
    diagnoses: DiagnosesInterface;
    procedure: ProcedureInterface;
    date: Date;
    healthPlan: string;
    reason: string;
}
