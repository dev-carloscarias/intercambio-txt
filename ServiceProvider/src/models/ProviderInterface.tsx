export interface CityInterface {
    providerAffiliationId?: number;
    cityId: number;
    cityIdProtected: string;
    name: string;
    zipCode: string;
}

export interface ProviderInterface {
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
}

export interface SpecialtyInterface {
    specialtyId: number;
    name: string;
    isPrimarySpecialty: boolean;
    providerAffiliationId?: number;
}

export interface ServicingProviderInterface extends ProviderInterface {
    isStarred?: boolean;
    isFeatured?: boolean;
    information?: string;
}
