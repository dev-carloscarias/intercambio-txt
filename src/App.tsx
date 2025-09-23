import React, { useCallback, useEffect, useState } from 'react';
import './App.scss';
import {
    CardWidgetHeading,
    CardWidgetSubheading,
    BottomSheetSelect,
    FlexInputDate,
    FlexInputArea,
    DropdownPicker,
    CONST
} from '@provider-portal/components';
import Select from 'react-select';
import CustomDateInput from './components/CustomDateInput';
import { Collapse } from 'reactstrap';
import BottomSheetCancelNext from './BottomSheetCancelNext/BottomSheetCancelNext';
import { IconPlus, IconMinus } from '@provider-portal/icons';
import ClinicalConsultationService, { BeneficiaryInformation, ConsultationConfigurations } from './services/ClinicalConsultationService';
import { AuditEventTypes } from './models/AuditEventTypes';
import { AuditEventGroups } from './models/AuditEventGroups';

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
    stepAlertMessage: string
}) {
    const [isOpenCollapse, setIsOpenCollapse] = useState(false);
    const [isOpenSheet, setIsOpenSheet] = useState(false);
    const [isValidForm, setIsValidForm] = useState(false);
    const [healthPlans, setHealthPlans] = useState<string[]>([]);
    const [healthPlanOptions, setHealthPlanOptions] = useState<{value: string, label: string}[]>([]); 
    const [selectedHealthPlan, setSelectedHealthPlan] = useState<string>('');
    const [isLoadingHealthPlans, setIsLoadingHealthPlans] = useState<boolean>(false);
    const [healthPlansError, setHealthPlansError] = useState<string>('');
    const [healthPlanValidationError, setHealthPlanValidationError] = useState<string>('');
    const [daysBackAllowed, setDaysBackAllowed] = useState<number>(1);
    const [consultationMaximumAllowedMessage, setconsultationMaximumAllowedMessage] = useState<string>('');
    const [consultationMaximumAllowedValue, setconsultationMaximumAllowedValue] = useState<number>(400);
    const [selectedDate, setSelectedDate] = useState<Date | null>(null);
    const [selectedReason, setSelectedReason] = useState('');
    const [reasonValue, setReasonValue] = useState('');
    const [wasUpdated, setWasUpdated] = useState(false);
    const [beneficiaryInformation, setBeneficiaryInformation] =
        useState<BeneficiaryInformation>(null);
    const [dateError, setDateError] = useState('');
    const [reasonCount, setReasonCount] = useState<number>(0);
    const [reasonError, setReasonError] = useState<string>('');

    const isMobile = () => !!window.matchMedia(CONST.MQ_MOBILE_DOWN).matches;

    // Estilos personalizados para react-select
    const customSelectStyles = {
        control: (provided: any, state: any) => ({
            ...provided,
            minHeight: '38px',
            borderColor: state.isFocused ? '#007bff' : '#ced4da',
            boxShadow: state.isFocused ? '0 0 0 0.2rem rgba(0, 123, 255, 0.25)' : 'none',
            '&:hover': {
                borderColor: '#007bff'
            }
        }),
        placeholder: (provided: any) => ({
            ...provided,
            color: '#6c757d'
        }),
        option: (provided: any, state: any) => ({
            ...provided,
            backgroundColor: state.isSelected ? '#007bff' : state.isFocused ? '#f8f9fa' : 'white',
            color: state.isSelected ? 'white' : '#212529'
        })
    };


    const onSelectHealthPlan = (selectedOption: {value: string, label: string} | null) => {
        if (selectedOption) {
            setSelectedHealthPlan(selectedOption.value);
        } else {
            setSelectedHealthPlan('');
        }
    };



    const handleOnChangeDate = (value: Date | null) => {
        // Validar que el valor no sea null y sea una fecha válida
        if (value && value instanceof Date && !isNaN(value.getTime())) {
            setSelectedDate(value);
        } else {
            // Si el valor es inválido, mantener la fecha actual o establecer null
            console.warn('Invalid date value received:', value);
            setSelectedDate(null);
        }
    };

    const handleOnClearDate = () => {
        setSelectedDate(null);
    };

    const handleOnChangeReason = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
        const text = e.currentTarget.value;
        
        
        // Siempre actualizar el estado para permitir borrado
        setReasonValue(text);
        setReasonCount(text.length);
        
        // Mostrar error si alcanza o excede el límite
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
    }

    const validateForm = () => {
        let valid = true;

        if (!selectedHealthPlan || selectedHealthPlan === 'No Health Plan') {
            valid = false;
            setHealthPlanValidationError('Please select a valid health plan');
        } else {
            setHealthPlanValidationError('');
        }
    
        if (!selectedDate || !(selectedDate instanceof Date) || isNaN(selectedDate.getTime())){
            valid = false;
            setDateError('*Consultation Date is Invalid');
        }
        else {
            const earliest = new Date();
            earliest.setDate(earliest.getDate() - daysBackAllowed);
            if (selectedDate < earliest) {
                valid = false;
                setDateError('*Consultation Date is invalid');
            }
            else {
                setDateError('');
            }
        }

        if (reasonValue && reasonValue.length >= consultationMaximumAllowedValue) {
            valid = false;
            setReasonError(consultationMaximumAllowedMessage);
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
    }, []);

    useEffect(() => {
        const loadConfigurations = async () => {
            try {
                const response = await ClinicalConsultationService.getInstance().getConfigurations();
                const configData = response.data;

                setconsultationMaximumAllowedMessage(configData.CreateRequestConsultationMaximumAllowedMessage);
                setconsultationMaximumAllowedValue(response.data.RequestConsultationMaximumAllowedValue);
                setDaysBackAllowed(configData.ConsultationDaysBackAllowed);
                console.log('Configuraciones Cargadas', configData);

            } catch (error) {
                console.log('Error loading configurations');
            }
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
        if (active) {
            // Solo validar si hay valores válidos
            if (selectedDate || selectedHealthPlan || selectedReason) {
                validateForm();
            }
        }
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [selectedDate, selectedHealthPlan, selectedReason]);

    const handleClickNext = () => {
        updateWizardProgress();

        ClinicalConsultationService.getInstance().logAuditEvent(
            AuditEventTypes.AdditionalInformationNextClick,
            AuditEventGroups.ClinicalConsultation,
            {
                beneficiaryId: beneficiaryInformation.beneficiaryId,
                beneficiaryName: beneficiaryInformation.displayName
            }
        )

        setIsOpenSheet(false);
    };

    useEffect(() => {
        const loadHealthPlans = async () => {
            try {
                setIsLoadingHealthPlans(true);
                setHealthPlansError('');

                const response = await ClinicalConsultationService.getInstance().getHealthPlans();
                console.log("health plans: ", response.data)
                const healthPlansData = response.data;
                console.log("health plansDATA: ", healthPlansData)
                
                setHealthPlans(healthPlansData);
                // Crear opciones para react-select
                const options = healthPlansData.map(plan => ({
                    value: plan,
                    label: plan
                }));
                setHealthPlanOptions(options);

            }catch (error){
                setHealthPlansError('Error loading health plans');
            }finally {
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

    const handleOnCancel = () => {
        ClinicalConsultationService.getInstance().logAuditEvent(
            AuditEventTypes.AdditionalInformationCancelYesClick,
            AuditEventGroups.ClinicalConsultation,
            {
                beneficiaryId: beneficiaryInformation.beneficiaryId,
                beneficiaryName: beneficiaryInformation.displayName
            }
        );

        setIsOpenSheet(false);
    }

    const toggleCollapse = () => setIsOpenCollapse(!isOpenCollapse);

    useEffect(() => {
        setIsOpenCollapse(active);
    }, [active]);

    useEffect(()=> {
        const shouldOpen = isMobile() && isOpenCollapse && isValidForm;
        setIsOpenSheet(shouldOpen );            
    }, [isValidForm , isOpenCollapse]);

    useEffect(()=>{
        if (!consultation?.date) {
            setSelectedDate(new Date());
        }
    },[])

    useEffect(() => {
    if (wasUpdated) {
      validateForm();
    }
  }, [selectedHealthPlan, wasUpdated]);

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
                                    label="Consultation Date"
                                    placeholder="Select Date"
                                    onChangeDate={handleOnChangeDate}
                                    value={selectedDate}
                                    onClear={handleOnClearDate}
                                />
                            </div>
                            <div className="col-mobile-7">
                                <div className="d-none d-mobile-block">
                                    <div className="form-group">
                                        <label className="form-label fw-bold text-primary">Additional Health Plan</label>
                                        <Select
                                            options={healthPlanOptions}
                                            value={healthPlanOptions.find(option => option.value === selectedHealthPlan) || null}
                                            onChange={onSelectHealthPlan}
                                            placeholder="No Health Plan"
                                            isSearchable={true}
                                            isClearable={true}
                                            styles={customSelectStyles}
                                            noOptionsMessage={() => "No health plans found"}
                                            loadingMessage={() => "Loading health plans..."}
                                            isLoading={isLoadingHealthPlans}
                                        />
                                    </div>
                                </div>
                                <div className="d-mobile-none mt-3">
                                    <div className="form-group">
                                        <label className="form-label fw-bold text-primary">Additional Health Plan</label>
                                        <Select
                                            options={healthPlanOptions}
                                            value={healthPlanOptions.find(option => option.value === selectedHealthPlan) || null}
                                            onChange={onSelectHealthPlan}
                                            placeholder="No Health Plan"
                                            isSearchable={true}
                                            isClearable={true}
                                            styles={customSelectStyles}
                                            noOptionsMessage={() => "No health plans found"}
                                            loadingMessage={() => "Loading health plans..."}
                                            isLoading={isLoadingHealthPlans}
                                        />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div className="row">
                            <div className="col-12 mt-3">
                                <FlexInputArea
                                    label="Reason to request a consultation"
                                    placeholder="Reason"
                                    rows="5"
                                    onChange={handleOnChangeReason}
                                    value={reasonValue}
                                    onBlur={handleSelectedReason}
                                    maxLength={consultationMaximumAllowedValue}
                                />
                                <div id="create-additional-info-disclaimer" className="text-muted mt-2">
                                    In case of rule outs (R/O) include signs & symptoms and do not code the R/O condition; you may describe it in a narrative way example: seeing flashes, hearing hissing noises, vomiting R/O epilepsy.

                                </div>
                                {reasonError && (
                                    <div id="create-additional-info-reason-max-msg" className="text-danger text-sm mt-1">
                                        {reasonError}
                                    </div>
                                )}
                                {dateError && (
                                    <div id="create-additional-info-date-error" className="text-danger text-sm mt-1">
                                        {dateError}
                                    </div>
                                )}
                            </div>
                        </div>
                    </div>
                </div>
                {!isValidForm && wasUpdated ? (
                    <div className="d-none d-mobile-block text-danger text-sm pl-2 pt-2 ml-3" id="clinical-consultation-requesting-provider-alert-message">
                        {stepAlertMessage}
                    </div>
                ) : null}
                {
                    reasonError && (
                        <div id="create-additional-info-reason-error" className="d-none d-mobile-block text-danger text-sm pl-2 pt-2">
                            {reasonError}
                        </div>
                )}
                {
                    healthPlanValidationError && (
                        <div id="create-additional-info-health-plan-error" className="d-none d-mobile-block text-danger text-sm pl-2 pt-2">
                            {healthPlanValidationError}
                        </div>
                )}
                <div className="d-none d-mobile-block mt-3">
                    <div className="d-flex aling-items-center">
                        <button
                            id = "create-additional-info-next-btn"
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
                onClose={handleOnCancel}
                onCancel={handleOnCancel}
                onNext={handleClickNext}
                isValid={!isValidForm && wasUpdated}
                alertMessage={stepAlertMessage}
            />
        </div>
    );
}

export default App;

