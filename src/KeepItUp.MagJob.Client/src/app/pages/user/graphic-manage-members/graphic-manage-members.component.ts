import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { GraphicsService } from '../../../features/calendar/services/graphics.service';
import { UserService } from '../../../features/users/services/user.service';
import { OrganizationApiService } from '../../../features/organizations/services/organization.api.service';
import {
  GraphicResponse,
  TimeEntryResponse,
  TimeEntryMemberResponse,
  CreateTimeEntryMembersBulkRequest,
} from '../../../features/calendar/models/graphic.model';
import { Organization } from '../../../features/organizations/models/organization.model';
import { Member } from '../../../features/members/models/member';
import { ButtonComponent } from '../../../shared/components/button/button.component';
import { TagComponent } from '../../../shared/components/tag/tag.component';
import { AlertService } from '../../../shared/services/alert.service';
import { AlertContainerComponent } from '../../../shared/components/alert-container/alert-container.component';

@Component({
  selector: 'app-graphic-manage-members',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    FormsModule,
    ButtonComponent,
    TagComponent,
    AlertContainerComponent,
  ],
  templateUrl: './graphic-manage-members.component.html',
  styleUrls: ['./graphic-manage-members.component.scss'],
})
export class GraphicManageMembersComponent implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly graphicsService = inject(GraphicsService);
  private readonly userService = inject(UserService);
  private readonly organizationApiService = inject(OrganizationApiService);
  private readonly alertService = inject(AlertService);

  graphic: GraphicResponse | null = null;
  isLoading = true;
  error: string | null = null;
  selectedTimeEntry: TimeEntryResponse | null = null;
  isAddingMember = false;
  isRemovingMember = false;

  showOrganizationModal = true;
  organizations: Organization[] = [];
  selectedOrganization: Organization | null = null;
  isLoadingOrganizations = false;

  selectedMembers: Record<string, { memberId: string; status: string }[]> = {};
  availableMembers: Member[] = [];

  ngOnInit(): void {
    setTimeout(() => {
      this.loadUserOrganizations();
    }, 100);
  }

  private loadUserOrganizations(): void {
    this.isLoadingOrganizations = true;
    console.log('Loading user organizations...');
    this.userService.getUserOrganizations().subscribe({
      next: response => {
        console.log('Organizations response:', response);
        this.organizations = response.items || [];
        this.isLoadingOrganizations = false;
        if (this.organizations.length === 0) {
          this.error = 'No organizations found';
        }
      },
      error: (error: unknown) => {
        console.error('Error loading organizations:', error);
        this.error = (error as Error)?.message || 'Failed to load organizations';
        this.isLoadingOrganizations = false;
      },
    });
  }

  selectOrganization(organization: Organization): void {
    console.log('Selected organization:', organization);
    this.selectedOrganization = organization;
    this.showOrganizationModal = false;

    this.loadOrganizationMembers(organization.id);

    const graphicId = this.route.snapshot.params['id'] as string;
    if (graphicId) {
      this.loadGraphic(graphicId);
    }
  }

  private loadOrganizationMembers(organizationId: string): void {
    this.organizationApiService
      .getMembers(
        organizationId,
        {},
        { pageNumber: 1, pageSize: 100, sortField: 'id', ascending: true },
      )
      .subscribe({
        next: response => {
          this.availableMembers = response.items || [];
        },
        error: (error: unknown) => {
          console.error('Error loading organization members:', error);
          this.availableMembers = [];
        },
      });
  }

  private loadGraphic(id: string): void {
    this.isLoading = true;
    this.error = null;

    this.graphicsService.getGraphic(id).subscribe({
      next: graphic => {
        this.graphic = graphic;
        this.isLoading = false;

        if (graphic.timeEntries) {
          graphic.timeEntries.forEach(entry => {
            if (!this.selectedMembers[entry.id]) {
              this.selectedMembers[entry.id] = [];
            }
          });
        }
      },
      error: (error: unknown) => {
        this.error = (error as Error)?.message || 'Failed to load graphic details';
        this.isLoading = false;
      },
    });
  }

  selectTimeEntry(timeEntry: TimeEntryResponse): void {
    this.selectedTimeEntry = timeEntry;
  }

  getTimeEntryMembers(timeEntryId: string, graphic: GraphicResponse): TimeEntryMemberResponse[] {
    return graphic.timeEntryMembers?.filter(member => member.timeEntryId === timeEntryId) || [];
  }

  addMemberToTimeEntry(timeEntryId: string, memberId: string, status: string): void {
    if (!this.selectedMembers[timeEntryId]) {
      this.selectedMembers[timeEntryId] = [];
    }

    const existingIndex = this.selectedMembers[timeEntryId].findIndex(m => m.memberId === memberId);
    if (existingIndex >= 0) {
      this.selectedMembers[timeEntryId][existingIndex].status = status;
    } else {
      this.selectedMembers[timeEntryId].push({ memberId, status });
    }
  }

  removeMemberFromTimeEntry(timeEntryId: string, memberId: string): void {
    if (this.selectedMembers[timeEntryId]) {
      this.selectedMembers[timeEntryId] = this.selectedMembers[timeEntryId].filter(
        m => m.memberId !== memberId,
      );
    }
  }

  isMemberSelected(timeEntryId: string, memberId: string): boolean {
    return this.selectedMembers[timeEntryId]?.some(m => m.memberId === memberId) ?? false;
  }

  getMemberStatus(timeEntryId: string, memberId: string): string {
    const member = this.selectedMembers[timeEntryId]?.find(m => m.memberId === memberId);
    return member?.status ?? 'Not confirmed';
  }

  saveTimeEntryMembers(timeEntryId: string): void {
    if (!this.selectedMembers[timeEntryId] || this.selectedMembers[timeEntryId].length === 0) {
      return;
    }

    this.isAddingMember = true;

    const request: CreateTimeEntryMembersBulkRequest = {
      timeEntryId: timeEntryId,
      memberAssignments: this.selectedMembers[timeEntryId].map(member => ({
        memberId: member.memberId,
        status: member.status,
      })),
    };

    console.log('Sending bulk request:', request);
    this.graphicsService.createTimeEntryMembersBulk(request).subscribe({
      next: () => {
        this.isAddingMember = false;
        this.alertService.success(
          'Members Saved',
          `Successfully assigned ${this.selectedMembers[timeEntryId].length} member(s) to this time entry.`,
        );
        this.refreshGraphic();
      },
      error: (error: unknown) => {
        this.isAddingMember = false;
        this.alertService.error('Save Failed', 'Failed to save members. Please try again.');
        console.error('Failed to save members:', error);
        console.error('Request that failed:', request);
      },
    });
  }

  removeMember(member: TimeEntryMemberResponse): void {
    if (this.isRemovingMember) return;

    this.isRemovingMember = true;

    this.graphicsService.removeMemberFromTimeEntry(member.timeEntryId, member.id).subscribe({
      next: () => {
        this.isRemovingMember = false;
        this.alertService.success(
          'Member Removed',
          'Successfully removed member from this time entry.',
        );
        this.refreshGraphic();
      },
      error: (error: unknown) => {
        this.isRemovingMember = false;
        this.alertService.error('Remove Failed', 'Failed to remove member. Please try again.');
        console.error('Failed to remove member:', error);
      },
    });
  }

  getMemberName(memberId: string): string {
    const member = this.availableMembers.find(m => m.userId === memberId);
    return member ? `${member.firstName} ${member.lastName}` : 'Unknown Member';
  }

  getSelectedMembersCount(timeEntryId: string): number {
    return this.selectedMembers[timeEntryId]?.length ?? 0;
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
    return graphic.timeEntryMembers?.length ?? 0;
  }

  getUniqueMembersCount(graphic: GraphicResponse): number {
    const uniqueMemberIds = new Set(graphic.timeEntryMembers?.map(member => member.memberId) ?? []);
    return uniqueMemberIds.size;
  }
}
