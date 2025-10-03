import React, { useCallback, useEffect, useState } from 'react';
import './App.scss';
import FormDropdownSelect from '../../FormDropdownSelect/FormDropdownSelect';
import {
    CardWidgetHeading,
    CardWidgetSubheading,
    BottomSheetSelect,
    FlexInputDate,
    FlexInputArea,
    DropdownPicker,
    CONST
} from '@provider-portal/components';
import { HEALTH_PLANS } from './mock-data';
import { Collapse } from 'reactstrap';
import BottomSheetCancelNext from './BottomSheetCancelNext/BottomSheetCancelNext';
import { IconPlus, IconMinus } from '@provider-portal/icons';
import ClinicalConsultationService, {
    BeneficiaryInformation,
    ConsultationConfigurations,
    HealthPlanDto
} from './services/ClinicalConsultationService';
import { useTranslation } from 'react-i18next';
import { AuditEventTypes } from './models/AuditEventTypes';
import { AuditEventGroups } from './models/AuditEventGroups';
import CustomDateInput from './components/CustomDateInput';

function App({
    eventBus,
    onValidate,
    consultation,
    active,
    beneficiaryId,
    stepAlertMessage
}: {
    eventBus: any;
    onValidate: (c: Consultation) => void;
    consultation: Consultation;
    active: boolean;
    beneficiaryId: string;
    stepAlertMessage: string;
}) {
    const { t } = useTranslation();
    const [isOpenCollapse, setIsOpenCollapse] = useState(false);
    const [isOpenSheet, setIsOpenSheet] = useState(false);
    const [isValidForm, setIsValidForm] = useState(false);
    const [healthPlans, setHealthPlans] = useState<HealthPlanDto[]>([]);
    const [healthPlanOptions, setHealthPlanOptions] = useState<
        { value: string; label: string }[]
    >([]);
    const [selectedHealthPlanItem, setSelectedHealthPlanItem] = useState<{
        value: string;
        label: string;
    } | null>(null);
    const [selectedHealthPlan, setSelectedHealthPlan] = useState<string>('');
    const [isLoadingHealthPlans, setIsLoadingHealthPlans] =
        useState<boolean>(true);
    const [healthPlansError, setHealthPlansError] = useState<string>('');
    const [healthPlanValidationError, setHealthPlanValidationError] =
        useState<string>('');
    const [daysBackAllowed, setDaysBackAllowed] = useState<number>(1);
    const [
        consultationMaximumAllowedMessage,
        setconsultationMaximumAllowedMessage
    ] = useState<string>('');
    const [
        consultationMaximumAllowedValue,
        setconsultationMaximumAllowedValue
    ] = useState<number>(400);
    const [selectedDate, setSelectedDate] = useState<Date | null>(null);
    const [selectedReason, setSelectedReason] = useState('');
    const [reasonValue, setReasonValue] = useState('');
    const [wasUpdated, setWasUpdated] = useState(false);
    const [beneficiaryInformation, setBeneficiaryInformation] =
        useState<BeneficiaryInformation | null>(null);
    const [dateError, setDateError] = useState('');
    const [reasonCount, setReasonCount] = useState<number>(0);
    const [reasonError, setReasonError] = useState<string>('');
    const [ruleOutsMessage, setRuleOutsMessage] = useState<string>('');

    const isMobile = () => !!window.matchMedia(CONST.MQ_MOBILE_DOWN).matches;

    const onSelectHealthPlan = (selectedItem: { value: string; label: string }) => {
        setSelectedHealthPlan(selectedItem.value);
        setSelectedHealthPlanItem(selectedItem);
        setHealthPlanValidationError('');
    };

    const handleOnChangeDate = (value: Date | null | undefined) => {
        if (value && value instanceof Date && !isNaN(value.getTime())) {
            setSelectedDate(value);
        } else {
            setSelectedDate(null);
        }
    };

    const handleOnClearDate = () => {
        setSelectedDate(null);
    };

    const handleOnChangeReason = (
        e: React.ChangeEvent<HTMLTextAreaElement>
    ) => {
        const text = e.currentTarget.value;

        setReasonValue(text);
        setReasonCount(text.length);

        if (text.length >= consultationMaximumAllowedValue) {
            setReasonError(consultationMaximumAllowedMessage);
        } else {
            setReasonError('');
        }
    };

    const handleSelectedReason = () => {
        setSelectedReason(reasonValue);
    };

    // return an updated consultation object
    const getConsultation = useCallback(() => {
        return {
            ...consultation,
            reason: reasonValue,
            date: selectedDate,
            healthPlan: selectedHealthPlan
        };
    }, [consultation, reasonValue, selectedDate, selectedHealthPlan]);

    const updateWizardProgress = () => {
        setWasUpdated(true);
        onValidate(getConsultation());
    };

    const validateForm = () => {
        let valid = true;

        if (selectedHealthPlan === undefined || selectedHealthPlan === null) {
            valid = false;
            // No ejecutar updateWizardProgress cuando se limpia el health plan
            // para evitar que se abra el modal de cancelación
        } else {
            setHealthPlanValidationError('');
        }

        if (
            !selectedDate ||
            !(selectedDate instanceof Date) ||
            isNaN(selectedDate.getTime())
        ) {
            valid = false;
            setDateError(
                t(
                    'clinicalconsultation:additional-information.CONSULTATION-DATE-INVALID'
                )
            );
            if (wasUpdated) {
                setSelectedDate(null);
                updateWizardProgress();
            }
        } else {
            const earliest = new Date();
            earliest.setDate(earliest.getDate() - daysBackAllowed);
            if (selectedDate < earliest) {
                valid = false;
                setDateError(
                    t(
                        'clinicalconsultation:additional-information.CONSULTATION-DATE-INVALID'
                    )
                );
                if (wasUpdated) {
                    setSelectedDate(null);
                    updateWizardProgress();
                }
            } else {
                setDateError('');
            }
        }

        if (
            reasonValue &&
            reasonValue.length > consultationMaximumAllowedValue
        ) {
            valid = false;
            setReasonError(consultationMaximumAllowedMessage);
            if (wasUpdated) {
                setSelectedReason(null);
                updateWizardProgress();
            }
        } else {
            setReasonError('');
        }

        setIsValidForm(valid);
    };

    useEffect(() => {
        ClinicalConsultationService.getInstance()
            .GetBeneficiaryInformation(beneficiaryId)
            .then(response => {
                if (response.data) {
                    setBeneficiaryInformation(response.data);
                }
            });
    }, [beneficiaryId]);

    useEffect(() => {
        const loadConfigurations = async () => {
            try {
                const response =
                    await ClinicalConsultationService.getInstance().getConfigurations();
                const configData = response.data;

                setconsultationMaximumAllowedMessage(
                    configData.createRequestConsultationMaximumAllowedMessage
                );
                setconsultationMaximumAllowedValue(
                    configData.requestConsultationMaximumAllowedValue
                );
                setDaysBackAllowed(configData.consultationDaysBackAllowed);
                setRuleOutsMessage(configData.ruleOutsMessage );
            } catch (error) {}
        };

        loadConfigurations();
    }, []);

    useEffect(() => {
        if (active)
            setTimeout(() => {
                validateForm();
            }, 500); // time to allow collapse expand and page scroll
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [active]);

    useEffect(() => {
        if (active)
            if (selectedDate || selectedHealthPlan || selectedReason) {
                validateForm();
            }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [selectedDate, selectedHealthPlan, selectedReason]);

    const handleClickNext = () => {
        updateWizardProgress();

        if (beneficiaryInformation) {
            ClinicalConsultationService.getInstance().logAuditEvent(
                AuditEventTypes.AdditionalInformationNextClick,
                AuditEventGroups.ClinicalConsultation,
                {
                    beneficiaryId: beneficiaryInformation.beneficiaryId,
                    beneficiaryName: beneficiaryInformation.displayName
                }
            );
        }

        setIsOpenSheet(false);
    };

    useEffect(() => {
        const loadHealthPlans = async () => {
            try {
                setIsLoadingHealthPlans(true);
                setHealthPlansError('');

                const response =
                    await ClinicalConsultationService.getInstance().getHealthPlans();
                const healthPlansData =
                    response.data as unknown as HealthPlanDto[];
                setHealthPlans(healthPlansData);
                const options = [
                    {
                        value: '',
                        label: t('clinicalconsultation:additional-information.NO-HEALTH-PLAN')
                    },
                    ...healthPlansData.map(plan => ({
                        value: String(plan.additionalHealthPlanId),
                        label: plan.additionalHealthPlanName
                    }))
                ];
                setHealthPlanOptions(options);
                // No establecer selectedHealthPlanItem aquí, se manejará en el useEffect
            } catch (error) {
                setHealthPlansError('Error loading health plans');
            } finally {
                setIsLoadingHealthPlans(false);
            }
        };

        loadHealthPlans();
    }, []);

    useEffect(() => {
        if (consultation) {
            const { reason, healthPlan, date } = consultation;
            setSelectedReason(reason);
            setSelectedHealthPlan(healthPlan || '');
            setSelectedDate(date);
            setReasonCount((reason || '').length);
        }
    }, [consultation]);

    // Sincronizar selectedHealthPlanItem con selectedHealthPlan
    useEffect(() => {
        if (healthPlanOptions.length > 0) {
            const foundItem = healthPlanOptions.find(option => option.value === selectedHealthPlan);
            if (foundItem) {
                setSelectedHealthPlanItem(foundItem);
            } else {
                // Si no se encuentra, usar el primer elemento (No health plan) como placeholder
                setSelectedHealthPlanItem(healthPlanOptions[0]);
                // También actualizar selectedHealthPlan para mantener consistencia
                setSelectedHealthPlan(healthPlanOptions[0].value);
            }
        }
    }, [selectedHealthPlan, healthPlanOptions]);

    const handleOnCancel = () => {
        setIsOpenSheet(false);
        eventBus.emit('cancel-consultation-create', {
            auditEvent: AuditEventTypes.RequestingProviderCancelYesClick,
            auditData: {
                beneficiaryName: beneficiaryInformation.displayName,
                beneficiaryId: beneficiaryInformation.beneficiaryId
            }
        });
        if (beneficiaryInformation) {
            ClinicalConsultationService.getInstance().logAuditEvent(
                AuditEventTypes.AdditionalInformationCancelYesClick,
                AuditEventGroups.ClinicalConsultation,
                {
                    beneficiaryId: beneficiaryInformation.beneficiaryId,
                    beneficiaryName: beneficiaryInformation.displayName
                }
            );
        }
    };

    const handleOnCloseSheet = () => {
        setIsOpenSheet(false);
    };

    const toggleCollapse = () => setIsOpenCollapse(!isOpenCollapse);

    useEffect(() => {
        setIsOpenCollapse(active);
    }, [active]);

    useEffect(() => {
        // Solo abrir el BottomSheet en móvil cuando el formulario es válido
        const shouldOpen = isMobile() && isOpenCollapse && isValidForm;
        setIsOpenSheet(shouldOpen);
    }, [isValidForm, isOpenCollapse]);

    useEffect(() => {
        if (!consultation?.date) {
            setSelectedDate(new Date());
        }
    }, [consultation?.date]);

    useEffect(() => {
        if (wasUpdated) {
            validateForm();
        }
    }, [selectedDate, selectedReason, wasUpdated, validateForm]); // Removido selectedHealthPlan

    return (
        <div className="px-3">
            <div
                className="d-flex align-items-center"
                role="button"
                tabIndex={0}
                onClick={toggleCollapse}
            >
                <div>
                    <CardWidgetHeading>
                        Additional Information
                    </CardWidgetHeading>
                    <CardWidgetSubheading>
                        Provide the corresponding information.
                    </CardWidgetSubheading>
                </div>
                <div className="ml-auto mr-3">
                    {isOpenCollapse ? (
                        <IconMinus width="1.5rem" height="1.5rem" />
                    ) : (
                        <IconPlus width="1.5rem" height="1.5rem" />
                    )}
                </div>
            </div>

            <Collapse isOpen={isOpenCollapse}>
                <div className="row">
                    <div className="col-12 col-xxl-8">
                        <div className="row mt-3">
                            <div className="col-mobile-5">
                                <CustomDateInput
                                    label={t('clinicalconsultation:additional-information.CONSULTATION-DATE')}
                                    placeholder={t('clinicalconsultation:additional-information.SELECT-DATE')}
                                    onChangeDate={handleOnChangeDate}
                                    value={selectedDate || undefined}
                                    onClear={handleOnClearDate}
                                />
                                {dateError && (
                                    <div
                                        id="create-additional-info-date-error-msg"
                                        className="text-danger text-sm mt-1"
                                    >
                                        {dateError}
                                    </div>
                                )}
                            </div>
                            <div className="col-mobile-7">
                                <div className="d-none d-mobile-block">
                                    <label
                                        htmlFor="health-plan-select-mobile"
                                        className="flex-input-label"
                                    >
                                        {t('clinicalconsultation:additional-information.ADDITIONAL-HEALTH-PLAN')}
                                    </label>
                                    <FormDropdownSelect
                                        key={`health-plan-${healthPlanOptions.length}`}
                                        items={healthPlanOptions}
                                        value={selectedHealthPlanItem}
                                        onChange={onSelectHealthPlan}
                                        formatItem={(item: { value: string; label: string }) => item?.label || ''}
                                        formatSelected={(item: { value: string; label: string }) => item?.label || ''}
                                        filterable={true}
                                        disabled={isLoadingHealthPlans}
                                        noItemsText={t('clinicalconsultation:additional-information.NO-HEALTH-PLANS-FOUND')}
                                    />
                                </div>
                                <div className="d-mobile-none mt-3">
                                    <label
                                        htmlFor="health-plan-select-desktop"
                                        className="flex-input-label"
                                    >
                                        {t('clinicalconsultation:additional-information.ADDITIONAL-HEALTH-PLAN')}
                                    </label>
                                    <FormDropdownSelect
                                        key={`health-plan-${healthPlanOptions.length}`}
                                        items={healthPlanOptions}
                                        value={selectedHealthPlanItem}
                                        onChange={onSelectHealthPlan}
                                        formatItem={(item: { value: string; label: string }) => item?.label || ''}
                                        formatSelected={(item: { value: string; label: string }) => item?.label || ''}
                                        filterable={true}
                                        disabled={isLoadingHealthPlans}
                                        noItemsText={t('clinicalconsultation:additional-information.NO-HEALTH-PLANS-FOUND')}
                                    />
                                </div>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-12 mt-3">
                                <FlexInputArea
                                    label={t('clinicalconsultation:additional-information.REASON-TO-REQUEST')}
                                    placeholder={t('clinicalconsultation:additional-information.REASON')}
                                    rows="5"
                                    onChange={handleOnChangeReason}
                                    value={reasonValue}
                                    onBlur={handleSelectedReason}
                                    maxLength={consultationMaximumAllowedValue}
                                    className={reasonValue.length >= consultationMaximumAllowedValue ? 'border-danger' : ''}
                                />
                                <div
                                    id="create-additional-info-disclaimer"
                                    className="text-muted mt-2"
                                >
                                    {ruleOutsMessage}
                                </div>
                                {reasonError && (
                                    <div
                                        id="create-additional-info-reason-max-msg"
                                        className="text-danger text-sm mt-1"
                                    >
                                        {reasonError}
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
                {!isValidForm && wasUpdated ? (
                    <div
                        className="d-none d-mobile-block text-danger text-sm pl-2 pt-2 ml-3"
                        id="clinical-consultation-requesting-provider-alert-message"
                    >
                        {stepAlertMessage}
                    </div>
                ) : null}
                <div className="d-none d-mobile-block mt-3">
                    <div className="d-flex aling-items-center">
                        <button
                            id="create-additional-info-next-btn"
                            type="button"
                            className="btn btn-md btn-info btn-info-gradient rounded-pill px-3 mr-3"
                            style={{ width: '133px' }}
                            onClick={handleClickNext}
                            disabled={!isValidForm}
                        >
                            Next
                        </button>
                        <button
                            type="button"
                            className="btn btn-md btn-outline-secondary rounded-pill px-3 "
                            style={{ width: '133px' }}
                            onClick={handleOnCancel}
                        >
                            Cancel
                        </button>
                    </div>
                </div>
            </Collapse>

            <BottomSheetCancelNext
                isOpen={isOpenSheet}
                id="clinical-consultation-additional-information-mobile"
                onClose={handleOnCloseSheet}
                onCancel={handleOnCancel}
                onNext={handleClickNext}
            />
        </div>
    );
}

export default App; 
