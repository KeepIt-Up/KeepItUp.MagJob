import { TimeEntryTemplate } from './time-entry-template.model';

export interface AvailabilityTemplateResponse {
  id: string;
  name: string;
  organizationId: string;
  startDayOfWeek: string;
  numberOfDays: number;
  timeEntryTemplates: (TimeEntryTemplate & { id: string })[];
}
