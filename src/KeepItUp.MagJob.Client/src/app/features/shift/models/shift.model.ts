import { ShiftEditRequest } from './shiftEditRequest.model';

export interface Shift {
  id: string;
  startTime: string;
  endTime: string;
  description?: string;
  memberId: string;
  status: boolean;
  shiftEditRequests: ShiftEditRequest[];
}
