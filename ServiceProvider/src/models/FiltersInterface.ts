import {
    AdministrationGroup,
    County,
    Specialty,
    State,
    ZipCode
} from '../services/ClinicalConsultationService';

export interface FiltersInterface {
    specialty?: Specialty;
    city?: County;
    group?: AdministrationGroup;
    country?: State;
    zipCode?: ZipCode;
}
