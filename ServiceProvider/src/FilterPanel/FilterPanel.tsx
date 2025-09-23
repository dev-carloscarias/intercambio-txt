import React, { useEffect, useState } from 'react';
import './FilterPanel.scss';
import { Card } from 'reactstrap';
import { SidePanel, BottomSheetSelect } from '@provider-portal/components';
import { IconClose } from '@provider-portal/icons';
import { fetchFilter } from '../mock-data';
import { FiltersInterface } from '../models/FiltersInterface';

const HeadingDesktop = ({ onClose }) => (
    <div className="filter-panel-heading">
        <div className="filter-panel-modal-title">Filter</div>
        <div
            className="filter-panel-modal-close"
            onClick={onClose}
            role="button"
            tabIndex={0}
        >
            <IconClose width="1.5rem" height="1.5rem" />
        </div>
    </div>
);

export type FilterPanelProps = {
    isOpen: boolean;
    onClose: () => void;
    searchTerm: string;
    onApply: (f: FiltersInterface, c: number) => void;
    totalResults: number;
    id: string;
};

const FilterPanel: React.FC<FilterPanelProps> = ({
    isOpen,
    onClose,
    searchTerm,
    onApply,
    totalResults,
    id
}) => {
    const [filters, setFilters] = useState({});
    const [filtersCount, setFiltersCount] = useState(0);
    const [SpecialtiesList, setSpecialtiesList] = useState([]);
    const [CitiesList, setCitiesList] = useState([]);
    const [GroupsList, setGroupsList] = useState([]);
    const [CountriesList, setCountriesList] = useState([]);
    const [ZipCodesList, setZipCodesList] = useState([]);

    const onSelectFilter = (key: string) => (value: string) => {
        setFilters({ ...filters, [key]: value });
    };

    const handleOnReset = () => setFilters({});

    const handleOnApply = () => {
        onApply(filters, filtersCount);
    };

    useEffect(() => {
        if (filters)
            setFiltersCount(
                Object.keys(filters).filter(k => filters[k] != '').length
            );
    }, [filters]);

    const loadFilterValues = async () => {
        setSpecialtiesList(await fetchFilter('Specialties'));
        setCitiesList(await fetchFilter('Cities'));
        setGroupsList(await fetchFilter('Groups'));
        setCountriesList(await fetchFilter('Countries'));
        setZipCodesList(await fetchFilter('ZipCodes'));
    };

    useEffect(() => {
        loadFilterValues();
    }, []);

    return (
        <SidePanel className="filter-panel" isOpen={isOpen} toggle={onClose}>
            <Card className="card-filter-panel ">
                <HeadingDesktop onClose={onClose} />
                <div className="card-filter-panel-content">
                    <div className="card-filter-panel-content-search">
                        Searching for: "<strong>{searchTerm}</strong>"
                    </div>
                    <div className="card-filter-panel-content-applied">
                        Applied Filters ( {filtersCount} )
                    </div>
                </div>
                <div className="mt-3">
                    <BottomSheetSelect
                        title={'Specialties'}
                        selected={filters['specialty']}
                        items={SpecialtiesList}
                        onSelect={onSelectFilter('specialty')}
                        placeholder="Specialist"
                        filterable={true}
                    />
                </div>
                <div className="mt-3">
                    <BottomSheetSelect
                        title={'Cities'}
                        selected={filters['city']}
                        items={CitiesList}
                        onSelect={onSelectFilter('city')}
                        placeholder="Cities"
                        filterable={true}
                    />
                </div>
                <div className="mt-3">
                    <BottomSheetSelect
                        title={'Administration Group'}
                        selected={filters['group']}
                        items={GroupsList}
                        onSelect={onSelectFilter('group')}
                        placeholder="Administration Group"
                        filterable={true}
                    />
                </div>
                <div className="mt-3">
                    <BottomSheetSelect
                        title={'Country'}
                        selected={filters['country']}
                        items={CountriesList}
                        onSelect={onSelectFilter('country')}
                        placeholder="Country"
                        filterable={true}
                    />
                </div>
                <div className="mt-3">
                    <BottomSheetSelect
                        title={'Zip Code'}
                        selected={filters['zipCode']}
                        items={ZipCodesList}
                        onSelect={onSelectFilter('zipCode')}
                        placeholder="Zip Code"
                        filterable={true}
                    />
                </div>
                <div className="mt-auto">
                    <div className="text-sm text-muted mb-3 text-center">
                        Results {totalResults} specialists
                    </div>
                    <div className="card-filter-panel-footer">
                        <div className="d-flex aling-items-center justify-content-between pb-1">
                            <button
                                type="button"
                                className="btn btn-link px-3"
                                style={{ minWidth: '130px' }}
                                onClick={handleOnReset}
                            >
                                Reset Filter
                            </button>
                            <button
                                type="button"
                                className="btn btn btn-info btn-info-gradient rounded-pill px-3"
                                style={{ minWidth: '130px' }}
                                onClick={handleOnApply}
                            >
                                Apply
                            </button>
                        </div>
                    </div>
                </div>
            </Card>
        </SidePanel>
    );
};

export default FilterPanel;
