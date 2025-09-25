import React, { useEffect, useState } from 'react';
import './FilterBar.scss';
import { Collapse } from 'reactstrap';
import { fetchFilter } from '../mock-data';

import { DropdownPicker } from '@provider-portal/components';
import { FiltersInterface } from '../models/FiltersInterface';
import { useTranslation } from 'react-i18next';

export type FilterBarProps = {
    isOpen: boolean;
    onApply: (f: FiltersInterface, c: number) => void;
    disabled: boolean;
    id: string;
};

const FilterBar: React.FC<FilterBarProps> = ({
    isOpen,
    onApply,
    disabled,
    id
}) => {
    const { t } = useTranslation();
    const [filters, setFilters] = useState({});
    const [CitiesList, setCitiesList] = useState([]);
    const [GroupsList, setGroupsList] = useState([]);
    const [CountriesList, setCountriesList] = useState([]);
    const [ZipCodesList, setZipCodesList] = useState([]);

    const onSelectFilter = (key: string) => (value: string) => {
        setFilters({ ...filters, [key]: value });
    };

    useEffect(() => {
        if (filters) {
            const filtersCount = Object.keys(filters).filter(
                k => filters[k] != ''
            ).length;
            if (filtersCount) onApply(filters, filtersCount);
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [filters]);

    const loadFilterValues = async () => {
        setCitiesList(await fetchFilter('Cities'));
        setGroupsList(await fetchFilter('Groups'));
        setCountriesList(await fetchFilter('Countries'));
        setZipCodesList(await fetchFilter('ZipCodes'));
    };

    useEffect(() => {
        loadFilterValues();
    }, []);

    console.log('FilterBar component rendering, isOpen:', isOpen);
    
    return (
        <div style={{ display: isOpen ? 'block' : 'none' }}>
            <div className="filter-bar">
                <DropdownPicker
                    title={t('clinicalconsultation:servicing-provider.CITIES')}
                    selected={filters['city']}
                    items={CitiesList}
                    onSelect={onSelectFilter('city')}
                    placeholder={t(
                        'clinicalconsultation:servicing-provider.CITIES'
                    )}
                    disabled={disabled}
                    id={`${id}-cities`}
                />
                <DropdownPicker
                    title={t(
                        'clinicalconsultation:servicing-provider.ADMINISTRATION-GROUP'
                    )}
                    selected={filters['group']}
                    items={GroupsList}
                    onSelect={onSelectFilter('group')}
                    placeholder={t(
                        'clinicalconsultation:servicing-provider.ADMINISTRATION-GROUP'
                    )}
                    disabled={disabled}
                    id={`${id}-group`}
                />
                <DropdownPicker
                    title={t('clinicalconsultation:servicing-provider.COUNTRY')}
                    selected={filters['country']}
                    items={CountriesList}
                    onSelect={onSelectFilter('country')}
                    placeholder={t(
                        'clinicalconsultation:servicing-provider.COUNTRY'
                    )}
                    disabled={disabled}
                    id={`${id}-country`}
                />
                <DropdownPicker
                    title={t('clinicalconsultation:servicing-provider.ZIPCODE')}
                    selected={filters['zipCode']}
                    items={ZipCodesList}
                    onSelect={onSelectFilter('zipCode')}
                    placeholder={t(
                        'clinicalconsultation:servicing-provider.ZIPCODE'
                    )}
                    disabled={disabled}
                    id={`${id}-zipcode`}
                />
            </div>
        </div>
    );
};

export default FilterBar;
