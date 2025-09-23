import React, { useEffect, useState } from 'react';
import './ServicingProviderSelectedItem.scss';
import { DropdownPicker } from '@provider-portal/components';

import {
    CityInterface,
    ProviderInterface,
    ServicingProviderInterface
} from '../models/ProviderInterface';
import { useTranslation } from 'react-i18next';

export type ServicingProviderSelectedItemProps = {
    item: ProviderInterface;
    onSelectCity: (c: CityInterface) => void;
    id: string;
};

const ServicingProviderSelectedItem: React.FC<
    ServicingProviderSelectedItemProps
> = ({ item, onSelectCity, id }) => {
    const { t } = useTranslation();
    const [selectedCity, setSelectedCity] = useState<CityInterface>();

    const handleOnSelect = (city: CityInterface) => {
        setSelectedCity(city);
    };

    const isMultipleCity = (provider: ServicingProviderInterface) => {
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
            <div className="servicing-provider-item">
                <div className="servicing-provider-item-name" id={`${id}-name`}>
                    {item.renderingProviderName}
                </div>
                <div className="d-mobile-flex ">
                    <div className="mr-mobile-5">
                        <div className="servicing-provider-item-data">
                            <div
                                className="servicing-provider-item-data-name"
                                id={`${id}-specialties-label`}
                            >
                                {t(
                                    'clinicalconsultation:servicing-provider.PROVIDER-SPECIALTY'
                                )}
                            </div>
                            <div
                                className="servicing-provider-item-data-value"
                                id={`${id}-specialties`}
                            >
                                {item.specialties?.map(s => s.name).join(' | ')}
                            </div>
                        </div>
                        <div className="d-flex align-items-center flex-wrap">
                            <div className="servicing-provider-item-data">
                                <div
                                    className="servicing-provider-item-data-name"
                                    id={`${id}-phone-label`}
                                >
                                    {t(
                                        'clinicalconsultation:servicing-provider.PROVIDER-PHONE'
                                    )}
                                </div>
                                <div
                                    className="servicing-provider-item-data-value"
                                    id={`${id}-phone`}
                                >
                                    {item.phoneNumber}
                                </div>
                            </div>
                        </div>
                        <div className="servicing-provider-item-data">
                            <div
                                className="servicing-provider-item-data-name"
                                id={`${id}-email-label`}
                            >
                                {t(
                                    'clinicalconsultation:servicing-provider.PROVIDER-EMAIL'
                                )}
                            </div>
                            <div
                                className="servicing-provider-item-data-value"
                                id={`${id}-email`}
                            >
                                {item.email}
                            </div>
                        </div>
                    </div>
                    <div>
                        <div className="servicing-provider-item-data d-mobile-none">
                            <div
                                className="servicing-provider-item-data-name"
                                id={`${id}-city-mobile-label`}
                            >
                                {t(
                                    'clinicalconsultation:servicing-provider.PROVIDER-CITY'
                                )}
                            </div>
                            <div
                                className="servicing-provider-item-data-value"
                                id={`${id}-city-mobile`}
                            >
                                {item.selectedCity?.name}
                            </div>
                        </div>
                        {!isMultipleCity(item) ? (
                            <>
                                <div className="servicing-provider-item-data">
                                    <div
                                        className="servicing-provider-item-data-name"
                                        id={`${id}-city-label`}
                                    >
                                        {t(
                                            'clinicalconsultation:servicing-provider.PROVIDER-CITY'
                                        )}
                                    </div>
                                    <div
                                        className="servicing-provider-item-data-value"
                                        id={`${id}-city`}
                                    >
                                        {item.selectedCity?.name}
                                    </div>
                                </div>
                                <div className="servicing-provider-item-data">
                                    <div className="servicing-provider-item-data-name flex-shrink-0"></div>
                                    <div className="servicing-provider-item-data-value flex-shrink-0"></div>
                                </div>
                            </>
                        ) : (
                            <div className="servicing-provider-item-data">
                                <div className="servicing-provider-item-data-value d-none d-mobile-block mb-2">
                                    <div className="dd-picker-city d-none d-mobile-block">
                                        <DropdownPicker
                                            title={t(
                                                'clinicalconsultation:servicing-provider.PROVIDER-SPECIALTY-CITY'
                                            )}
                                            selected={selectedCity}
                                            items={item.cities}
                                            onSelect={handleOnSelect}
                                            formatItem={(c: CityInterface) =>
                                                c.name + ' - ' + c.zipCode
                                            }
                                            formatSelected={(
                                                c: CityInterface
                                            ) => c.name + ' - ' + c.zipCode}
                                            placeholder={t(
                                                'clinicalconsultation:servicing-provider.PROVIDER-SELECT-CITY'
                                            )}
                                            autoOpen={true}
                                            id={`${id}-city-dropdown`}
                                        />
                                    </div>
                                </div>
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </>
    );
};

export default ServicingProviderSelectedItem;
