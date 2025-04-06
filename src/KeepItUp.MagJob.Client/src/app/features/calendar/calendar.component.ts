import {
  Component,
  ChangeDetectionStrategy,
  OnInit,
} from '@angular/core';
import { CalendarEvent, CalendarView, CalendarEventAction } from 'angular-calendar';
import { Subject } from 'rxjs';
import { CalendarModule } from 'angular-calendar';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import moment from 'moment';

interface CalendarEventExtended extends CalendarEvent {
  description?: string;
}

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule, FormsModule, CalendarModule, DatePipe],
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.scss']
})
export class CalendarComponent implements OnInit {
  view: CalendarView = CalendarView.Month;
  CalendarView = CalendarView;
  viewDate: Date = new Date();
  refresh = new Subject<void>();

  actions: CalendarEventAction[] = [
    {
      label: '<i class="bi bi-pencil-fill"></i>',
      onClick: ({ event }: { event: CalendarEventExtended }): void => {
        this.handleEvent('Edited', event);
      }
    },
    {
      label: '<i class="bi bi-trash-fill"></i>',
      onClick: ({ event }: { event: CalendarEventExtended }): void => {
        this.events = this.events.filter(iEvent => iEvent !== event);
        this.handleEvent('Deleted', event);
      }
    }
  ];

  events: CalendarEventExtended[] = [
    {
      start: moment().startOf('day').toDate(),
      title: 'An example event',
      description: 'This is a sample event description',
      actions: this.actions,
      draggable: true,
      resizable: {
        beforeStart: true,
        afterEnd: true,
      },
    }
  ];

  ngOnInit(): void {
    this.viewDate = new Date();
  }

  handleEvent(action: string, event: CalendarEventExtended): void {
    console.log('Event action:', action, 'Event:', event);
  }

  addEvent(): void {
    this.events = [
      ...this.events,
      {
        title: 'New event',
        start: moment().startOf('day').toDate(),
        description: 'Add description here',
        draggable: true,
        actions: this.actions,
      },
    ];
  }

  deleteEvent(eventToDelete: CalendarEventExtended): void {
    this.events = this.events.filter((event) => event !== eventToDelete);
  }

  setView(view: CalendarView): void {
    this.view = view;
  }
} 