import React, { useEffect, useState } from 'react';
import './AnyContractedSpecialistSelectedItem.scss';
import { useTranslation } from 'react-i18next';

export type AnyContractedSpecialistSelectedItemProps = {
    item: ProviderInterface;
    id: string;
};

const AnyContractedSpecialistSelectedItem: React.FC<
    AnyContractedSpecialistSelectedItemProps
> = ({ item, id }) => {
    const { t } = useTranslation();

    return (
        <>
            <div className="servicing-provider-item">
                <div
                    className="servicing-provider-item-name-anyspecialist"
                    id={`${id}-name`}
                >
                    {t('clinicalconsultation:common.ANY-CONTRACTED-SPECIALIST')}
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

                            <div
                                className="servicing-provider-item-data-value flex-grow-1"
                                id={`${id}-specialties`}
                            >
                                {item.selectedSpecialty?.name || '-'}
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </>
    );
};

export default AnyContractedSpecialistSelectedItem;
