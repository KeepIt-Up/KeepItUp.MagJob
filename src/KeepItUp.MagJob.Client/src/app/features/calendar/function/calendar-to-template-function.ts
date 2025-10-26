import { Injectable } from '@angular/core';
import { CalendarEventExtended } from '../models/calendar-event.model';
import { TimeEntryTemplate } from '../models/time-entry-template.model';
import { AvailabilityTemplate } from '../models/availability-template.model';

@Injectable({
  providedIn: 'root',
})
export class CalendarToTemplateFunction {
  convertEventsToTemplate(
    events: CalendarEventExtended[],
    templateName: string,
    startDayOfWeek = 'MONDAY',
    numberOfDays = 7,
  ): AvailabilityTemplate {
    const timeEntryTemplates = events.map(event => this.eventToTimeEntry(event, startDayOfWeek));

    return {
      name: templateName,
      startDayOfWeek,
      numberOfDays,
      timeEntryTemplates,
    };
  }

  private eventToTimeEntry(
    event: CalendarEventExtended,
    startDayOfWeek: string,
  ): TimeEntryTemplate {
    const referenceDate = this.getWeekStartDate(startDayOfWeek);

    const startDayOffset = this.calculateDayOffset(event.start, referenceDate);
    const endDayOffset = this.calculateDayOffset(event.end, referenceDate);

    return {
      startTime: this.formatTimeString(event.start),
      endTime: this.formatTimeString(event.end),
      startDayOffset,
      endDayOffset,
    };
  }

  public calculateDayOffset(date: Date, referenceDate: Date): number {
    const millisecondsPerDay = 24 * 60 * 60 * 1000;
    const diffTime = date.getTime() - referenceDate.getTime();
    return Math.floor(diffTime / millisecondsPerDay);
  }

  public getWeekStartDate(startDayOfWeek: string): Date {
    const today = new Date();
    const currentDay = today.getDay();
    const targetDay = this.dayOfWeekToNumber(startDayOfWeek);

    const daysToSubtract = (currentDay - targetDay + 7) % 7;

    const startDate = new Date(today);
    startDate.setDate(today.getDate() - daysToSubtract);
    startDate.setHours(0, 0, 0, 0); // Reset to midnight

    return startDate;
  }

  private dayOfWeekToNumber(day: string): number {
    const days = {
      SUNDAY: 0,
      MONDAY: 1,
      TUESDAY: 2,
      WEDNESDAY: 3,
      THURSDAY: 4,
      FRIDAY: 5,
      SATURDAY: 6,
    };
    if (Object.prototype.hasOwnProperty.call(days, day)) {
      return days[day as keyof typeof days];
    }
    return 1;
  }

  private formatTimeString(date: Date): string {
    return date.toTimeString().split(' ')[0];
  }
}
