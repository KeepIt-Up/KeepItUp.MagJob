import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GraphicsService } from '../../../features/calendar/services/graphics.service';
import {
  GraphicResponse,
  TimeEntryResponse,
  TimeEntryMemberResponse,
} from '../../../features/calendar/models/graphic.model';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { TagComponent } from '../../../shared/components/tag/tag.component';

@Component({
  selector: 'app-graphic-manage-members',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, ButtonComponent, TagComponent],
  templateUrl: './graphic-manage-members.component.html',
  styleUrls: ['./graphic-manage-members.component.scss'],
})
export class GraphicManageMembersComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly graphicsService = inject(GraphicsService);

  graphic: GraphicResponse | null = null;
  isLoading = true;
  error: string | null = null;
  selectedTimeEntry: TimeEntryResponse | null = null;
  newMemberUserId = '';
  isAddingMember = false;
  isRemovingMember = false;

  ngOnInit(): void {
    const graphicId = this.route.snapshot.params['id'];
    if (typeof graphicId === 'string' && graphicId) {
      this.loadGraphic(graphicId);
    } else {
      this.error = 'No graphic ID provided';
      this.isLoading = false;
    }
  }

  private loadGraphic(id: string): void {
    this.isLoading = true;
    this.error = null;

    this.graphicsService.getGraphic(id).subscribe({
      next: graphic => {
        this.graphic = graphic;
        this.isLoading = false;
      },
      error: error => {
        this.error = error.message || 'Failed to load graphic details';
        this.isLoading = false;
      },
    });
  }

  selectTimeEntry(timeEntry: TimeEntryResponse): void {
    this.selectedTimeEntry = timeEntry;
    this.newMemberUserId = '';
  }

  getTimeEntryMembers(timeEntryId: string, graphic: GraphicResponse): TimeEntryMemberResponse[] {
    return graphic.timeEntryMembers?.filter(member => member.timeEntryId === timeEntryId) || [];
  }

  addMember(): void {
    if (!this.selectedTimeEntry || !this.newMemberUserId.trim() || this.isAddingMember) {
      return;
    }

    this.isAddingMember = true;

    this.graphicsService
      .addMemberToTimeEntry(this.selectedTimeEntry.id, this.newMemberUserId.trim())
      .subscribe({
        next: () => {
          this.isAddingMember = false;
          this.newMemberUserId = '';
          this.refreshGraphic();
        },
        error: error => {
          this.isAddingMember = false;
          console.error('Failed to add member:', error);
        },
      });
  }

  removeMember(member: TimeEntryMemberResponse): void {
    if (this.isRemovingMember) return;

    this.isRemovingMember = true;

    this.graphicsService.removeMemberFromTimeEntry(member.timeEntryId, member.id).subscribe({
      next: () => {
        this.isRemovingMember = false;
        this.refreshGraphic();
      },
      error: error => {
        this.isRemovingMember = false;
        console.error('Failed to remove member:', error);
      },
    });
  }

  private refreshGraphic(): void {
    if (this.graphic) {
      this.loadGraphic(this.graphic.id);
    }
  }

  getTimeEntryDate(dateTime: string): string {
    return new Date(dateTime).toLocaleDateString();
  }

  getTimeEntryTime(dateTime: string): string {
    return new Date(dateTime).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' });
  }

  getTotalMembersCount(graphic: GraphicResponse): number {
    return graphic.timeEntryMembers?.length || 0;
  }

  getUniqueMembersCount(graphic: GraphicResponse): number {
    const uniqueUserIds = new Set(graphic.timeEntryMembers?.map(member => member.userId) || []);
    return uniqueUserIds.size;
  }
}
