import React, { useEffect, useState } from 'react';
import './ServicingProviderSelectedItem.scss';
import { DropdownPicker } from '@provider-portal/components';

import {
    CityInterface,
    ProviderInterface,
    ServicingProviderInterface,
    SpecialtyInterface
} from '../models/ProviderInterface';
import { useTranslation } from 'react-i18next';

export type ServicingProviderSelectedItemProps = {
    item: ProviderInterface;
    onSelectCity: (c: CityInterface) => void;
    onSelectSpecialty?: (s: SpecialtyInterface) => void;
    id: string;
};

const ServicingProviderSelectedItem: React.FC<
    ServicingProviderSelectedItemProps
> = ({ item, onSelectCity, onSelectSpecialty, id }) => {
    console.log('ServicingProviderSelectedItem - Component started rendering');
    console.log('ServicingProviderSelectedItem - Props received:', { item, onSelectCity, onSelectSpecialty, id });
    
    const { t } = useTranslation();
    const [selectedCity, setSelectedCity] = useState<CityInterface>();
    const [selectedSpecialty, setSelectedSpecialty] = useState<SpecialtyInterface>();

    const handleOnSelectCity = (city: CityInterface) => {
        setSelectedCity(city);
    };

    const handleOnSelectSpecialty = (specialty: SpecialtyInterface) => {
        setSelectedSpecialty(specialty);
    };

    const isMultipleCity = (provider: ServicingProviderInterface) => {
        return (provider.cities?.length || 0) > 1;
    };

    const isMultipleSpecialty = (provider: ServicingProviderInterface) => {
        return (provider.specialties?.length || 0) > 1;
    };

    useEffect(() => {
        if (selectedCity) onSelectCity(selectedCity);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [selectedCity]);

    useEffect(() => {
        if (selectedSpecialty && onSelectSpecialty) onSelectSpecialty(selectedSpecialty);
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [selectedSpecialty]);

    useEffect(() => {
        if (item){
            console.log('ITEM: ',item);
            console.log('Specialities: ',item.specialties);
            console.log('Cities: ',item.cities);
            console.log('SelectedCity: ',item.selectedCity);
            
            // Si no hay ciudad seleccionada pero hay ciudades disponibles, seleccionar la primera
            if (!item.selectedCity && item.cities?.length > 0) {
                console.log('Setting first city:', item.cities[0]);
                setSelectedCity(item.cities[0]);
            } else {
                setSelectedCity(item.selectedCity);
            }
            
            if(!item.selectedSpecialty && item.specialties?.length > 0){
                setSelectedSpecialty(item.specialties[0]);
            }else {
                setSelectedSpecialty(item.selectedSpecialty);
            }
        } 
    }, [item]);

    console.log('ServicingProviderSelectedItem - About to return JSX');
    
    return (
        <>
            <div className="servicing-provider-item">
                <div className="servicing-provider-item-name" id={`${id}-name`}>
                    {item.renderingProviderName}
                </div>
                <div className="d-mobile-flex ">
                    <div className="mr-mobile-5">
                        <div className="servicing-provider-item-data d-flex align-items-center">
                            <div
                                className="servicing-provider-item-data-name flex-shrink-0"
                                id={`${id}-specialties-label`}
                            >
                                {t(
                                    'clinicalconsultation:servicing-provider.PROVIDER-SPECIALTY'
                                )}
                            </div>
                            {(() => {
                                console.log('Rendering specialties - item.specialties:', item.specialties);
                                console.log('Rendering specialties - isMultipleSpecialty:', isMultipleSpecialty(item));
                                console.log('Rendering specialties - selectedSpecialty:', selectedSpecialty);

                                if(!item.specialties || item.specialties.length === 0 ){
                                    return (
                                        <div
                                            className="servicing-provider-item-data-value flex-grow-1"
                                            id={`${id}-specialties`}
                                        >
                                                {t('clinicalconsultation:servicing-provider.NO-SPECIALTY-SELECTED')}
                                        </div>
                                    );
                                }else if (!isMultipleSpecialty(item)){
                                    const specialtyName = item.specialties[0]?.name || '';
                                    console.log('Rendering single specialty name', specialtyName);
                                    return (
                                    <div
                                        className="servicing-provider-item-data-value flex-grow-1"
                                        id={`${id}-specialties`}
                                    >
                                        {specialtyName}
                                    </div>
                                    );
                                }else{
                                    console.log('Rendering dropdown with items: ', item.specialties);
                                    console.log('Rendering dropdown with selected: ', selectedSpecialty);
                                    
                                    return (
                                        <div className="servicing-provider-item-data-value flex-grow-1">
                                            <div className="dd-picker-specialty">
                                                <DropdownPicker
                                                    title={t(
                                                        'clinicalconsultation:servicing-provider.PROVIDER-SPECIALTY'
                                                    )}
                                                    selected={selectedSpecialty || null}
                                                    items={item.specialties}
                                                    onSelect={handleOnSelectSpecialty}
                                                    formatItem={(s: SpecialtyInterface) => s.name}
                                                    formatSelected={(s: SpecialtyInterface) => s.name}
                                                    placeholder={t(
                                                        'clinicalconsultation:servicing-provider.PROVIDER-SELECT-SPECIALTY'
                                                    )}
                                                    autoOpen={false}
                                                    id={`${id}-specialty-dropdown`}
                                                />
                                            </div>
                                        </div>
                                    );
                                }
                            })()}
                            


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
                        <div className="servicing-provider-item-data">
                            <div
                                className="servicing-provider-item-data-name"
                                id={`${id}-npi-label`}
                            >
                                {t(
                                    'clinicalconsultation:servicing-provider.PROVIDER-NPI'
                                )}
                            </div>
                            <div
                                className="servicing-provider-item-data-value"
                                id={`${id}-npi`}
                            >
                                {item.renderingProviderNPI || t('clinicalconsultation:servicing-provider.NO-NPI-AVAILABLE')}
                            </div>
                        </div>
                    </div>
                    <div>
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
                                         {item.selectedCity?.name || item.cities?.[0]?.name || t('clinicalconsultation:servicing-provider.NO-CITY-SELECTED')}
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
                                            onSelect={handleOnSelectCity}
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