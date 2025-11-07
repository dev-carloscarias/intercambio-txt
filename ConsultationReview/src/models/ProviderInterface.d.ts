import { CityInterface } from './CityInterface';
import { SpecialtyInterface } from './SpecialtyInterface';

interface ProviderInterface {
    providerAffiliationId?: number;
    renderingProviderId: number;
    renderingProviderNPI: string;
    renderingProviderName: string;
    phoneNumber: string;
    email: string;
    specialties: SpecialtyInterface[];
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
    anyContractedSpecialist?: boolean;
}

export interface ConsultationProvider extends ProviderInterface {}

export interface RequestingProvider extends ProviderInterface {}
