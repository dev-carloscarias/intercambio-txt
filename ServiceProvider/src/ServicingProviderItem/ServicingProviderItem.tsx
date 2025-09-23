import React, { useState } from 'react';
import './ServicingProviderItem.scss';
import {
    CustomCheckbox,
    BottomSheet,
    BottomSheetList,
    BottomSheetListItem,
    BottomSheetTitle,
    BottomSheetSubtitle,
    WithTooltip
} from '@provider-portal/components';
import {
    IconAngleDown,
    IconInformation,
    IconStarRounded,
    IconRibbonCheck
} from '@provider-portal/icons';
import {
    CityInterface,
    ServicingProviderInterface
} from '../models/ProviderInterface';
import { useTranslation } from 'react-i18next';

export type ServicingProviderItemProps = {
    item: ServicingProviderInterface;
    // callback with item marked as selected
    onCheck: (i: ServicingProviderInterface) => void;
    // keep checked state in consecutive openings
    checked: boolean;
    // display bottom sheet to select city? intended for mobile only
    autoSelectCity?: boolean;
    id?: number;
};

const ServicingProviderItem: React.FC<ServicingProviderItemProps> = ({
    item,
    onCheck,
    checked,
    id,
    autoSelectCity = false
}) => {
    const { t } = useTranslation();
    const [isOpenCitySheet, setIsOpenCitySheet] = useState(false);

    const handleOnSelectCity = (city: CityInterface) => () => {
        setIsOpenCitySheet(false);
        const itemWithCity = { ...item };
        itemWithCity.selectedCity = city;
        onCheck(itemWithCity);
    };

    const onClose = () => setIsOpenCitySheet(false);

    const handleOnCheck = (e: React.ChangeEvent<HTMLInputElement>) => {
        const state = e.target.checked;
        if (state) {
            if (
                autoSelectCity &&
                (hasMultiple(item.cities, 'name') ||
                    hasMultiple(item.cities, 'zipCode')) &&
                !item.selectedCity
            ) {
                e.preventDefault();
                setIsOpenCitySheet(true);
            } else onCheck(item);
        } else onCheck(null);
    };

    const hasMultiple = (arr: any, property: string) => {
        const uniqueValues = new Set();

        for (const obj of arr) {
            if (obj.hasOwnProperty(property)) {
                uniqueValues.add(obj[property]);
            }
        }

        return uniqueValues.size > 1;
    };

    return (
        <>
            <CustomCheckbox
                inputProps={{
                    checked: checked,
                    onChange: handleOnCheck
                }}
                labelProps={{
                    role: 'button',
                    tabIndex: 0
                }}
                id={'consultation-servicing-provider-checkbox-' + id}
            >
                <div className="servicing-provider-item">
                    {item.information ? (
                        <div
                            className="servicing-provider-item-icon-information"
                            title={item.information}
                            id={
                                'consultation-servicing-provider-information-icon-' +
                                id
                            }
                        >
                            <WithTooltip text={'Information text'}>
                                <IconInformation
                                    width="1.5rem"
                                    height="1.5rem"
                                    onClick={(e: React.MouseEvent) =>
                                        e.preventDefault()
                                    }
                                />
                            </WithTooltip>
                        </div>
                    ) : null}
                    {item.isFeatured ? (
                        <div
                            className="servicing-provider-item-icon-feature"
                            id={
                                'consultation-servicing-provider-feature-icon-' +
                                id
                            }
                        >
                            <WithTooltip text={'Information text'}>
                                <IconRibbonCheck
                                    width="1.5rem"
                                    height="1.5rem"
                                    onClick={(e: React.MouseEvent) =>
                                        e.preventDefault()
                                    }
                                />
                            </WithTooltip>
                        </div>
                    ) : null}
                    {item.isStarred ? (
                        <div className="servicing-provider-item-icon-feature">
                            <WithTooltip text={'Information text'}>
                                <IconStarRounded
                                    width="1.5rem"
                                    height="1.5rem"
                                    onClick={(e: React.MouseEvent) =>
                                        e.preventDefault()
                                    }
                                    id={
                                        'consultation-servicing-provider-star-icon-' +
                                        id
                                    }
                                />
                            </WithTooltip>
                        </div>
                    ) : null}

                    <div
                        className="servicing-provider-item-name"
                        id={
                            'consultation-servicing-provider-item-name-value-' +
                            id
                        }
                    >
                        {item.renderingProviderName}
                    </div>
                    <div className="row">
                        <div className="col-12">
                            <div className="servicing-provider-item-data">
                                <div
                                    className="servicing-provider-item-data-name"
                                    id={
                                        'consultation-servicing-provider-item-data-speciality-text-' +
                                        id
                                    }
                                >
                                    {t(
                                        'clinicalconsultation:servicing-provider.PROVIDER-SPECIALTY'
                                    )}
                                </div>
                                <div
                                    className="servicing-provider-item-data-value"
                                    id={
                                        'consultation-servicing-provider-item-data-speciality-value-' +
                                        id
                                    }
                                >
                                    {item.specialties
                                        ?.map(s => s.name)
                                        .join(' | ')}
                                </div>
                            </div>
                        </div>
                        <div className="col-12 col-lg-3">
                            <div className="servicing-provider-item-data">
                                <div
                                    className="servicing-provider-item-data-name"
                                    id={
                                        'consultation-servicing-provider-item-data-npi-text-' +
                                        id
                                    }
                                >
                                    {t(
                                        'clinicalconsultation:servicing-provider.PROVIDER-NPI'
                                    )}
                                </div>
                                <div
                                    className="servicing-provider-item-data-value"
                                    id={
                                        'consultation-servicing-provider-item-data-npi-value-' +
                                        id
                                    }
                                >
                                    {item.renderingProviderNPI}
                                </div>
                            </div>
                        </div>
                        <div className="col-12 col-lg-3">
                            <div className="servicing-provider-item-data">
                                <div
                                    className="servicing-provider-item-data-name"
                                    id={
                                        'consultation-servicing-provider-item-data-city-text-' +
                                        id
                                    }
                                >
                                    {t(
                                        'clinicalconsultation:servicing-provider.PROVIDER-CITY'
                                    )}
                                </div>

                                <div
                                    className="servicing-provider-item-data-value"
                                    id={
                                        'consultation-servicing-provider-item-data-city-value-' +
                                        id
                                    }
                                >
                                    {hasMultiple(item.cities, 'name')
                                        ? t(
                                              'clinicalconsultation:servicing-provider.PROVIDER-MULTIPLE'
                                          )
                                        : item.cities?.length &&
                                          item.cities[0].name}
                                </div>
                            </div>
                        </div>
                        <div className="col-12 col-lg-6">
                            <div className="servicing-provider-item-data">
                                <div
                                    className="servicing-provider-item-data-name"
                                    id={
                                        'consultation-servicing-provider-item-data-zipCode-text-' +
                                        id
                                    }
                                >
                                    {t(
                                        'clinicalconsultation:servicing-provider.PROVIDER-ZIPCODE'
                                    )}
                                </div>

                                <div
                                    className="servicing-provider-item-data-value"
                                    id={
                                        'consultation-servicing-provider-item-data-zipcode-value-' +
                                        id
                                    }
                                >
                                    {hasMultiple(item.cities, 'zipCode')
                                        ? t(
                                              'clinicalconsultation:servicing-provider.PROVIDER-MULTIPLE'
                                          )
                                        : item.cities?.length &&
                                          item.cities[0].zipCode}
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-12 col-lg-3">
                            <div className="servicing-provider-item-data">
                                <div
                                    className="servicing-provider-item-data-name"
                                    id={
                                        'consultation-servicing-provider-item-data-billing-text-' +
                                        id
                                    }
                                >
                                    {t(
                                        'clinicalconsultation:servicing-provider.PROVIDER-BILLING'
                                    )}
                                </div>
                                <div
                                    className="servicing-provider-item-data-value"
                                    id={
                                        'consultation-servicing-provider-item-data-billing-value-' +
                                        id
                                    }
                                >
                                    {item.billingProviderNPI}
                                </div>
                            </div>
                        </div>
                        <div className="col-12 col-lg-9">
                            <div className="servicing-provider-item-data">
                                <div
                                    className="servicing-provider-item-data-name"
                                    id={
                                        'consultation-servicing-provider-item-data-facility-text-' +
                                        id
                                    }
                                >
                                    {t(
                                        'clinicalconsultation:servicing-provider.PROVIDER-FACILITY'
                                    )}
                                </div>
                                <div
                                    className="servicing-provider-item-data-value"
                                    id={
                                        'consultation-servicing-provider-item-data-facility-value-' +
                                        id
                                    }
                                >
                                    {item.facilityName}
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </CustomCheckbox>
            {hasMultiple(item.cities, 'name') ||
            hasMultiple(item.cities, 'zipCode') ? (
                <BottomSheet isOpen={isOpenCitySheet} onClose={onClose}>
                    <BottomSheetTitle>
                        {t(
                            'clinicalconsultation:servicing-provider.PROVIDER-SPECIALTY-CITY'
                        )}
                    </BottomSheetTitle>
                    <BottomSheetSubtitle>
                        {t(
                            'clinicalconsultation:servicing-provider.PROVIDER-SPECIALTY-CITY-MESSAGE'
                        )}
                    </BottomSheetSubtitle>
                    <BottomSheetList>
                        {item.cities.map((c, i: number) => (
                            <BottomSheetListItem
                                key={i}
                                name={c.name + ' - ' + c.zipCode}
                                onClick={handleOnSelectCity(c)}
                                formatItem={(c: CityInterface) =>
                                    c.name + ' - ' + c.zipCode
                                }
                                isActive={c == item.selectedCity}
                            />
                        ))}
                    </BottomSheetList>
                </BottomSheet>
            ) : null}
        </>
    );
};

export default ServicingProviderItem;
