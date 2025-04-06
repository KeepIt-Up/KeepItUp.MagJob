import { Component } from '@angular/core';
import { CalendarComponent } from '../../features/calendar/calendar.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-calendar-page',
  standalone: true,
  imports: [CommonModule, CalendarComponent],
  templateUrl: './calendar-page.component.html',
  styleUrls: ['./calendar-page.component.scss'],
})
export class CalendarPageComponent {} 