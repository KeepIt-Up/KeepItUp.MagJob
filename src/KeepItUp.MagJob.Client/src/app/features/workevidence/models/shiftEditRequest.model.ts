import { Shift } from './shift.model';

export interface ShiftEditRequest {
    id: string;
    status: string;
    startTime: string;
    endTime: string;
    shift: Shift;
    description: string;
}