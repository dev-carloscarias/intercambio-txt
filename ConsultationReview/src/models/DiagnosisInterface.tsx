export interface DiagnosisInterface {
    code: string;
    description: string;
    isPrimary?: boolean;
}

export interface DiagnosesInterface extends Array<DiagnosisInterface> {}
