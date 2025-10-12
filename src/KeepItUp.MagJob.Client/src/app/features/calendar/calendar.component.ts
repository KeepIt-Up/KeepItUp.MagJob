import {
  Component,
  ChangeDetectionStrategy,
  OnInit,
  CUSTOM_ELEMENTS_SCHEMA,
  inject,
} from '@angular/core';
import { trigger, state, style, transition, animate } from '@angular/animations';
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
import { AvailabilityTemplateService } from './services/availability-template.service';
import { AvailabilityTemplate } from './models/availability-template.model';
import { AvailabilityTemplateDialogComponent } from './components/availability-template-dialog/availability-template-dialog.component';
import { CalendarToTemplateFunction } from './function/calendar-to-template-function';

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
  view: CalendarView = CalendarView.Month;
  CalendarView = CalendarView;
  viewDate: Date = new Date();
  refresh = new Subject<void>();
  showDeleteConfirm = false;
  showEditDialog = false;
  showAddDialog = false;
  eventToDelete: CalendarEventExtended | null = null;
  eventToEdit: CalendarEventExtended | null = null;
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
        sourceEvent,
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
        sourceEvent,
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

  // Add new properties for availability template functionality
  showAvailabilityTemplateDialog = false;
  showSuccessAlert = false;
  successMessage = '';
  private AvailabilityTemplateService = inject(AvailabilityTemplateService);
  private calendarToTemplateFunction = inject(CalendarToTemplateFunction);

  ngOnInit(): void {
    this.viewDate = new Date();
    moment.updateLocale('en', {
      week: {
        dow: 1,
      },
    });
  }

  handleEvent(action: string, event: CalendarEvent): void {
    if (event.end) {
      const extendedEvent = event as CalendarEventExtended;
      console.log('Event action:', action, 'Event:', extendedEvent);
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
}
