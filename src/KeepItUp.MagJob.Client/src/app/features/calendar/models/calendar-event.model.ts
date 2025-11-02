import { CalendarEvent } from 'angular-calendar';

export interface CalendarEventExtended extends CalendarEvent {
  description?: string;
  end: Date;
  meta?: {
    status?: string;
    memberId?: string;
    timeEntryId?: string;
  };
}
