import React, { useEffect, useState } from 'react';
import './App.scss';
import {
    CardWidgetTable,
    CardWidgetHeading,
    CardWidgetSubheading,
    CONST
} from '@provider-portal/components';
import { IconPlus, IconMinus, IconSwapArrows } from '@provider-portal/icons';
import { Collapse, Tooltip } from 'reactstrap';
import moment from 'moment';
import BottomSheetCancelNext from './BottomSheetCancelNext/BottomSheetCancelNext';
import { Consultation } from './models/Consultation';
import {
    DiagnosesInterface,
    DiagnosisInterface
} from './models/DiagnosisInterface';
import { AuditEventTypes } from './models/AuditEventTypes';
import { BeneficiaryInformation } from './models/BeneficiaryInformation';
import ClinicalConsultationService from './services/ClinicalConsultationService';
import { useTranslation } from 'react-i18next';
import { AuditEventGroups } from './models/AuditEventGroups';
import { formatPhoneNumber } from '../utilities/PhoneUtilities';

function App({
    onValidate,
    consultation,
    active,
    eventBus,
    beneficiaryId,
    submitAlertMessage
}: {
    eventBus: any;
    onValidate: (c: Consultation) => void;
    consultation: Consultation;
    active: boolean;
    beneficiaryId: string;
    submitAlertMessage: string;
}) {
    const [isOpenCollapse, setIsOpenCollapse] = useState(false);
    const [isOpenSheet, setIsOpenSheet] = useState(false);
    const [reviewData, setReviewData] = useState<Consultation>();
    const [beneficiaryInformation, setBeneficiaryInformation] =
        useState<BeneficiaryInformation>(null);
    const [isValidForm, setIsValidForm] = useState(false);
    const { t } = useTranslation();
    const [tooltipOpen, setTooltipOpen] = useState(false);
    const toggle = () => setTooltipOpen(!tooltipOpen);
    const [isSubmitClicked, setIsSubmitClicked] = useState(false);

    const isMobile = () => !!window.matchMedia(CONST.MQ_MOBILE_DOWN).matches;

    useEffect(() => {
        if (consultation) setReviewData(consultation);
        let isCompleted = isConsultationCompleted(consultation)
        setIsValidForm(isCompleted);
        if (isMobile() && !isValidForm) setIsOpenSheet(true);

    }, [consultation]);

    const isConsultationCompleted = (consultation: Consultation) => {

        return (
            consultation.consultationProvider != null &&
            consultation.date != null &&
            consultation.diagnoses.length > 0 &&
            consultation.requestingProvider != null &&
            consultation.procedure != null &&
            consultation.procedureQty != null

        );
    };

    useEffect(() => {
        ClinicalConsultationService.getInstance()
            .GetBeneficiaryInformation(beneficiaryId)
            .then(response => {
                if (response.data) {
                    setBeneficiaryInformation(response.data);
                }
            });
    }, []);

    useEffect(() => {
        function expandStep(stepIndex: number) {
            if (stepIndex == 5) setIsOpenCollapse(true);
        }
        eventBus.on('clinical-consultation-expand-step', expandStep);
        return () => {
            eventBus.off('clinical-consultation-expand-step', expandStep);
        };
    }, [eventBus]);

    useEffect(() => {
        if (active) {
            if (isMobile())
                setTimeout(() => {
                    setIsOpenCollapse(active);
                    setIsOpenSheet(true);
                }, 500); // time to allow collapse expand and page scroll

        }
        setIsOpenCollapse(active);

        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [active]);

    const getPrimaryDiagnosis = (diagnoses: DiagnosesInterface) => {
        const found = diagnoses?.find((d: DiagnosisInterface) => d.isPrimary);
        return found ? `${found.code} - ${found.description}` : '';
    };

    const handleClickNext = () => {
        if (isMobile() && !isValidForm) {
            setIsSubmitClicked(true);
            ClinicalConsultationService.getInstance().logAuditEvent(
                AuditEventTypes.ConsultationReviewError,
                AuditEventGroups.ClinicalConsultation,
                {
                    beneficiaryName: beneficiaryInformation.displayName,
                    beneficiaryId: beneficiaryInformation.beneficiaryId,
                    errorMessage: submitAlertMessage
                }
            );
            setIsOpenSheet(true);
            return;

        }
        onValidate(consultation);
        setIsOpenSheet(false);

    };

    const handleOnClose = () => setIsOpenSheet(false);


    const handleCancelClick = () => {
        setIsOpenSheet(false);
        eventBus.emit('cancel-consultation-create', {
            auditEvent: AuditEventTypes.ConsultationReviewCancelYesClick,
            auditData: {
                beneficiaryName: beneficiaryInformation.displayName,
                beneficiaryId: beneficiaryInformation.beneficiaryId
            }
        });
    };

    const toggleCollapse = () => setIsOpenCollapse(!isOpenCollapse);

    useEffect(() => {
        if (active) {
            if (isMobile()) setIsOpenSheet(true);
            setIsOpenCollapse(true);

        }
    }, [isValidForm]);


    useEffect(() => {
        const shouldOpen = isMobile() && isOpenCollapse;
        setIsOpenSheet(shouldOpen);
    }, [isOpenCollapse]);

    return (
        <div className="px-3">
            <div
                className="d-flex align-items-center"
                role="button"
                tabIndex={0}
                onClick={toggleCollapse}
            >
                <div>
                    <CardWidgetHeading id="clinical-consultation-review-header">
                        {t('clinicalconsultation:review.CONSULTATION-REVIEW')}
                    </CardWidgetHeading>
                    <CardWidgetSubheading id="clinical-consultation-review-subheader">
                        {t('clinicalconsultation:review.REVIEW-INFORMATION')}
                    </CardWidgetSubheading>
                </div>
                <div className="ml-auto mr-3">
                    {isOpenCollapse ? (
                        <IconMinus
                            width="1.5rem"
                            height="1.5rem"
                            id="clinical-consultation-review-icon-minus"
                        />
                    ) : (
                        <IconPlus
                            width="1.5rem"
                            height="1.5rem"
                            id="clinical-consultation-review-icon-plus"
                        />
                    )}
                </div>
            </div>

            <Collapse isOpen={isOpenCollapse}>
                {reviewData ? (
                    <div className="consultation-review mt-3 ml-1">
                        <div className="row">
                            <div className="col-md-6 mb-1">
                                <div className="consultation-review-data">
                                    <div
                                        className="consultation-review-data-name"
                                        id="clinical-consultation-review-beneficiary-name-label"
                                    >
                                        {t(
                                            'clinicalconsultation:review.BENEFICIARY-NAME'
                                        )}
                                    </div>
                                    <div
                                        className="consultation-review-data-value"
                                        id="clinical-consultation-review-beneficiary-name-value"
                                    >
                                        {beneficiaryInformation?.displayName}
                                    </div>
                                </div>
                            </div>
                            <div className="col-md-6 mb-1">
                                <div className="consultation-review-data">
                                    <div
                                        className="consultation-review-data-name"
                                        id="clinical-consultation-review-beneficiary-id-label"
                                    >
                                        {t(
                                            'clinicalconsultation:review.BENEFICIARY-ID'
                                        )}
                                    </div>
                                    <div
                                        className="consultation-review-data-value"
                                        id="clinical-consultation-review-beneficiary-id-value"
                                    >
                                        {beneficiaryInformation?.cardNumber}
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-md-6 mb-1">
                                <div className="consultation-review-data">
                                    <div
                                        className="consultation-review-data-name"
                                        id="clinical-consultation-review-primary-diagnoses-label"
                                    >
                                        {t(
                                            'clinicalconsultation:review.PRIMARY-DIAGNOSIS'
                                        )}
                                        :
                                    </div>
                                    <div
                                        className="consultation-review-data-value"
                                        id="clinical-consultation-review-primary-diagnoses-value"
                                    >
                                        {getPrimaryDiagnosis(
                                            reviewData.diagnoses
                                        )}
                                    </div>
                                </div>
                            </div>
                            <div className="col-md-6 mb-1">
                                <div className="consultation-review-data d-flex align-items-center mb-1">
                                    <div
                                        className="consultation-review-data-name"
                                        id="clinical-consultation-review-consultation-date-label"
                                    >
                                        {t(
                                            'clinicalconsultation:additional-information.CONSULTATION-DATE'
                                        )}
                                        :
                                    </div>
                                    <div
                                        className="consultation-review-data-value"
                                        id="clinical-consultation-review-consultation-date"
                                    >
                                        {moment(reviewData.date).format(
                                            'MM/DD/YYYY'
                                        )}
                                    </div>
                                </div>
                                <div className="consultation-review-data d-flex align-items-center">
                                    <div
                                        className="consultation-review-data-name"
                                        id="clinical-consultation-review-additional-health-plan-label"
                                    >
                                        {t(
                                            'clinicalconsultation:review.ADDITIONAL-HEALTH-PLAN'
                                        )}
                                        :
                                    </div>
                                    <div
                                        className="consultation-review-data-value"
                                        id="clinical-consultation-review-additional-health-plan-value"
                                    >
                                        {reviewData.healthPlan
                                            ?.additionalHealthPlanName ?? '-'}
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-md-6 mb-1">
                                <div className="consultation-review-data d-flex align-items-center flex-wrap">
                                    <div
                                        className="consultation-review-data-name"
                                        id="clinical-consultation-review-services-label"
                                    >
                                        {t(
                                            'clinicalconsultation:step.SERVICES'
                                        )}
                                        :
                                    </div>
                                    <div
                                        className="consultation-review-data-value"
                                        id="clinical-consultation-review-services-value"
                                    >
                                        {reviewData.procedure?.description}
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="consultation-review-data mb-3 d-flex align-items-center">
                            <div
                                className="consultation-review-data-name"
                                id="clinical-consultation-review-additional-information-reason-label"
                            >
                                {t(
                                    'clinicalconsultation:additional-information.REASON'
                                )}
                                :
                            </div>
                            <div
                                className="consultation-review-data-value"
                                id="clinical-consultation-review-additional-information-reason-value"
                            >
                                {reviewData.reason != ''
                                    ? reviewData.reason
                                    : '-'}
                            </div>
                        </div>
                    </div>
                ) : null}
                <div className="d-mobile-flex align-items-center">
                    {reviewData?.requestingProvider ? (
                        <div className="w-100 mr-mobile-2">
                            <div className="consultation-review-table">
                                <CardWidgetTable
                                    headers={[
                                        t(
                                            'clinicalconsultation:review.REQUESTING-TITLE'
                                        )
                                    ]}
                                    id="clinical-consultation-review-requesting-title"
                                >
                                    <tbody>
                                        <tr>
                                            <td>
                                                <div className="consultation-review-provider">
                                                    <div
                                                        className="consultation-review-provider-name mb-1"
                                                        id="clinical-consultation-review-consultation-review-provider-name"
                                                    >
                                                        {
                                                            reviewData
                                                                .requestingProvider
                                                                .renderingProviderName
                                                        }
                                                    </div>
                                                    <div className="consultation-review-provider-data">
                                                        <div
                                                            className="consultation-review-provider-data-name"
                                                            id="consultation-review-provider-specialties-label"
                                                        >
                                                            {t(
                                                                'clinicalconsultation:servicing-provider.SPECIALTIES'
                                                            )}
                                                            :
                                                        </div>
                                                        <div
                                                            className="consultation-review-provider-data-value"
                                                            id="consultation-review-provider-specialties-value"
                                                        >
                                                            {reviewData.requestingProvider.specialties
                                                                ?.map(
                                                                    s => s.name
                                                                )
                                                                .join(', ')}
                                                        </div>
                                                    </div>
                                                    <div className="consultation-review-provider-data">
                                                        <div
                                                            className="consultation-review-provider-data-name"
                                                            id="consultation-review-provider-phone-label"
                                                        >
                                                            {t(
                                                                'clinicalconsultation:requesting-provider.PROVIDER-PHONE'
                                                            )}
                                                        </div>
                                                        <div
                                                            className="consultation-review-provider-data-value"
                                                            id="consultation-review-provider-phone-value"
                                                        >
                                                            {
                                                                reviewData
                                                                    .requestingProvider
                                                                    .phoneNumber ? formatPhoneNumber(reviewData
                                                                        .requestingProvider
                                                                        .phoneNumber) : '-'
                                                            }
                                                        </div>
                                                    </div>
                                                    <div className="consultation-review-provider-data">
                                                        <div
                                                            className="consultation-review-provider-data-name"
                                                            id="consultation-review-provider-email-label"
                                                        >
                                                            {t(
                                                                'clinicalconsultation:requesting-provider.PROVIDER-EMAIL'
                                                            )}
                                                        </div>
                                                        <div
                                                            className="consultation-review-provider-data-value"
                                                            id="consultation-review-provider-email-value"
                                                        >
                                                            {
                                                                reviewData
                                                                    .requestingProvider
                                                                    .email
                                                            }
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </CardWidgetTable>
                            </div>
                        </div>
                    ) : (
                        <div className="w-100 mr-mobile-2 no-requesting-height">
                            <div className="consultation-review-table">
                                <CardWidgetTable
                                    headers={[
                                        t(
                                            'clinicalconsultation:review.REQUESTING-TITLE'
                                        )
                                    ]}

                                    id="clinical-consultation-review-requesting-no-provider">
                                    <tbody>
                                        <tr>
                                            <td>

                                                <div className="pl-3 table">
                                                    {t(
                                                        'clinicalconsultation:requesting-provider.NO-PROVIDER'
                                                    )}
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </CardWidgetTable>
                            </div>
                        </div>
                    )}

                    <div className="consultation-review-icon-container">
                        <div
                            className="consultation-review-icon"
                            id="consultation-review-icon-arrows"
                        >
                            <IconSwapArrows width="1rem" height="1rem" />
                        </div>
                    </div>

                    {reviewData?.consultationProvider ? (
                        <div className="w-100 ml-mobile-2">
                            <div className="consultation-review-table consultation-provider">
                                <CardWidgetTable
                                    headers={[
                                        t(
                                            'clinicalconsultation:review.CONSULTATION-TITLE'
                                        )
                                    ]}
                                    id="clinical-consultation-review-consultation-title"
                                >
                                    <tbody>
                                        <tr>
                                            <td>
                                                <div className="consultation-review-provider">
                                                    <div
                                                        className="consultation-review-provider-name mb-1"
                                                        id="clinical-consultation-review-consultation-provider-name"
                                                    >
                                                        {reviewData
                                                            .consultationProvider
                                                            .anyContractedSpecialist ===
                                                            true
                                                            ? t(
                                                                'clinicalconsultation:common.ANY-CONTRACTED-SPECIALIST'
                                                            )
                                                            : reviewData
                                                                .consultationProvider
                                                                .billingProviderName ||
                                                            '-'}
                                                    </div>
                                                    <div className="consultation-review-provider-data">
                                                        <div
                                                            className="consultation-review-provider-data-name"
                                                            id="clinical-consultation-review-consultation-provider-specialty-label"
                                                        >
                                                            {t(
                                                                'clinicalconsultation:requesting-provider.PROVIDER-SPECIALTY'
                                                            )}
                                                        </div>
                                                        <div
                                                            className="consultation-review-provider-data-value"
                                                            id="clinical-consultation-review-consultation-provider-specialty-value"
                                                        >
                                                            {reviewData
                                                                .consultationProvider
                                                                .selectedSpecialty
                                                                ?.name || '-'}
                                                        </div>
                                                    </div>
                                                    <div className="consultation-review-provider-data">
                                                        <div
                                                            className="consultation-review-provider-data-name"
                                                            id="clinical-consultation-review-consultation-provider-phone-label"
                                                        >
                                                            {t(
                                                                'clinicalconsultation:requesting-provider.PROVIDER-PHONE'
                                                            )}
                                                        </div>
                                                        <div
                                                            className="consultation-review-provider-data-value"
                                                            id="clinical-consultation-review-consultation-provider-phone-label"
                                                        >
                                                            {reviewData
                                                                .consultationProvider
                                                                .phoneNumber ? formatPhoneNumber(reviewData
                                                                    .consultationProvider
                                                                    .phoneNumber) :
                                                                '-'}
                                                        </div>
                                                    </div>
                                                    <div className="consultation-review-provider-data">
                                                        <div
                                                            className="consultation-review-provider-data-name"
                                                            id="clinical-consultation-review-consultation-provider-email-label"
                                                        >
                                                            {t(
                                                                'clinicalconsultation:requesting-provider.PROVIDER-EMAIL'
                                                            )}
                                                        </div>
                                                        <div
                                                            className="consultation-review-provider-data-value"
                                                            id="clinical-consultation-review-consultation-provider-email-value"
                                                        >
                                                            {reviewData
                                                                .consultationProvider
                                                                .email || '-'}
                                                        </div>
                                                    </div>
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>
                                </CardWidgetTable>
                            </div>
                        </div>
                    ) : (
                        <div className="w-100 mr-mobile-2 no-requesting-height">
                            <div className="consultation-review-table consultation-provider">
                                <CardWidgetTable
                                    headers={[
                                        t(
                                            'clinicalconsultation:review.CONSULTATION-TITLE'
                                        )
                                    ]}

                                    id="clinical-consultation-review-consultation-no-provider"
                                >
                                    <tbody>
                                        <tr>
                                            <td>

                                                <div className="pl-3 table">
                                                    {t(
                                                        'clinicalconsultation:requesting-provider.NO-PROVIDER'
                                                    )}
                                                </div>
                                            </td>
                                        </tr>
                                    </tbody>

                                </CardWidgetTable>
                            </div>
                        </div>
                    )}
                </div>

                <div className="d-none d-mobile-block mt-3">
                    <div className="d-flex aling-items-center pt-4">
                        <button
                            type="button"
                            className="btn btn-md btn-info btn-info-gradient rounded-pill px-3 mr-3"
                            style={{ width: '133px' }}
                            onClick={handleClickNext}
                            disabled={!isValidForm}
                            id="clinical-consultation-review-next-button"
                        >
                            {t('clinicalconsultation:review.SUBMIT')}
                        </button>
                        {!isValidForm && !isMobile() ? (
                            <Tooltip
                                placement="right"
                                isOpen={tooltipOpen}
                                target={
                                    'clinical-consultation-review-next-button'
                                }
                                toggle={toggle}
                                trigger="hover focus"
                            >
                                <span
                                    id={`clinical-consultation-review-next-button-tooltip`}
                                >
                                    {submitAlertMessage}
                                </span>
                            </Tooltip>
                        ) : null}

                        <button
                            type="button"
                            className="btn btn-md btn-outline-secondary rounded-pill px-3 "
                            style={{ width: '133px' }}
                            onClick={handleCancelClick}
                            id="clinical-consulation-review-cancel-button"
                        >
                            {t(
                                'clinicalconsultation:requesting-provider.CANCEL'
                            )}
                        </button>
                    </div>
                </div>
            </Collapse>

            <BottomSheetCancelNext
                isOpen={isOpenSheet}
                id={"clinical-consulation-review-mobile"}
                onClose={handleOnClose}
                onCancel={handleCancelClick}
                onNext={handleClickNext}
                isValid={isValidForm}
                submitAlertMessage={submitAlertMessage}
                isMobile={isMobile()}
                isSubmitClicked={isSubmitClicked}
            />
        </div>
    );
}

export default App;
