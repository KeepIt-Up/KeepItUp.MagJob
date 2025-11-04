import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TagComponent } from '../../../../shared/components/tag/tag.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { CreateGraphicDialogComponent } from '../create-graphic-dialog/create-graphic-dialog.component';
import { AvailabilityTemplateResponse } from '../../models/availability-template-response.model';

@Component({
  selector: 'app-availability-template-card',
  standalone: true,
  imports: [CommonModule, TagComponent, ButtonComponent, CreateGraphicDialogComponent],
  templateUrl: './availability-template-card.component.html',
  styleUrls: ['./availability-template-card.component.scss'],
})
export class AvailabilityTemplateCardComponent {
  @Input() template!: AvailabilityTemplateResponse;
  isDialogOpen = false;

  getDayOfWeekName(dayOfWeek: string): string {
    const dayMap: Record<string, string> = {
      MONDAY: 'Monday',
      TUESDAY: 'Tuesday',
      WEDNESDAY: 'Wednesday',
      THURSDAY: 'Thursday',
      FRIDAY: 'Friday',
      SATURDAY: 'Saturday',
      SUNDAY: 'Sunday',
    };
    return dayMap[dayOfWeek] || dayOfWeek;
  }

  getTotalHours(): number {
    return this.template.timeEntryTemplates.reduce((total, entry) => {
      const start = this.parseTime(entry.startTime);
      const end = this.parseTime(entry.endTime);
      return total + (end - start) / (1000 * 60 * 60);
    }, 0);
  }

  private parseTime(timeString: string): number {
    const [hours, minutes, seconds] = timeString.split(':').map(Number);
    return new Date(0, 0, 0, hours, minutes, seconds).getTime();
  }

  openCreateGraphicDialog(): void {
    this.isDialogOpen = true;
  }

  closeCreateGraphicDialog(): void {
    this.isDialogOpen = false;
  }

  onGraphicCreated(): void {
    // Handle successful graphic creation
    console.log('Graphic created successfully!');
    // You could emit an event to the parent component or show a success message
  }
}
