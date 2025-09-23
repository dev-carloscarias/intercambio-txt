import {
    RequestingProvider,
    ConsultationProvider,
    DiagnosesInterface
} from './CreateRequestingServicingProvider';
import { ProcedureInterface } from './ProcedureInterface';

export interface Consultation {
    requestingProvider?: {
        billing: RequestingProvider;
        rendering: RequestingProvider;
    };
    consultationProvider?: ConsultationProvider;
    diagnoses: DiagnosesInterface;
    procedure: ProcedureInterface;
    date: Date;
    healthPlan: string;
    reason: string;
}
