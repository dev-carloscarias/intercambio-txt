import React, { useCallback, useEffect, useRef, useState } from 'react';
import './App.scss';
import {
    CardWidgetHeading,
    CardWidgetSubheading,
    FormFlexInputDynamicSearch,
    CardWidgetTable,
    CardWidgetTableRemoveIcon,
    BottomSheetSelect,
    DropdownPicker,
    CONST,
    LoadingIndicator
} from '@provider-portal/components';
import { IconPlus, IconMinus, IconFilterList } from '@provider-portal/icons';
import {
    Collapse,
    Dropdown,
    DropdownMenu,
    DropdownToggle,
    Fade
} from 'reactstrap';
import FilterPanel from './FilterPanel/FilterPanel';
import ServicingProviderItem from './ServicingProviderItem/ServicingProviderItem';
import ServicingProviderSelectedItem from './ServicingProviderSelectedItem/ServicingProviderSelectedItem';
import FilterBar from './FilterBar/FilterBar';
import BottomSheetCancelNext from './BottomSheetCancelNext/BottomSheetCancelNext';
import { Consultation } from './models/Consultation';
import {
    ProviderInterface,
    CityInterface,
    ServicingProviderInterface,
    SpecialtyInterface
} from './models/ProviderInterface';
import ClinicalConsultationService, {
    BeneficiaryInformation
} from './services/ClinicalConsultationService';
import { Trans, useTranslation } from 'react-i18next';
import InfiniteScroll from 'react-infinite-scroll-component';
import { AuditEventGroups } from './models/AuditEventGroups';
import { AuditEventTypes } from './models/AuditEventTypes';
import { Specialties } from './mock-data';
import { FiltersInterface, hasFilters } from './models/FiltersInterface';

interface SearchParams {
    searchValue: string;
    page: number;
    filters: FiltersInterface;
}

function App({
    onValidate,
    consultation,
    active,
    beneficiaryId,
    stepAlertMessage
}: {
    onValidate: (c: Consultation) => void;
    consultation: Consultation;
    active: boolean;
    beneficiaryId: string;
    stepAlertMessage: string;
}) {
    const { t } = useTranslation();
    const [beneficiaryInformation, setBeneficiaryInformation] =
        useState<BeneficiaryInformation>(null);

    const [isOpenCollapse, setIsOpenCollapse] = useState(false);
    const [isOpenSheet, setIsOpenSheet] = useState(false);
    const [isValidForm, setIsValidForm] = useState(false);
    const [isSearchLoading, setIsSearchLoading] = useState(false);
    const [providers, setProviders] = useState<ServicingProviderInterface[]>(
        []
    );
    const [hasMore, setHasMore] = useState(false);
    const [selectedProvider, setSelectedProvider] =
        useState<ProviderInterface>();
    const [totalRows, setTotalRows] = useState(0);

    const [isOpenDropdown, setIsOpenDropdown] = useState(false);

    const [isFilterPanelOpen, setIsFilterPanelOpen] = useState(false);
    const [isFilterBarOpen, setIsFilterBarOpen] = useState(false);
    const [filtersCount, setFiltersCount] = useState(0);

    const [searchInitiated, setSearchInitiated] = useState(false);

    const [searchParams, setSearchParams] = useState<SearchParams>({
        searchValue: '',
        page: 1,
        filters: {}
    });
    const [wasUpdated, setWasUpdated] = useState(false);

    // Ref to store the current AbortController
    const controllerRef = useRef<AbortController | null>(null);

    const isMobile = () => !!window.matchMedia(CONST.MQ_MOBILE_DOWN).matches;

    const isBottomSheetElement = (element: Element) => {
        return element
            ?.getAttribute('class')
            ?.split(' ')
            .some(c => /bottom-sheet-list-item-.*/.test(c));
    };

    const toggleDropdown = (e: React.MouseEvent) => {
        // prevent click sheets to trigger drodpown toggle
        if (isBottomSheetElement(e.target as Element)) return;
        // do not show empty dropdowns
        if (providers.length) setIsOpenDropdown(!isOpenDropdown);
    };

    const loadProviders = async (
        term: string,
        page: number,
        filters: FiltersInterface = null,
        signal: any
    ) => {
        //if (isSearchLoading) return;
        if (page === 1) setProviders([]);
        setIsSearchLoading(true);

        try {
            const response =
                await ClinicalConsultationService.getInstance().SearchServicingProvider(
                    beneficiaryId,
                    term,
                    page,
                    filters,
                    signal
                );

            if (page === 1) {
                await ClinicalConsultationService.getInstance().logAuditEvent(
                    AuditEventTypes.ServicingProviderSearch,
                    AuditEventGroups.ClinicalConsultation,
                    {
                        search: term,
                        city: filters.city,
                        administrationGroup: filters.group,
                        state: filters.country,
                        zipCode: filters.zipCode,
                        specialty: filters.specialty,
                        totalCount: response?.data?.total,
                        beneficiaryId: beneficiaryInformation?.beneficiaryId,
                        beneficiaryName: beneficiaryInformation?.displayName
                    }
                );
            } else {
                await ClinicalConsultationService.getInstance().logAuditEvent(
                    AuditEventTypes.ServicingProviderSearchLoadMore,
                    AuditEventGroups.ClinicalConsultation,
                    {
                        search: term,
                        page: page,
                        city: filters.city,
                        administrationGroup: filters.group,
                        state: filters.country,
                        zipCode: filters.zipCode,
                        specialty: filters.specialty,
                        totalCount: totalRows,
                        beneficiaryId: beneficiaryInformation?.beneficiaryId,
                        beneficiaryName: beneficiaryInformation?.displayName
                    }
                );
            }

            if (response.config.signal?.aborted === true) {
                return;
            }

            if (response?.data) {
                if (page === 1) {
                    setProviders(response?.data?.servicingProviders || []);
                    setTotalRows(response.data.total);
                } else {
                    setProviders(prev => [
                        ...prev,
                        ...response?.data?.servicingProviders
                    ]);
                }
            }
        } finally {
            setIsSearchLoading(false);
        }
    };

    const isMultipleCity = (provider: ServicingProviderInterface) => {
        return (provider.cities?.length || 0) > 1;
    };

    const isMultipleSpecialty = (provider: ServicingProviderInterface) => {
        return provider.specialties?.length > 1;
    };

    const handleOnCheck = (item: ServicingProviderInterface) => {
        console.log('App - handleOnCheck called with item:', item);
        console.log('App - item.specialties:', item.specialties);
        console.log('App - isMultipleSpecialty:', isMultipleSpecialty(item));
        
        if (item) {
            const updatedItem = { ...item };
            if (!isMultipleCity(item)) updatedItem.selectedCity = item.cities?.[0];
            if (!isMultipleSpecialty(item)) {
                console.log('App - Setting first specialty:', item.specialties?.[0]);
                updatedItem.selectedSpecialty = item.specialties?.[0];
            }
            console.log('App - Final updatedItem:', updatedItem);
            setSelectedProvider(updatedItem);
        } else {
            setSelectedProvider(null);
        }
    };

    useEffect(() => {
        // when detect providers loaded, trigger dropdown visibility
        setIsOpenDropdown(!!providers.length);
    }, [providers]);

    useEffect(() => {
        if (selectedProvider) {
            setIsOpenDropdown(false);
            if (selectedProvider.selectedCity) {
                setIsValidForm(true);
            }
        } else setIsValidForm(false);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [selectedProvider]);

    useEffect(() => {
        if (searchParams.searchValue || searchParams.filters?.specialty) {
            // Cancelar request anterior si existe
            if (controllerRef.current) {
                controllerRef.current.abort();
            }

            const controller = new AbortController();
            controllerRef.current = controller;

            loadProviders(
                searchParams.searchValue,
                searchParams.page,
                searchParams.filters,
                controller.signal
            );

            return () => {
                controller.abort();
            };
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [searchParams, searchInitiated]);

    const onSelectSpecialty = (item: string) => {
        setSearchParams(prev => {
            var filters = prev.filters;
            filters.specialty = item;

            return {
                searchValue: prev.searchValue,
                page: 1,
                filters: filters
            };
        });
    };

    const handleFilterPanelClose = () => setIsFilterPanelOpen(false);

    const openFilterPanel = () => setIsFilterPanelOpen(true);

    const handleOnApply = (filters: FiltersInterface, total: number) => {
        setFiltersCount(total);
        setSearchParams(prev => {
            return {
                searchValue: prev.searchValue,
                page: 1,
                filters: {
                    specialty: prev.filters?.specialty,
                    city: filters.city,
                    country: filters.country,
                    group: filters.group,
                    zipCode: filters.zipCode
                }
            };
        });
        setIsFilterPanelOpen(false);
    };

    // return an updated consultation object
    const getConsultation = useCallback(() => {
        return {
            ...consultation,
            consultationProvider: selectedProvider
        };
    }, [consultation, selectedProvider]);

    const updateWizardProgress = () => {
        setWasUpdated(true);
        onValidate(getConsultation());
    };

    const handleClickNext = () => {
        updateWizardProgress();
        setIsOpenSheet(false);
        setIsOpenCollapse(false);
    };

    const handleSelectCity = (city: CityInterface) => {
        setSelectedProvider(sp => {
            if (!sp) return sp;
            const p = { ...sp };
            p.selectedCity = city;
            return p;
        });
    };

    const handleSelectSpecialty = (specialty: SpecialtyInterface) => {
        console.log('App - handleSelectSpecialty called with specialty:', specialty);
        setSelectedProvider(sp => {
            if (!sp) return sp;
            const p = { ...sp };
            p.selectedSpecialty = specialty;
            console.log('App - Updated provider with specialty:', p);
            return p;
        });
    };

    const toggleFilterBar = (e: any) => {
        console.log('toggleFilterBar called, current isFilterBarOpen:', isFilterBarOpen);
        if (e && e.stopPropagation) {
            e.stopPropagation(); // prevent button toggle drodpown
        }
        setIsOpenDropdown(false); // click always close dropdown dropdown results
        const newValue = !isFilterBarOpen;
        setIsFilterBarOpen(newValue);
        console.log('toggleFilterBar setting isFilterBarOpen to:', newValue);
    };

    const handleClickSpecialties = (e: React.MouseEvent) => {
        e.stopPropagation(); // prevent dropdownpicker to open dropdown results
        setIsOpenDropdown(false); // click always close dropdown dropdown results
    };

    useEffect(() => {
        if (consultation?.consultationProvider?.renderingProviderId) {
            const { consultationProvider: provider } = consultation;
            // update selected from recreate if not one selected before
            const updatedProvider = { 
                ...provider,
                selectedCity: provider.selectedCity ?? provider.cities?.[0],
                selectedSpecialty: provider.selectedSpecialty ?? provider.specialties?.[0]
            };
            setSelectedProvider(updatedProvider);
        }
    }, [consultation]);

    const handleOnCancel = () => setIsOpenSheet(false);

    const toggleCollapse = () => setIsOpenCollapse(!isOpenCollapse);

    useEffect(() => {
        setIsOpenCollapse(active);
    }, [active]);

    useEffect(() => {
        const shouldOpen = isMobile() && isOpenCollapse && isValidForm;
        setIsOpenSheet(shouldOpen);
    }, [isValidForm, isOpenCollapse]);

    const handleOnSearch = (term: string) =>
        setSearchParams(prev => {
            return { searchValue: term, page: 1, filters: prev.filters };
        });
    const handleLoadMore = () =>
        setSearchParams(prev => {
            return {
                searchValue: prev.searchValue,
                page: prev.page + 1,
                filters: prev.filters
            };
        });

    const handleOnClearSearch = () => {
        setSearchParams({ searchValue: '', page: 1, filters: {} });
        setProviders([]);
    };

    const handleSearchInputClick = () => {
        if (providers && providers.length > 0) {
            setIsOpenDropdown(true);
        }
    };

    useEffect(() => {
        setHasMore(totalRows > providers?.length);
    }, [providers, totalRows]);

    return (
        <>
            <div className="px-3">
                <div
                    id="consultation-servicing-provider-togglebar"
                    className="d-flex align-items-center"
                    role="button"
                    tabIndex={0}
                    onClick={toggleCollapse}
                >
                    <div>
                        <CardWidgetHeading>
                            <span id="consultation-servicing-provider-header">
                                {t(
                                    'clinicalconsultation:servicing-provider.HEADER'
                                )}
                            </span>
                        </CardWidgetHeading>
                        <CardWidgetSubheading>
                            <span id="consultation-servicing-provider-subheader">
                                {t(
                                    'clinicalconsultation:servicing-provider.SUBHEADER'
                                )}
                            </span>
                        </CardWidgetSubheading>
                    </div>
                    <div className="ml-auto mr-3">
                        {isOpenCollapse ? (
                            <IconMinus
                                width="1.5rem"
                                height="1.5rem"
                                id="consultation-servicing-provider-icon-minus"
                            />
                        ) : (
                            <IconPlus
                                width="1.5rem"
                                height="1.5rem"
                                id="consultation-servicing-provider-icon-plus"
                            />
                        )}
                    </div>
                </div>
            </div>
            <Collapse isOpen={isOpenCollapse}>
                <div className="mt-3">
                    <div className="d-mobile-flex">
                        <div className="smartprofile-referral-provider">
                            <Dropdown
                                isOpen={isOpenDropdown}
                                toggle={toggleDropdown}
                                className="dropdown-input"
                            >
                                <DropdownToggle tag="div">
                                    <div className="px-3">
                                        <div className="d-mobile-flex">
                                            <div className="smartprofile-referral-provider-search">
                                                <FormFlexInputDynamicSearch
                                                    onSearch={handleOnSearch}
                                                    placeholder={t(
                                                        'clinicalconsultation:servicing-provider.SEARCH-PLACEHOLDER'
                                                    )}
                                                    isLoading={isSearchLoading}
                                                    onClear={
                                                        handleOnClearSearch
                                                    }
                                                    id="consultation-servicing-provider-search-input-text"
                                                    debounceSearch={true}
                                                    onClick={
                                                        handleSearchInputClick
                                                    }
                                                />
                                            </div>
                                            <div className="mt-3 mt-mobile-0 mr-mobile-3">
                                                <div className="d-mobile-none">
                                                    <BottomSheetSelect
                                                        title={t(
                                                            'clinicalconsultation:servicing-provider.SPECIALTIES'
                                                        )}
                                                        selected={
                                                            searchParams.filters
                                                                .specialty
                                                        }
                                                        items={Specialties}
                                                        onSelect={
                                                            onSelectSpecialty
                                                        }
                                                        placeholder={t(
                                                            'clinicalconsultation:servicing-provider.SPECIALIST'
                                                        )}
                                                        filterable={true}
                                                        disabled={
                                                            isSearchLoading
                                                        }
                                                        id="consultation-servicing-provider-search-specialty-mobile"
                                                    />
                                                </div>
                                                <div className="d-none d-mobile-block">
                                                    <DropdownPicker
                                                        title={t(
                                                            'clinicalconsultation:servicing-provider.SPECIALTIES'
                                                        )}
                                                        selected={
                                                            searchParams.filters
                                                                .specialty
                                                        }
                                                        items={Specialties}
                                                        onSelect={
                                                            onSelectSpecialty
                                                        }
                                                        placeholder={t(
                                                            'clinicalconsultation:servicing-provider.SPECIALIST'
                                                        )}
                                                        disabled={
                                                            isSearchLoading
                                                        }
                                                        onClick={
                                                            handleClickSpecialties
                                                        }
                                                        id="consultation-servicing-provider-filter-specialty"
                                                    />
                                                </div>
                                            </div>
                                            <div className="flex-shrink-0">
                                                <button
                                                    className="btn btn-smartprofile-referral-provider-filter w-100 d-mobile-block more-filter-mobile-spacing"
                                                    onClick={toggleFilterBar}
                                                    id="consultation-servicing-provider-search-morefilters"
                                                >
                                                    <div className="d-flex align-items-center">
                                                        {t(
                                                            'clinicalconsultation:servicing-provider.MORE-FILTER'
                                                        )}
                                                        {filtersCount ? (
                                                            <span
                                                                className="ml-1"
                                                                id="consultation-servicing-provider-search-filtercount"
                                                            >
                                                                ( {filtersCount}{' '}
                                                                )
                                                            </span>
                                                        ) : null}
                                                        <div className="fade-toggle-icon ml-2">
                                                            <Fade
                                                                in={
                                                                    !isFilterBarOpen
                                                                }
                                                            >
                                                                <em
                                                                    className="fa fa-caret-down"
                                                                    id="consultation-servicing-provider-search-filters-downbtn"
                                                                ></em>
                                                            </Fade>
                                                            <Fade
                                                                in={
                                                                    isFilterBarOpen
                                                                }
                                                            >
                                                                <em
                                                                    className="fa fa-caret-up"
                                                                    id="consultation-servicing-provider-search-filters-upbtn"
                                                                ></em>
                                                            </Fade>
                                                        </div>
                                                    </div>
                                                </button>
                                            </div>
                                        </div>
                                    </div>
                                </DropdownToggle>
                                <DropdownMenu
                                    id="consultation-servicing-provider-search-results-container"
                                    className="smartprofile-referral-provider-dropdown-menu"
                                >
                                    <div className="dropdown-input-results">
                                        <div
                                            className="dropdown-input-results-total"
                                            id="consultation-servicing-provider-search-results-count"
                                        >
                                            <Trans
                                                i18nKey={
                                                    'clinicalconsultation:servicing-provider.RESULTS'
                                                }
                                                values={{
                                                    totalRows: totalRows
                                                }}
                                            >
                                                Results {totalRows} specialists
                                            </Trans>
                                        </div>
                                        <div
                                            className="d-flex align-items-center dropdown-input-results-filter d-mobile-none"
                                            onClick={openFilterPanel}
                                            role="button"
                                            tabIndex={0}
                                            id="consultation-servicing-provider-search-results-filters"
                                        >
                                            <IconFilterList
                                                width="1rem"
                                                height="1rem"
                                                className="mr-1"
                                                id="consultation-servicing-provider-search-results-filters-icon"
                                            />
                                            {t(
                                                'clinicalconsultation:servicing-provider.MORE-FILTER'
                                            )}
                                            {filtersCount ? (
                                                <span
                                                    className="ml-1"
                                                    id="consultation-servicing-provider-search-results-filters-count"
                                                >
                                                    ({filtersCount})
                                                </span>
                                            ) : null}
                                        </div>
                                    </div>
                                    <InfiniteScroll
                                        scrollableTarget={
                                            'consultation-servicing-provider-search-results-container'
                                        }
                                        dataLength={providers.length}
                                        next={handleLoadMore}
                                        hasMore={hasMore}
                                        loader={
                                            <LoadingIndicator
                                                id={
                                                    'consultation-servicing-provider-search-results-loading-indicator'
                                                }
                                                loading={isSearchLoading}
                                                overlay={false}
                                                inline={true}
                                            />
                                        }
                                        endMessage={
                                            <span
                                                id="consultation-servicing-provider-search-results-end"
                                                className="dropdown-input-end-results"
                                            >
                                                <Trans
                                                    i18nKey={
                                                        'clinicalconsultation:create.END-OF-RESULTS'
                                                    }
                                                >
                                                    End of results.
                                                </Trans>
                                            </span>
                                        }
                                    >
                                        {providers.map((p, i) => (
                                            <div
                                                className="dropdown-input-item"
                                                key={i}
                                            >
                                                <ServicingProviderItem
                                                    id={i}
                                                    item={p}
                                                    onCheck={handleOnCheck}
                                                    checked={
                                                        p.providerAffiliationId ==
                                                        selectedProvider?.providerAffiliationId
                                                    }
                                                    autoSelectCity={isMobile()}
                                                />
                                            </div>
                                        ))}
                                    </InfiniteScroll>
                                </DropdownMenu>
                            </Dropdown>
                        </div>
                    </div>
                </div>

                <div className="px-3">
                    {(() => {
                        console.log('FilterBar render - isFilterBarOpen:', isFilterBarOpen);
                        return (
                            <FilterBar
                                isOpen={isFilterBarOpen}
                                onApply={handleOnApply}
                                disabled={isSearchLoading}
                                id="consultation-servicing-provider-filterbar"
                            />
                        );
                    })()}
                </div>

                <div className="mt-3 px-3">
                    <CardWidgetTable
                        headers={[
                            t(
                                'clinicalconsultation:servicing-provider.SELECTED'
                            )
                        ]}
                        placeholder={t(
                            'clinicalconsultation:servicing-provider.NO-PROVIDER'
                        )}
                        id="consultation-servicing-provider-selected-header"
                    >
                        {selectedProvider ? (
                            <tbody>
                                <tr>
                                    <td>
                                        {(() => {
                                            console.log('App - About to render ServicingProviderSelectedItem with selectedProvider:', selectedProvider);
                                            return (
                                                <ServicingProviderSelectedItem
                                                    item={selectedProvider}
                                                    onSelectCity={handleSelectCity}
                                                    onSelectSpecialty={handleSelectSpecialty}
                                                    id="consultation-servicing-provider-selected-item"
                                                />
                                            );
                                        })()}
                                    </td>
                                </tr>
                            </tbody>
                        ) : null}
                    </CardWidgetTable>
                    {selectedProvider && !selectedProvider.selectedCity ? (
                        <div
                            className="d-none d-mobile-block text-danger text-sm pl-2 pt-2"
                            id="consultation-servicing-provider-selectcity-msg"
                        >
                            {t(
                                'clinicalconsultation:servicing-provider.PROVIDER-SELECT-CITY-REQUIRE-MSG'
                            )}
                        </div>
                    ) : null}
                </div>
                {!isValidForm && wasUpdated ? (
                    <div
                        className=" d-mobile-block text-danger text-sm pl-2 pt-2 ml-3"
                        id="clinical-consultation-requesting-provider-alert-message"
                    >
                        {stepAlertMessage}
                    </div>
                ) : null}
                <div className="d-none d-mobile-block px-3 mt-3">
                    <div className="d-flex aling-items-center pt-3">
                        <button
                            type="button"
                            className="btn btn-md btn-info btn-info-gradient rounded-pill px-3 mr-3"
                            style={{ width: '133px' }}
                            onClick={handleClickNext}
                            disabled={!isValidForm}
                            id="consultation-servicing-provider-next-button"
                        >
                            {t('clinicalconsultation:servicing-provider.NEXT')}
                        </button>
                        <button
                            type="button"
                            className="btn btn-md btn-outline-secondary rounded-pill px-3 "
                            style={{ width: '133px' }}
                            disabled={!isValidForm}
                            id="consultation-servicing-provider-cancel-button"
                        >
                            {t(
                                'clinicalconsultation:servicing-provider.CANCEL'
                            )}
                        </button>
                    </div>
                </div>
            </Collapse>

            <BottomSheetCancelNext
                isOpen={isOpenSheet}
                id="consultation-servicing-provider-nextcancel-mobile"
                onClose={handleOnCancel}
                onCancel={handleOnCancel}
                onNext={handleClickNext}
                isValid={!isValidForm}
            />
            <FilterPanel
                id="consultation-servicing-provider-filterpanel-mobile"
                isOpen={isFilterPanelOpen}
                onClose={handleFilterPanelClose}
                searchTerm={searchParams.searchValue}
                onApply={handleOnApply}
                totalResults={totalRows}
            />
        </>
    );
}

export default App;
