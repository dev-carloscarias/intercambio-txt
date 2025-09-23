export interface FiltersInterface {
    specialty?: string;
    city?: string;
    group?: string;
    country?: string;
    zipCode?: string;
}

export function hasFilters(filters: FiltersInterface): boolean {
    for (const key of Object.keys(filters) as (keyof FiltersInterface)[]) {
        const value = filters[key];
        if (value !== null && value !== undefined && value !== '') {
            return true;
        }
    }
    return false;
}
