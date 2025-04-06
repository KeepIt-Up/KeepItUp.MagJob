import {
  Component,
  ChangeDetectionStrategy,
  OnInit,
  CUSTOM_ELEMENTS_SCHEMA
} from '@angular/core';
import { 
  CalendarEvent, 
  CalendarView, 
  CalendarEventAction, 
  CalendarUtils, 
  CalendarModule,
  CalendarA11y,
  CalendarEventTitleFormatter
} from 'angular-calendar';
import { Subject } from 'rxjs';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import moment from 'moment';

interface CalendarEventExtended extends CalendarEvent {
  description?: string;
  end: Date;
}

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [
    CommonModule, 
    FormsModule, 
    CalendarModule,
    DatePipe
  ],
  providers: [
    CalendarUtils, 
    CalendarA11y,
    CalendarEventTitleFormatter
  ],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
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
  editedEvent: CalendarEventExtended | null = null;

  actions: CalendarEventAction[] = [
    {
      label: '<i class="bi bi-pencil-fill"></i>',
      onClick: ({ event, sourceEvent }: { event: CalendarEvent; sourceEvent: MouseEvent | KeyboardEvent }): void => {
        if (event.end) {
          this.editEvent(event as CalendarEventExtended);
        }
      }
    },
    {
      label: '<i class="bi bi-trash-fill"></i>',
      onClick: ({ event, sourceEvent }: { event: CalendarEvent; sourceEvent: MouseEvent | KeyboardEvent }): void => {
        if (event.end) {
          this.deleteEvent(event as CalendarEventExtended);
        }
      }
    }
  ];

  newEvent: CalendarEventExtended = {
    title: '',
    start: new Date(),
    end: new Date(new Date().getTime() + 60 * 60 * 1000), // 1 hour later
    description: '',
    draggable: true,
    actions: this.actions
  };

  events: CalendarEventExtended[] = [
    {
      start: moment().startOf('day').toDate(),
      end: moment().startOf('day').add(1, 'hour').toDate(),
      title: 'An example event',
      description: 'This is a sample event description',
      actions: this.actions,
      draggable: true,
      resizable: {
        beforeStart: true,
        afterEnd: true
      }
    }
  ];

  ngOnInit(): void {
    this.viewDate = new Date();
  }

  handleEvent(action: string, event: CalendarEvent): void {
    if (event.end) {
      const extendedEvent = event as CalendarEventExtended;
      console.log('Event action:', action, 'Event:', extendedEvent);
    }
  }

  addEvent(): void {
    const now = new Date();
    this.newEvent = {
      title: '',
      start: now,
      end: new Date(now.getTime() + 60 * 60 * 1000), // 1 hour later
      description: '',
      draggable: true,
      actions: this.actions
    };
    this.showAddDialog = true;
  }

  confirmAdd(): void {
    if (this.newEvent.title) {
      const newEvent: CalendarEventExtended = {
        ...this.newEvent,
        start: new Date(this.newEvent.start),
        end: new Date(this.newEvent.end),
        resizable: {
          beforeStart: true,
          afterEnd: true
        }
      };
      this.events = [...this.events, newEvent];
      this.handleEvent('Added', newEvent);
      this.refresh.next();
      this.cancelAdd();
    }
  }

  cancelAdd(): void {
    this.showAddDialog = false;
    this.newEvent = {
      title: '',
      start: new Date(),
      end: new Date(new Date().getTime() + 60 * 60 * 1000), // 1 hour later
      description: '',
      draggable: true,
      actions: this.actions
    };
  }

  editEvent(event: CalendarEventExtended): void {
    this.eventToEdit = event;
    this.editedEvent = { 
      ...event,
      start: new Date(event.start),
      end: new Date(event.end)
    };
    this.showEditDialog = true;
  }

  confirmEdit(): void {
    if (this.editedEvent && this.eventToEdit) {
      const index = this.events.indexOf(this.eventToEdit);
      if (index !== -1) {
        const updatedEvent = {
          ...this.editedEvent,
          start: new Date(this.editedEvent.start),
          end: new Date(this.editedEvent.end)
        };
        this.events[index] = updatedEvent;
        this.handleEvent('Edited', updatedEvent);
        this.refresh.next();
        this.cancelEdit();
      }
    }
  }

  cancelEdit(): void {
    this.showEditDialog = false;
    this.eventToEdit = null;
    this.editedEvent = null;
  }

  confirmDelete(): void {
    if (this.eventToDelete) {
      this.events = this.events.filter(event => event !== this.eventToDelete);
      this.handleEvent('Deleted', this.eventToDelete);
      this.cancelDelete();
    }
  }

  cancelDelete(): void {
    this.showDeleteConfirm = false;
    this.eventToDelete = null;
  }

  deleteEvent(eventToDelete: CalendarEventExtended): void {
    this.eventToDelete = eventToDelete;
    this.showDeleteConfirm = true;
  }

  setView(view: CalendarView): void {
    this.view = view;
  }
} 