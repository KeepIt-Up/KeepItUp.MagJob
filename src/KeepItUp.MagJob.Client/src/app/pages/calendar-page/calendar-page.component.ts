import { Component } from '@angular/core';
import { CalendarComponent } from '../../features/calendar/calendar.component';
import { CommonModule } from '@angular/common';
import {
  CalendarUtils,
  CalendarModule,
  CalendarA11y,
  CalendarEventTitleFormatter,
} from 'angular-calendar';

@Component({
  selector: 'app-calendar-page',
  standalone: true,
  imports: [CommonModule, CalendarComponent, CalendarModule],
  providers: [CalendarUtils, CalendarA11y, CalendarEventTitleFormatter],
  templateUrl: './calendar-page.component.html',
  styleUrls: ['./calendar-page.component.scss'],
})
export class CalendarPageComponent {}
