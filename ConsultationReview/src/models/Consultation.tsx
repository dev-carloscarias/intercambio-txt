import { DiagnosesInterface } from './DiagnosisInterface';
import { ProcedureInterface } from './ProcedureInterface';
import { RequestingProvider, ConsultationProvider } from './ProviderInterface';

export interface Consultation {
    requestingProvider?: RequestingProvider;
    consultationProvider?: ConsultationProvider;
    diagnoses: DiagnosesInterface;
    procedure: ProcedureInterface;
    date: Date;
    healthPlan: HealthPlanInterface;
    reason: string;
    procedureQty?:number;
}
