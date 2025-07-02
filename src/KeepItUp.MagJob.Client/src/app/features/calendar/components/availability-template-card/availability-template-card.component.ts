import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AvailabilityTemplateResponse } from '../../models/availability-template-response.model';
import { TagComponent } from '@shared/components/tag/tag.component';

@Component({
  selector: 'app-availability-template-card',
  standalone: true,
  imports: [CommonModule, TagComponent],
  templateUrl: './availability-template-card.component.html',
  styleUrls: ['./availability-template-card.component.scss'],
})
export class AvailabilityTemplateCardComponent {
  @Input({ required: true }) template!: AvailabilityTemplateResponse;
  getDayOfWeekName(dayOfWeek: string): string {
    const days = {
      MONDAY: 'Monday',
      TUESDAY: 'Tuesday',
      WEDNESDAY: 'Wednesday',
      THURSDAY: 'Thursday',
      FRIDAY: 'Friday',
      SATURDAY: 'Saturday',
      SUNDAY: 'Sunday',
    };
    return days[dayOfWeek as keyof typeof days] || dayOfWeek;
  }

  getTotalHours(): number {
    return this.template.timeEntryTemplates.reduce((total, entry) => {
      const start = new Date(`1970-01-01T${entry.startTime}`);
      const end = new Date(`1970-01-01T${entry.endTime}`);
      const hours = (end.getTime() - start.getTime()) / (1000 * 60 * 60);
      return total + hours;
    }, 0);
  }
}
