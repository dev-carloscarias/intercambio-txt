import React, { useEffect, useState } from 'react';
import './BottomSheetCancelNext.scss';
import { BottomSheet } from '@provider-portal/components';
import { useTranslation } from 'react-i18next';
import { Tooltip } from 'reactstrap';
export type BottomSheetCancelNextProps = {
    isOpen: boolean;
    onClose: () => void;
    onCancel: () => void;
    onNext: () => void;
     id: string;
    isValid?: boolean; 
    submitAlertMessage?:string;
    isMobile?:boolean;
    isSubmitClicked?:boolean;
};

const BottomSheetCancelNext: React.FC<BottomSheetCancelNextProps> = ({
    isOpen,
    onClose,
    onCancel,
    onNext,
    id,
    isValid,
    submitAlertMessage,
    isMobile,
    isSubmitClicked    
}) => {
    const { t } = useTranslation();
    const [tooltipOpen, setTooltipOpen] = useState(false);
    const toggle = () => setTooltipOpen(!tooltipOpen);

    
    
    
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
                    id={id + '-submit-button'}
                    type="button"
                    className= {!isValid ? "btn btn-info btn-info-gradient rounded-pill px-3 disabled" : "btn btn-info btn-info-gradient rounded-pill px-3 "}
                    style={{ width: '133px' }}
                    onClick={onNext}                    
                >
                    {t('clinicalconsultation:review.SUBMIT')}
                </button>
                {!isValid && isMobile && isSubmitClicked ? (
                    <Tooltip
                        placement="left"
                        isOpen={!tooltipOpen}
                        target={id+ '-submit-button'}
                        toggle={toggle}
                        trigger="focus"
                    >
                        <span id={`${id}-tooltip`}>
                            {submitAlertMessage}
                        </span>
                    </Tooltip>
            ) : null}
            </div>
        </BottomSheet>
    );
};

export default BottomSheetCancelNext;
