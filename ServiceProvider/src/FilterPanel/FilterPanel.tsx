import React, { useEffect, useState } from 'react';
import './FilterPanel.scss';
import { Card } from 'reactstrap';
import { SidePanel, BottomSheetSelect } from '@provider-portal/components';
import { IconAngleLeft } from '@provider-portal/icons';
import { FiltersInterface } from '../models/FiltersInterface';
import ClinicalConsultationService, {
    AdministrationGroup,
    County,
    Specialty,
    State,
    ZipCode
} from '../services/ClinicalConsultationService';
import { useTranslation } from 'react-i18next';

const HeadingDesktop = ({ onClose }) => (
    <div className="filter-panel-heading">
        <div className="filter-panel-modal-title">Filter</div>
        <div
            className="filter-panel-modal-close-custom"
            onClick={onClose}
            role="button"
            tabIndex={0}
        >
            <IconAngleLeft width="1.5rem" height="1.5rem" />
            Back
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
    beneficiaryId: string;
    resetInput: () => void;
    specialtiesPrev: Specialty[];
    specialtyPrevSelected: Specialty;
    handleSpecialty: (item: Specialty) => void;
};

const FilterPanel: React.FC<FilterPanelProps> = ({
    isOpen,
    onClose,
    searchTerm,
    onApply,
    totalResults,
    id,
    beneficiaryId,
    resetInput,
    specialtiesPrev,
    specialtyPrevSelected,
    handleSpecialty
}) => {
    const { t } = useTranslation();
    const [filters, setFilters] = useState({});
    const [filtersCount, setFiltersCount] = useState(0);

    const [states, setStates] = useState<State[]>();
    const [counties, setCounties] = useState<County[]>();
    const [zipCodes, setZipCodes] = useState<ZipCode[]>();
    const [adminGroups, setAdminGroups] = useState<AdministrationGroup[]>();

    const [selectedState, setSelectedState] = useState<State>();
    const [selectedCounty, setSelectedCounty] = useState<County>();
    const [selectedZipCode, setSelectedZipCode] = useState<ZipCode>();
    const [selectedAdminGroup, setSelectedAdminGroup] =
        useState<AdministrationGroup>();

    const [defaultState, setDefaultState] = useState<State>();

    const [specialties, setSpecialties] =
        useState<Specialty[]>(specialtiesPrev);
    const [selectedSpecialty, setSelectedSpecialty] = useState<Specialty>();

    const handleOnReset = () => {
        setSelectedAdminGroup(null);
        setSelectedCounty(null);
        setSelectedState(defaultState);
        setSelectedZipCode(null);
        setSelectedSpecialty(null);
        resetInput();
        setFiltersCount(1);
    };
    const handleOnApply = () => {
        applyFilters();
        onApply(filters, filtersCount);
    };
    const handleOnSelectAdminGroup = (adminGroup: AdministrationGroup) => {
        setSelectedAdminGroup(adminGroup);
    };
    const handleOnSelectState = (state: State) => {
        setSelectedState(state);
    };
    const handleOnSelectCounty = (county: County) => {
        setSelectedCounty(county);
        setSelectedZipCode(null);
    };
    const handleOnSelectZipCode = (zipCode: ZipCode) => {
        setSelectedZipCode(zipCode);
    };
    const handleOnSelectSpecialty = (specialty: Specialty) => {
        if (!selectedSpecialty) {
        }
        setSelectedSpecialty(specialty);
        handleSpecialty(specialty);
    };

    const filteredCounty = () => {
        if (!selectedZipCode) {
            return counties;
        } else {
            return counties?.filter(
                c => c.countyId === selectedZipCode.countyId
            );
        }
    };

    const filteredZipCode = () => {
        if (!selectedCounty) {
            return zipCodes;
        } else {
            return zipCodes.filter(c => c.countyId === selectedCounty.countyId);
        }
    };

    useEffect(() => {
        if (filters) {
            const filtersCount = Object.keys(filters).filter(
                k =>
                    filters[k] != '' &&
                    filters[k] !== undefined &&
                    filters[k] !== null
            ).length;
            setFiltersCount(filtersCount);
            if (filtersCount) onApply(filters, filtersCount);
        }
    }, [filters]);

    useEffect(() => {
        if (filteredZipCode()?.length == 1) {
            setSelectedZipCode(filteredZipCode()[0]);
        }
    }, [selectedCounty]);

    useEffect(() => {
        if (filteredCounty()?.length == 1) {
            setSelectedCounty(filteredCounty()[0]);
        }
    }, [selectedZipCode]);

    useEffect(() => {
        ClinicalConsultationService.getInstance()
            .GetServicingProviderFilters(beneficiaryId, null)
            .then(response => {
                if (response) {
                    //setSpecialties(response.data.specialty);
                    setCounties(response.data.county);
                    setStates(response.data.state);
                    setAdminGroups(response.data.administrationGroup);
                    setZipCodes(response.data.zipCode);
                    setDefaultState(response.data.state[0]);
                    setSelectedState(response.data.state[0]);
                    setFiltersCount(filtersCount + 1);
                }
            });
    }, []);

    useEffect(() => {
        if (specialtyPrevSelected) {
            setSelectedSpecialty(specialtyPrevSelected);
            setFiltersCount(filtersCount + 1);
        }
    }, [specialtyPrevSelected]);

    const applyFilters = () => {
        setFilters({
            city: selectedCounty,
            group: selectedAdminGroup,
            country: selectedState,
            zipCode: selectedZipCode,
            specialties: selectedSpecialty
        });
    };
    return (
        <SidePanel className="filter-panel" isOpen={isOpen} toggle={onClose}>
            <Card className="card-filter-panel ">
                <HeadingDesktop onClose={onClose} />
                <div className="card-filter-panel-content">
                    <div className="card-filter-panel-content-search">
                        {t(
                            'clinicalconsultation:servicing-provider.SEARCHING-FOR'
                        )}
                        "<strong>{searchTerm}</strong>"
                    </div>
                    <div className="card-filter-panel-content-applied">
                        {t(
                            'clinicalconsultation:servicing-provider.APPLIED-FILTERS'
                        )}{' '}
                        ( {filtersCount} )
                    </div>
                </div>
                <div className="mt-3">
                    <BottomSheetSelect
                        title={t(
                            'clinicalconsultation:servicing-provider.ADMINISTRATION-GROUP'
                        )}
                        selected={selectedAdminGroup}
                        items={adminGroups}
                        onSelect={handleOnSelectAdminGroup}
                        formatItem={(a: AdministrationGroup) => a.name}
                        formatSelected={(a: AdministrationGroup) => a.name}
                        placeholder={t(
                            'clinicalconsultation:servicing-provider.ADMINISTRATION-GROUP'
                        )}
                        id={`${id}-group`}
                        filterable={true}
                        blockRegex={/[^A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s]/g}
                    />
                </div>
                <div className="mt-3">
                    <BottomSheetSelect
                        title={t(
                            'clinicalconsultation:servicing-provider.COUNTRY'
                        )}
                        selected={selectedState ? selectedState : defaultState}
                        items={states}
                        onSelect={handleOnSelectState}
                        formatItem={(s: State) => s.name}
                        formatSelected={(s: State) => s.name}
                        placeholder={t(
                            'clinicalconsultation:servicing-provider.COUNTRY'
                        )}
                        id={`${id}-country`}
                        filterable={true}
                        blockRegex={/[^A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s]/g}
                    />
                </div>
                <div className="mt-3">
                    <BottomSheetSelect
                        title={t(
                            'clinicalconsultation:servicing-provider.CITIES'
                        )}
                        selected={selectedCounty}
                        items={counties}
                        onSelect={handleOnSelectCounty}
                        formatItem={(c: County) => c.name}
                        formatSelected={(c: County) => c.name}
                        placeholder={t(
                            'clinicalconsultation:servicing-provider.CITIES'
                        )}
                        filterable={true}
                        id={`${id}-cities`}
                        blockRegex={/[^A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s]/g}
                    />
                </div>
                <div className="mt-3">
                    <BottomSheetSelect
                        title={t(
                            'clinicalconsultation:servicing-provider.ZIPCODE'
                        )}
                        selected={selectedZipCode}
                        items={filteredZipCode()}
                        onSelect={handleOnSelectZipCode}
                        formatItem={(z: ZipCode) => z.zipCodeValue}
                        formatSelected={(z: ZipCode) => z.zipCodeValue}
                        placeholder={t(
                            'clinicalconsultation:servicing-provider.ZIPCODE'
                        )}
                        filterable={true}
                        id={`${id}-zipcode`}
                        blockRegex={/\D/g}
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
                                {t(
                                    'clinicalconsultation:servicing-provider.RESET-FILTER'
                                )}
                            </button>
                            <button
                                type="button"
                                className="btn btn btn-info btn-info-gradient rounded-pill px-3"
                                style={{ minWidth: '130px' }}
                                onClick={handleOnApply}
                            >
                                {t(
                                    'clinicalconsultation:servicing-provider.APPLY'
                                )}
                            </button>
                        </div>
                    </div>
                </div>
            </Card>
        </SidePanel>
    );
};

export default FilterPanel;
