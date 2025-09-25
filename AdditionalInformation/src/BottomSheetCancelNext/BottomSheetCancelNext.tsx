import React from 'react';
import './BottomSheetCancelNext.scss';
import { BottomSheet } from '@provider-portal/components';
import { useTranslation } from 'react-i18next';

export type BottomSheetCancelNextProps = {
    isOpen: boolean;
    onClose: () => void;
    onCancel: () => void;
    onNext: () => void;
    id: string;
};

const BottomSheetCancelNext: React.FC<BottomSheetCancelNextProps> = ({
    isOpen,
    onClose,
    onCancel,
    onNext,
    id
}) => {
    const { t } = useTranslation();
    return (
        <BottomSheet
            isOpen={isOpen}
            onClose={onClose}
            closeable={false}
            backdrop={false}
            modalClassName="referral-create-bottomsheet"
        >
            <div className="d-flex aling-items-center justify-content-between pb-1">
                <button
                    id={id + '-cancel-button'}
                    type="button"
                    className="btn btn-outline-secondary rounded-pill px-3"
                    style={{ width: '133px' }}
                    onClick={onCancel}
                >
                    {t('clinicalconsultation:requesting-provider.CANCEL')}
                </button>
                <button
                    id={id + '-next-button'}
                    type="button"
                    className="btn btn-info btn-info-gradient rounded-pill px-3"
                    style={{ width: '133px' }}
                    onClick={onNext}
                >
                    {t('clinicalconsultation:requesting-provider.NEXT')}
                </button>
            </div>
        </BottomSheet>
    );
};

export default BottomSheetCancelNext;
