import { DiagnosisInterface } from './DiagnosisInterface';
import { ProcedureInterface } from './ProcedureInterface';
import { ProviderInterface } from './ProviderInterface';

export interface ProceduresInterface extends Array<ProcedureInterface> {}

export interface DiagnosesInterface extends Array<DiagnosisInterface> {}

export interface ConsultationProvider extends ProviderInterface {}

export interface RequestingProvider extends ProviderInterface {}
