import React, { useEffect, useState } from 'react';
import './FilterBar.scss';

import { FormDropdownSelect } from '@provider-portal/components';
import { FiltersInterface } from '../models/FiltersInterface';
import { useTranslation } from 'react-i18next';
import ClinicalConsultationService, {
    AdministrationGroup,
    County,
    Specialty,
    State,
    ZipCode
} from '../services/ClinicalConsultationService';

export type FilterBarProps = {
    isOpen: boolean;
    onApply: (f: FiltersInterface, c: number) => void;
    disabled: boolean;
    id: string;
    beneficiaryId: string;
    resetInput: () => void;
    specialtyPrevSelected: Specialty;
};

const FilterBar: React.FC<FilterBarProps> = ({
    isOpen,
    onApply,
    disabled,
    id,
    beneficiaryId,
    resetInput,
    specialtyPrevSelected
}) => {
    const { t } = useTranslation();
    const [filters, setFilters] = useState<FiltersInterface>();

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

    useEffect(() => {
        if (filters) {
            const filtersCount = Object.keys(filters).filter(
                k =>
                    filters[k] != '' &&
                    filters[k] !== undefined &&
                    filters[k] !== null
            ).length;

            if (filtersCount)
                onApply(
                    filters,
                    specialtyPrevSelected ? filtersCount + 1 : filtersCount
                );
        }
    }, [filters]);

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

    const filteredZipCode = () => {
        if (!selectedCounty) {
            return zipCodes;
        } else {
            return zipCodes?.filter(
                c => c.countyId === selectedCounty.countyId
            );
        }
    };

    useEffect(() => {
        ClinicalConsultationService.getInstance()
            .GetServicingProviderFilters(beneficiaryId, null)
            .then(response => {
                if (response) {
                    setCounties(response.data.county);
                    setStates(response.data.state);
                    setAdminGroups(response.data.administrationGroup);
                    setZipCodes(response.data.zipCode);
                    setDefaultState(response.data.state[0]);
                    setSelectedState(response.data.state[0]);
                }
            });
    }, []);

    useEffect(() => {
        var getStateId =
            selectedState?.stateId != defaultState?.stateId
                ? selectedState?.stateId
                : null;
        ClinicalConsultationService.getInstance()
            .GetServicingProviderFilters(beneficiaryId, getStateId)
            .then(response => {
                if (response) {
                    setStates(response.data.state);
                    setCounties(response.data.county);
                    setZipCodes(response.data.zipCode);
                }
            });
    }, [selectedState]);

    const handleResetFilter = () => {
        setSelectedAdminGroup(null);
        setSelectedCounty(null);
        setSelectedState(defaultState);
        setSelectedZipCode(null);
        resetInput();
    };
    const applyFilters = () => {
        setFilters({
            city: selectedCounty,
            group: selectedAdminGroup,
            country: selectedState,
            zipCode: selectedZipCode
        });
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

    return (
        <div style={{ display: isOpen ? 'block' : 'none' }}>
            <div className="filter-bar">
                <FormDropdownSelect
                    key={`admin-group-${
                        selectedAdminGroup?.administrationGroupId ??
                        adminGroups?.length
                    }`}
                    items={adminGroups}
                    value={selectedAdminGroup}
                    onChange={handleOnSelectAdminGroup}
                    formatItem={(s: AdministrationGroup) => s?.name}
                    formatSelected={(s: AdministrationGroup) => s?.name}
                    filterable={true}
                    disabled={false}
                    noItemsText={t(
                        'clinicalconsultation:servicing-provider.NO-RESULTS'
                    )}
                    placeHolder={t(
                        'clinicalconsultation:servicing-provider.ADMINISTRATION-GROUP'
                    )}
                    className={'filter-dropdown '}
                    requiredVisiblePlaceHolder={true}
                    blockRegex={/[^A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s]/g}
                />
                <FormDropdownSelect
                    key={`states-${selectedState?.stateId ?? states?.length}`}
                    items={states}
                    value={selectedState ? selectedState : defaultState}
                    onChange={handleOnSelectState}
                    formatItem={(s: State) => s?.name}
                    formatSelected={(s: State) => s?.name}
                    filterable={true}
                    disabled={false}
                    noItemsText={t(
                        'clinicalconsultation:servicing-provider.NO-RESULTS'
                    )}
                    placeHolder={t(
                        'clinicalconsultation:servicing-provider.COUNTRY'
                    )}
                    className={'filter-dropdown '}
                    requiredVisiblePlaceHolder={true}
                    blockRegex={/[^A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s]/g}
                />
                <FormDropdownSelect
                    key={`counties-${
                        selectedCounty?.countyId ?? counties?.length
                    }`}
                    items={counties}
                    value={selectedCounty}
                    onChange={handleOnSelectCounty}
                    formatItem={(s: County) => s?.name}
                    formatSelected={(s: County) => s?.name}
                    filterable={true}
                    disabled={false}
                    noItemsText={t(
                        'clinicalconsultation:servicing-provider.NO-RESULTS'
                    )}
                    placeHolder={t(
                        'clinicalconsultation:servicing-provider.CITIES'
                    )}
                    className={'filter-dropdown '}
                    requiredVisiblePlaceHolder={true}
                    blockRegex={/[^A-Za-zÁÉÍÓÚÜÑáéíóúüñ\s]/g}
                />
                <FormDropdownSelect
                    key={`zipcodes-${
                        selectedZipCode?.zipCodeId ?? zipCodes?.length
                    }`}
                    items={filteredZipCode()}
                    value={selectedZipCode}
                    onChange={handleOnSelectZipCode}
                    formatItem={(s: ZipCode) => s?.zipCodeValue}
                    formatSelected={(s: ZipCode) => s?.zipCodeValue}
                    filterable={true}
                    disabled={false}
                    noItemsText={t(
                        'clinicalconsultation:servicing-provider.NO-RESULTS'
                    )}
                    placeHolder={t(
                        'clinicalconsultation:servicing-provider.ZIPCODE'
                    )}
                    className={'filter-dropdown '}
                    requiredVisiblePlaceHolder={true}
                    blockRegex={/\D/g}
                />
                <button
                    type="button"
                    className="btn btn-md btn-info btn-info-gradient rounded-pill px-3 "
                    style={{ width: '133px', marginLeft: '8px' }}
                    id="consultation-servicing-provider-apply-filter-button"
                    onClick={applyFilters}
                    disabled={specialtyPrevSelected == null}
                >
                    {t('clinicalconsultation:servicing-provider.APPLY-FILTER')}
                </button>
                <button
                    type="button"
                    className="btn btn-link px-3 resert-btn "
                    id="consultation-servicing-provider-reset-filter-button "
                    onClick={handleResetFilter}
                >
                    {t('clinicalconsultation:servicing-provider.RESET-FILTER')}
                </button>
            </div>
        </div>
    );
};

export default FilterBar;
