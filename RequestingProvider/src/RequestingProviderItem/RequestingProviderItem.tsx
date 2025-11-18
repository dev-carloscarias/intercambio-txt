import React, { useState } from 'react';
import './RequestingProviderItem.scss';
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
    IconInformation,
    IconStarRounded,
    IconRibbonCheck
} from '@provider-portal/icons';
import { useTranslation } from 'react-i18next';

export interface RequestingProviderInterface extends ProviderInterface {
    isStarred?: boolean;
    isFeatured?: boolean;
    information?: string;
}

export type RequestingProviderItemProps = {
    item: RequestingProviderInterface;
    // callback with item marked as selected
    onCheck: (i: RequestingProviderInterface) => void;
    // keep checked state in consecutive openings
    checked: boolean;
    // display bottom sheet to select city? intended for mobile only
    autoSelectCity?: boolean;
    id?: number;
};

const RequestingProviderItem: React.FC<RequestingProviderItemProps> = ({
    item,
    onCheck,
    checked,
    autoSelectCity = false,
    id
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
            if (autoSelectCity && isMultipleCity(item) && !item.selectedCity) {
                e.preventDefault();
                setIsOpenCitySheet(true);
            } else onCheck(item);
        } else onCheck(null);
    };

    const isMultipleCity = (provider: RequestingProviderInterface) => {
        return provider?.cities?.length > 1;
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
            >
                <div className="requesting-provider-item">
                    {item.information ? (
                        <div
                            className="requesting-provider-item-icon-information"
                            title={item.information}
                            id={
                                'clinical-consultation-requesting-provider-information-icon-' +
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
                            className="requesting-provider-item-icon-feature"
                            id={
                                'clinical-consultation-requesting-provider-feature-icon-' +
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
                        <div className="requesting-provider-item-icon-feature">
                            <WithTooltip text={'Information text'}>
                                <IconStarRounded
                                    width="1.5rem"
                                    height="1.5rem"
                                    onClick={(e: React.MouseEvent) =>
                                        e.preventDefault()
                                    }
                                    id={
                                        'clinical-consultation-requesting-provider-star-icon-' +
                                        id
                                    }
                                />
                            </WithTooltip>
                        </div>
                    ) : null}

                    <div
                        className="requesting-provider-item-name"
                        id={
                            'clinical-consultation-requesting-provider-item-name-value-' +
                            id
                        }
                    >
                        {item.renderingProviderName}
                    </div>
                    <div className="row">
                        <div className="col-12">
                            <div className="requesting-provider-item-data">
                                <div
                                    className="requesting-provider-item-data-name"
                                    id={
                                        'clinical-consultation-requesting-provider-item-data-speciality-text-' +
                                        id
                                    }
                                >
                                    {t(
                                        'clinicalconsultation:requesting-provider.PROVIDER-SPECIALTY'
                                    )}
                                </div>
                                <div
                                    className="requesting-provider-item-data-value"
                                    id={
                                        'clinical-consultation-requesting-provider-item-data-speciality-value-' +
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
                            <div className="requesting-provider-item-data">
                                <div
                                    className="requesting-provider-item-data-name"
                                    id={
                                        'clinical-consultation-requesting-provider-item-data-npi-text-' +
                                        id
                                    }
                                >
                                    {t(
                                        'clinicalconsultation:requesting-provider.PROVIDER-NPI'
                                    )}
                                </div>
                                <div
                                    className="requesting-provider-item-data-value"
                                    id={
                                        'clinical-consultation-requesting-provider-item-data-npi-value-' +
                                        id
                                    }
                                >
                                    {item.renderingProviderNPI}
                                </div>
                            </div>
                        </div>
                        <div className="col-12 col-lg-9">
                            <div className="requesting-provider-item-data">
                                <div
                                    className="requesting-provider-item-data-name"
                                    id={
                                        'clinical-consultation-requesting-provider-item-data-city-text-' +
                                        id
                                    }
                                >
                                    {t(
                                        'clinicalconsultation:requesting-provider.PROVIDER-CITY'
                                    )}
                                </div>

                                <div
                                    className="requesting-provider-item-data-value"
                                    id={
                                        'clinical-consultation-requesting-provider-item-data-city-value-' +
                                        id
                                    }
                                >
                                    {isMultipleCity(item)
                                        ? t(
                                              'clinicalconsultation:requesting-provider.PROVIDER-MULTIPLE'
                                          )
                                        : item.cities?.length &&
                                          item.cities[0].name}
                                </div>
                            </div>
                        </div>
                    </div>
                    <div className="row">
                        <div className="col-12 col-lg-3">
                            <div className="requesting-provider-item-data">
                                <div
                                    className="requesting-provider-item-data-name"
                                    id={
                                        'clinical-consultation-requesting-provider-item-data-billing-text-' +
                                        id
                                    }
                                >
                                    {t(
                                        'clinicalconsultation:requesting-provider.PROVIDER-BILLING'
                                    )}
                                </div>
                                <div
                                    className="requesting-provider-item-data-value"
                                    id={
                                        'clinical-consultation-requesting-provider-item-data-billing-value-' +
                                        id
                                    }
                                >
                                    {item.billingProviderNPI}
                                </div>
                            </div>
                        </div>
                        <div className="col-12 col-lg-9">
                            <div className="requesting-provider-item-data">
                                <div
                                    className="requesting-provider-item-data-name"
                                    id={
                                        'clinical-consultation-requesting-provider-item-data-facility-text-' +
                                        id
                                    }
                                >
                                    {t(
                                        'clinicalconsultation:requesting-provider.PROVIDER-FACILITY'
                                    )}
                                </div>
                                <div
                                    className="requesting-provider-item-data-value"
                                    id={
                                        'clinical-consultation-requesting-provider-item-data-facility-value-' +
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
            {isMultipleCity(item) ? (
                <BottomSheet isOpen={isOpenCitySheet} onClose={onClose}>
                    <BottomSheetTitle>
                        {t(
                            'clinicalconsultation:requesting-provider.PROVIDER-SPECIALTY-CITY'
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
                                formatItem={(c: CityInterface) => c.name}
                                isActive={c == item.selectedCity}
                            />
                        ))}
                    </BottomSheetList>
                </BottomSheet>
            ) : null}
        </>
    );
};

export default RequestingProviderItem;
