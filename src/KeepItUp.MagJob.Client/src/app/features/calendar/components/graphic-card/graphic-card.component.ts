import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TagComponent } from '../../../../shared/components/tag/tag.component';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { GraphicResponse } from '../../models/graphic.model';

@Component({
  selector: 'app-graphic-card',
  standalone: true,
  imports: [CommonModule, RouterModule, TagComponent, ButtonComponent],
  templateUrl: './graphic-card.component.html',
  styleUrls: ['./graphic-card.component.scss'],
})
export class GraphicCardComponent {
  @Input() graphic!: GraphicResponse;

  getTotalTimeEntries(): number {
    return this.graphic.timeEntries?.length || 0;
  }

  getTotalMembers(): number {
    return this.graphic.timeEntryMembers?.length || 0;
  }

  getUniqueMembersCount(): number {
    const uniqueUserIds = new Set(
      this.graphic.timeEntryMembers?.map(member => member.memberId) || [],
    );
    return uniqueUserIds.size;
  }

  getTotalHours(): number {
    if (!this.graphic.timeEntries) return 0;

    return this.graphic.timeEntries.reduce((total, entry) => {
      const start = new Date(entry.startDateTime);
      const end = new Date(entry.endDateTime);
      const duration = (end.getTime() - start.getTime()) / (1000 * 60 * 60); // Convert to hours
      return total + duration;
    }, 0);
  }

  getDateRange(): string {
    if (!this.graphic.timeEntries || this.graphic.timeEntries.length === 0) {
      return 'No time entries';
    }

    const dates = this.graphic.timeEntries.map(entry => new Date(entry.startDateTime));
    const minDate = new Date(Math.min(...dates.map(d => d.getTime())));
    const maxDate = new Date(Math.max(...dates.map(d => d.getTime())));

    if (minDate.toDateString() === maxDate.toDateString()) {
      return minDate.toLocaleDateString();
    }

    return `${minDate.toLocaleDateString()} - ${maxDate.toLocaleDateString()}`;
  }
}
