import { TimeEntryTemplate } from './time-entry-template.model';

export interface AvailabilityTemplate {
  name: string;
  organizationId: string;
  startDayOfWeek: string;
  numberOfDays: number;
  timeEntryTemplates: TimeEntryTemplate[];
  userId: string;
}
