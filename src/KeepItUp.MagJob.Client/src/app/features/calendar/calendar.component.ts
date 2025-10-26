import {
  Component,
  ChangeDetectionStrategy,
  OnInit,
  CUSTOM_ELEMENTS_SCHEMA,
  inject,
  Input,
} from '@angular/core';

import {
  CalendarEvent,
  CalendarView,
  CalendarEventAction,
  CalendarUtils,
  CalendarModule,
  CalendarA11y,
  CalendarEventTitleFormatter,
  CalendarEventTimesChangedEvent,
} from 'angular-calendar';
import { Subject } from 'rxjs';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import moment from 'moment';
import { CalendarEventExtended } from './models/calendar-event.model';
import { AddEventDialogComponent } from './components/add-event-dialog/add-event-dialog.component';
import { EditEventDialogComponent } from './components/edit-event-dialog/edit-event-dialog.component';
import { DeleteConfirmationDialogComponent } from './components/delete-confirmation-dialog/delete-confirmation-dialog.component';
import { EmployeeEventDialogComponent } from './components/employee-event-dialog/employee-event-dialog.component';
import { AvailabilityTemplateService } from './services/availability-template.service';
import { AvailabilityTemplate } from './models/availability-template.model';
import { AvailabilityTemplateDialogComponent } from './components/availability-template-dialog/availability-template-dialog.component';
import { CalendarToTemplateFunction } from './function/calendar-to-template-function';
import { CalendarViewMode } from './models/calendar-view-mode.model';
import {
  TimeEntryMemberResponse,
  GetTimeEntryMembersResponse,
} from './models/time-entry-member.model';
import { GraphicApiService } from './services/graphic.api.service';
import { UserContextService } from '../../features/users/services/user-context.service';
import { PatchTimeEntryMemberRequest } from './models/graphic.model';

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    CalendarModule,
    DatePipe,
    AddEventDialogComponent,
    EditEventDialogComponent,
    DeleteConfirmationDialogComponent,
    EmployeeEventDialogComponent,
    AvailabilityTemplateDialogComponent,
  ],
  providers: [
    CalendarUtils,
    CalendarA11y,
    CalendarEventTitleFormatter,
    AvailabilityTemplateService,
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss'],
})
export class CalendarComponent implements OnInit {
  @Input() viewMode: CalendarViewMode = 'managerCreate';
  @Input() graphicId: string | null = null;
  view: CalendarView = CalendarView.Month;
  CalendarView = CalendarView;
  viewDate: Date = new Date();
  refresh = new Subject<void>();
  showDeleteConfirm = false;
  showEditDialog = false;
  showAddDialog = false;
  showEmployeeEventDialog = false;
  eventToDelete: CalendarEventExtended | null = null;
  eventToEdit: CalendarEventExtended | null = null;
  eventForEmployee: CalendarEventExtended | null = null;
  editedEvent: CalendarEventExtended = {
    title: '',
    start: new Date(),
    end: new Date(),
    description: '',
    draggable: true,
    resizable: {
      beforeStart: true,
      afterEnd: true,
    },
  };

  actions: CalendarEventAction[] = [
    {
      label: '<i class="bi bi-pencil-fill"></i>',
      onClick: ({
        event,
      }: {
        event: CalendarEvent;
        sourceEvent: MouseEvent | KeyboardEvent;
      }): void => {
        if (event.end) {
          this.editEvent(event as CalendarEventExtended);
        }
      },
    },
    {
      label: '<i class="bi bi-trash-fill"></i>',
      onClick: ({
        event,
      }: {
        event: CalendarEvent;
        sourceEvent: MouseEvent | KeyboardEvent;
      }): void => {
        if (event.end) {
          this.deleteEvent(event as CalendarEventExtended);
        }
      },
    },
  ];

  events: CalendarEventExtended[] = [
    {
      title: 'Morning Meeting',
      start: (() => {
        const date = new Date();
        date.setHours(6, 0, 0);
        return date;
      })(),
      end: (() => {
        const date = new Date();
        date.setHours(10, 30, 0);
        return date;
      })(),
      description: 'Team standup meeting',
      draggable: true,
      resizable: { beforeStart: true, afterEnd: true },
      actions: this.actions,
    },
    {
      title: 'Morning Meeting',
      start: new Date(new Date().setHours(9, 0, 0)),
      end: new Date(new Date().setHours(10, 30, 0)),
      description: 'Team standup meeting',
      draggable: true,
      resizable: {
        beforeStart: true,
        afterEnd: true,
      },
      actions: this.actions,
    },
    {
      title: 'Lunch Break',
      start: new Date(new Date().setHours(12, 0, 0)),
      end: new Date(new Date().setHours(13, 0, 0)),
      description: 'Lunch with team',
      draggable: true,
      resizable: {
        beforeStart: true,
        afterEnd: true,
      },
      actions: this.actions,
    },
    {
      title: 'Project Review',
      start: new Date(new Date().setHours(14, 0, 0)),
      end: new Date(new Date().setHours(16, 0, 0)),
      description: 'Project status review',
      draggable: true,
      resizable: {
        beforeStart: true,
        afterEnd: true,
      },
      actions: this.actions,
    },
    {
      title: 'Short Task',
      start: new Date(new Date().setHours(16, 30, 0)),
      end: new Date(new Date().setHours(17, 0, 0)),
      description: 'Quick task review',
      draggable: true,
      resizable: {
        beforeStart: true,
        afterEnd: true,
      },
      actions: this.actions,
    },
  ];

  showAvailabilityTemplateDialog = false;
  showSuccessAlert = false;
  successMessage = '';
  private AvailabilityTemplateService = inject(AvailabilityTemplateService);
  private calendarToTemplateFunction = inject(CalendarToTemplateFunction);
  private graphicApiService = inject(GraphicApiService);
  private userContextService = inject(UserContextService);

  ngOnInit(): void {
    this.viewDate = new Date();
    moment.updateLocale('en', {
      week: {
        dow: 1,
      },
    });

    if (this.viewMode === 'managerView') {
      this.loadGraphicTimeEntries();
    } else if (this.viewMode === 'employee') {
      this.loadEmployeeTimeEntries();
    }
  }

  handleEvent(action: string, event: CalendarEvent): void {
    if (event.end) {
      const extendedEvent = event as CalendarEventExtended;
      console.log('Event action:', action, 'Event:', extendedEvent);

      // Dla widoku grafiku (managerView) nie obsługuj kliknięć
      if (this.viewMode === 'managerView') {
        return;
      }

      if (this.viewMode === 'employee') {
        this.eventForEmployee = extendedEvent;
        this.showEmployeeEventDialog = true;
        return;
      }

      this.refresh.next();
    }
  }

  addEvent(): void {
    this.showAddDialog = true;
  }

  onAddEvent(event: CalendarEventExtended): void {
    const newEvent: CalendarEventExtended = {
      ...event,
      start: new Date(event.start),
      end: new Date(event.end),
      draggable: true,
      resizable: {
        beforeStart: true,
        afterEnd: true,
      },
      actions: this.actions,
    };
    this.events = [...this.events, newEvent];
    this.handleEvent('Added', newEvent);
    this.refresh.next();
    this.showAddDialog = false;
  }

  onCancelAdd(): void {
    this.showAddDialog = false;
  }

  editEvent(event: CalendarEventExtended): void {
    this.eventToEdit = event;
    this.editedEvent = {
      ...event,
      start: new Date(event.start),
      end: new Date(event.end),
      actions: this.actions,
      draggable: true,
      resizable: {
        beforeStart: true,
        afterEnd: true,
      },
    };
    this.showEditDialog = true;
  }

  onSaveEvent(event: CalendarEventExtended): void {
    if (this.eventToEdit) {
      const index = this.events.indexOf(this.eventToEdit);
      if (index !== -1) {
        const updatedEvent = {
          ...event,
          start: new Date(event.start),
          end: new Date(event.end),
          draggable: true,
          resizable: {
            beforeStart: true,
            afterEnd: true,
          },
          actions: this.actions,
        };
        this.events[index] = updatedEvent;
        this.handleEvent('Edited', updatedEvent);
        this.refresh.next();
        this.showEditDialog = false;
        this.eventToEdit = null;
      }
    }
  }

  onCancelEdit(): void {
    this.showEditDialog = false;
    this.eventToEdit = null;
  }

  deleteEvent(eventToDelete: CalendarEventExtended): void {
    this.eventToDelete = eventToDelete;
    this.showDeleteConfirm = true;
  }

  onConfirmDelete(): void {
    if (this.eventToDelete) {
      this.events = this.events.filter(event => event !== this.eventToDelete);
      this.handleEvent('Deleted', this.eventToDelete);
      this.showDeleteConfirm = false;
      this.eventToDelete = null;
    }
  }

  onCancelDelete(): void {
    this.showDeleteConfirm = false;
    this.eventToDelete = null;
  }

  setView(view: CalendarView): void {
    this.view = view;
  }

  eventTimesChanged({ event, newStart, newEnd }: CalendarEventTimesChangedEvent): void {
    event.start = newStart;
    event.end = newEnd;
    console.log('Event times changed:', event);
    this.refresh.next();
  }

  // Add new methods for availability template functionality
  openAvailabilityTemplateDialog(): void {
    console.log('Calendar events before opening dialog:', this.events);

    // If events are empty, this might be why
    if (!this.events || this.events.length === 0) {
      console.warn('No events available to create a template from!');
      // Maybe show a message to the user?
    }

    this.showAvailabilityTemplateDialog = true;
  }
  onSaveAvailabilityTemplate(template: AvailabilityTemplate): void {
    const events = this.events;

    // Assuming template.name and template.organizationId exist
    const templateWithEvents = this.calendarToTemplateFunction.convertEventsToTemplate(
      events,
      template.name,
      template.organizationId,
      template.startDayOfWeek || 'MONDAY',
      template.numberOfDays || 7,
      template.userId,
    );

    this.AvailabilityTemplateService.createAvailabilityTemplate(templateWithEvents).subscribe({
      next: response => {
        console.log('Template created:', response);
        this.showAvailabilityTemplateDialog = false;
        this.successMessage = `Availability template "${response.name}" was successfully saved!`;
        this.showSuccessAlert = true;
        setTimeout(() => (this.showSuccessAlert = false), 5000); // Hide alert after 5 seconds
      },
      error: error => {
        console.error('Error creating availability template:', error);
      },
    });
  }

  onCancelAvailabilityTemplate(): void {
    this.showAvailabilityTemplateDialog = false;
  }

  loadEmployeeTimeEntries(): void {
    const currentUser = this.userContextService.getCurrentUser();
    if (!currentUser) {
      console.error('User not authenticated');
      return;
    }

    this.graphicApiService.getTimeEntriesByUser(currentUser.id).subscribe({
      next: (response: GetTimeEntryMembersResponse) => {
        console.log(response);
        this.events = this.convertTimeEntryMembersToCalendarEvents(response.timeEntryMemberList);
        this.refresh.next();
      },
      error: error => {
        console.error('Error loading employee time entries:', error);
      },
    });
  }

  loadGraphicTimeEntries(): void {
    if (!this.graphicId) {
      console.error('Graphic ID not provided');
      return;
    }

    this.graphicApiService.getTimeEntriesByGraphic(this.graphicId).subscribe({
      next: (response: GetTimeEntryMembersResponse) => {
        console.log('Graphic time entries:', response);
        this.events = this.convertTimeEntryMembersToCalendarEvents(response.timeEntryMemberList);
        this.refresh.next();
      },
      error: error => {
        console.error('Error loading graphic time entries:', error);
      },
    });
  }

  convertTimeEntryMembersToCalendarEvents(
    timeEntryMembers: TimeEntryMemberResponse[],
  ): CalendarEventExtended[] {
    return timeEntryMembers.map(member => ({
      id: member.id,
      title:
        this.viewMode === 'managerView'
          ? `Member: ${member.memberId.substring(0, 8)}...`
          : 'Work Assignment',
      start: new Date(member.timeEntry.startDateTime),
      end: new Date(member.timeEntry.endDateTime),
      description: `${member.status}`,
      draggable: false,
      resizable: {
        beforeStart: false,
        afterEnd: false,
      },
      meta: {
        status: member.status,
        memberId: member.memberId,
        timeEntryId: member.timeEntry.id,
      },
      // Nie pokazuj akcji dla widoku grafiku (managerView)
      actions: this.viewMode === 'managerView' ? undefined : this.actions,
    }));
  }

  onEmployeeStatusChange(data: { event: CalendarEventExtended; status: string }): void {
    this.updateTimeEntryStatus(data.event, data.status);
    this.showEmployeeEventDialog = false;
    this.eventForEmployee = null;
  }

  onCancelEmployeeDialog(): void {
    this.showEmployeeEventDialog = false;
    this.eventForEmployee = null;
  }

  private updateTimeEntryStatus(event: CalendarEventExtended, newStatus: string): void {
    if (!event.id) return;

    const request: PatchTimeEntryMemberRequest = {
      status: newStatus,
    };

    this.graphicApiService.updateTimeEntryMemberStatus(String(event.id), request).subscribe({
      next: () => {
        this.loadEmployeeTimeEntries();
      },
      error: error => {
        console.error('Error updating time entry status:', error);
      },
    });
  }

  getEventStatusClass(event: CalendarEventExtended): string {
    // Pokazuj status dla employee lub dla managera w widoku managerView
    if (event.meta?.status && (this.viewMode === 'employee' || this.viewMode === 'managerView')) {
      switch (event.meta.status) {
        case 'Pending':
          return 'status-pending';
        case 'Confirmed':
          return 'status-confirmed';
        case 'Rejected':
          return 'status-rejected';
        default:
          return '';
      }
    }
    return '';
  }
}
