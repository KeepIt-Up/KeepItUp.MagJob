import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CalendarComponent } from '../../features/calendar/calendar.component';
import { CommonModule } from '@angular/common';
import {
  CalendarUtils,
  CalendarModule,
  CalendarA11y,
  CalendarEventTitleFormatter,
} from 'angular-calendar';
import { CalendarViewMode } from '../../features/calendar/models/calendar-view-mode.model';

@Component({
  selector: 'app-calendar-page',
  standalone: true,
  imports: [CommonModule, CalendarComponent, CalendarModule],
  providers: [CalendarUtils, CalendarA11y, CalendarEventTitleFormatter],
  templateUrl: './calendar-page.component.html',
  styleUrls: ['./calendar-page.component.scss'],
})
export class CalendarPageComponent implements OnInit {
  viewMode: CalendarViewMode = 'manager';

  constructor(private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.viewMode = this.route.snapshot.data['viewMode'] || 'manager';
  }
}
