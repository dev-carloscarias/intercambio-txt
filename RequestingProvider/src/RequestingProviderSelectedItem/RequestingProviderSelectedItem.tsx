import React, { useEffect, useState } from 'react';
import './RequestingProviderSelectedItem.scss';
import { RequestingProviderInterface } from '../RequestingProviderItem/RequestingProviderItem';
import { useTranslation } from 'react-i18next';
import { formatPhoneNumber } from '../utilities/PhoneUtilities';
import {
    DropdownPicker,
    BottomSheet,
    BottomSheetList,
    BottomSheetListItem,
    BottomSheetTitle,
    BottomSheetSubtitle
} from '@provider-portal/components';

export type RequestingProviderSelectedItemProps = {
    item: ProviderInterface;
    onSelectCity?: (c: CityInterface) => void;
    onCheck: (i: RequestingProviderInterface) => void;
    isMobile?: boolean;
    isOpenCitySheet?: boolean;
};

const RequestingProviderSelectedItem: React.FC<
    RequestingProviderSelectedItemProps
> = ({
    item,
    onSelectCity = () => {},
    onCheck,
    isMobile = false,
    isOpenCitySheet = false
}) => {
    const { t } = useTranslation();
    const [selectedCity, setSelectedCity] = useState<CityInterface>();

    const handleOnSelectCity = (city: CityInterface) => () => {
        isOpenCitySheet = false;
        const itemWithCity = { ...item };
        itemWithCity.selectedCity = city;
        setSelectedCity(city);
        onCheck(itemWithCity);
    };

    const onClose = () => (isOpenCitySheet = false);

    const handleOnSelect = (city: CityInterface) => {
        setSelectedCity(city);
    };

    const isMultipleCity = (provider: RequestingProviderInterface) => {
        return provider.cities?.length > 1;
    };

    useEffect(() => {
        if (selectedCity) onSelectCity(selectedCity);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [selectedCity]);

    useEffect(() => {
        if (item) setSelectedCity(item.selectedCity);
    }, [item]);

    return (
        <>
            <div className="requesting-provider-item">
                <div
                    className="requesting-provider-item-name"
                    id="clinical-consultation-requesting-provider-selected-name-value"
                >
                    {item.renderingProviderName}
                </div>
                <div className="d-lg-flex ">
                    <div className="mr-lg-5">
                        <div className="requesting-provider-item-data">
                            <div
                                className="requesting-provider-item-data-name"
                                id="clinical-consultation-requesting-provider-selected-npi-text"
                            >
                                {t(
                                    'clinicalconsultation:requesting-provider.PROVIDER-NPI'
                                )}
                            </div>
                            <div
                                className="requesting-provider-item-data-value"
                                id="clinical-consultation-requesting-provider-selected-npi-value"
                            >
                                {item.renderingProviderNPI}
                            </div>
                        </div>
                        <div className="requesting-provider-item-data">
                            <div
                                className="requesting-provider-item-data-name"
                                id="clinical-consultation-requesting-provider-selected-specialities-header"
                            >
                                {t(
                                    'clinicalconsultation:requesting-provider.PROVIDER-SPECIALTY'
                                )}
                            </div>
                            <div
                                className="requesting-provider-item-data-value"
                                id="clinical-consultation-requesting-provider-selected-specialities-value"
                            >
                                {item.specialties?.map(s => s.name).join(' | ')}
                            </div>
                        </div>
                        <div className="d-flex align-items-center flex-wrap">
                            <div className="requesting-provider-item-data">
                                <div
                                    className="requesting-provider-item-data-name"
                                    id="clinical-consultation-requesting-provider-selected-phone-header"
                                >
                                    {t(
                                        'clinicalconsultation:requesting-provider.PROVIDER-PHONE'
                                    )}
                                </div>
                                <div
                                    className="requesting-provider-item-data-value"
                                    id="clinical-consultation-requesting-provider-selected-phone-value"
                                >
                                    {item.phoneNumber
                                        ? formatPhoneNumber(item.phoneNumber)
                                        : ''}
                                </div>
                            </div>
                        </div>
                        <div className="requesting-provider-item-data">
                            <div
                                className="requesting-provider-item-data-name"
                                id="clinical-consultation-requesting-provider-selected-email-header"
                            >
                                {t(
                                    'clinicalconsultation:requesting-provider.PROVIDER-EMAIL'
                                )}
                            </div>
                            <div
                                className="requesting-provider-item-data-value"
                                id="clinical-consultation-requesting-provider-selected-email-value"
                            >
                                {item.email}
                            </div>
                        </div>
                    </div>
                    <div className="requesting-provider-item-data">
                        <div
                            className="requesting-provider-item-data-name"
                            id="clinical-consultation-requesting-provider-selected-city-header"
                        >
                            {t(
                                'clinicalconsultation:requesting-provider.PROVIDER-CITY'
                            )}
                        </div>
                        <div
                            className="requesting-provider-item-data-value"
                            id="clinical-consultation-requesting-provider-selected-city-value"
                        >
                            {item.selectedCity?.name}
                        </div>
                    </div>
                    <div>
                        {isMobile && isMultipleCity(item) ? (
                            <div id="clinical-consultation-requesting-provider-selected-multriple-city-selection-mobile">
                                <BottomSheet
                                    isOpen={isOpenCitySheet}
                                    onClose={onClose}
                                >
                                    <BottomSheetTitle>
                                        {t(
                                            'clinicalconsultation:requesting-provider.PROVIDER-SELECT-CITY'
                                        )}
                                    </BottomSheetTitle>
                                    <BottomSheetSubtitle>
                                        {t(
                                            'clinicalconsultation:requesting-provider.PROVIDER-SPECIALTY-CITY-MESSAGE'
                                        )}
                                    </BottomSheetSubtitle>
                                    <BottomSheetList>
                                        {item.cities.map((c, i: number) => (
                                            <BottomSheetListItem
                                                key={i}
                                                name={c.name}
                                                onClick={handleOnSelectCity(c)}
                                                formatItem={(
                                                    c: CityInterface
                                                ) => c.name}
                                                isActive={
                                                    c == item.selectedCity
                                                }
                                            />
                                        ))}
                                    </BottomSheetList>
                                </BottomSheet>
                            </div>
                        ) : isMultipleCity(item) && !item.selectedCity ? (
                            <div className="requesting-provider-item-data">
                                <div className="requesting-provider-item-data-value d-none d-mobile-block mb-2">
                                    <div
                                        className="dd-picker-city d-none d-mobile-block"
                                        id="clinical-consultation-requesting-provider-selected-multiple-city-droprown-picker"
                                    >
                                        <DropdownPicker
                                            title={'Specialist City'}
                                            selected={selectedCity}
                                            items={item.cities}
                                            onSelect={handleOnSelect}
                                            formatItem={(c: CityInterface) =>
                                                c.name
                                            }
                                            formatSelected={(
                                                c: CityInterface
                                            ) => c.name}
                                            placeholder={t(
                                                'clinicalconsultation:requesting-provider.PROVIDER-SELECT-CITY'
                                            )}
                                            autoOpen={true}
                                        />
                                    </div>
                                </div>
                            </div>
                        ) : null}
                    </div>
                </div>
            </div>
        </>
    );
};

export default RequestingProviderSelectedItem;
