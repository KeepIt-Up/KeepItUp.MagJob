import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { AvailabilityTemplate } from '../../models/availability-template.model';
import { ButtonComponent } from '../../../../shared/components/button/button.component';
import { InputComponent } from '../../../../shared/components/input/input.component';
import { CalendarEventExtended } from '../../models/calendar-event.model';
import { CalendarToTemplateFunction } from '../../function/calendar-to-template-function';

@Component({
  selector: 'app-availability-template-dialog',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, ButtonComponent, InputComponent],
  templateUrl: './availability-template-dialog.component.html',
  styleUrls: ['./availability-template-dialog.component.scss'],
})
export class AvailabilityTemplateDialogComponent implements OnInit {
  @Input() events: CalendarEventExtended[] = [];
  @Output() save = new EventEmitter<AvailabilityTemplate>();
  @Output() cancel = new EventEmitter<void>();

  templateForm: FormGroup;
  dayOfWeekOptions = ['MONDAY', 'TUESDAY', 'WEDNESDAY', 'THURSDAY', 'FRIDAY', 'SATURDAY', 'SUNDAY'];

  constructor(
    private fb: FormBuilder,
    private calendarToTemplate: CalendarToTemplateFunction,
  ) {
    this.templateForm = this.fb.group({
      name: ['Weekly Availability', Validators.required],
      organizationId: ['12345', Validators.required],
      startDayOfWeek: ['MONDAY', Validators.required],
      numberOfDays: [7, [Validators.required, Validators.min(1), Validators.max(14)]],
      timeEntryTemplates: this.fb.array([]),
    });
  }

  ngOnInit(): void {
    // Log all events when dialog opens
    console.log('Availability Template Dialog opened with events:', this.events);
    if (this.events && this.events.length > 0) {
      console.log(`Processing ${this.events.length} calendar events for template`);
      this.initFormWithEvents();
    } else {
      console.log('No events provided, creating default empty template');
      // Add default empty time entry if no events
      this.timeEntryTemplates.push(this.createTimeEntryForm());
    }
  }

  get timeEntryTemplates() {
    return this.templateForm.get('timeEntryTemplates') as FormArray;
  }

  createTimeEntryForm(event?: CalendarEventExtended) {
    if (event) {
      // Get reference date based on selected start day of week
      const startDayOfWeek = (this.templateForm.get('startDayOfWeek')?.value as string) || 'MONDAY';
      const referenceDate = this.calendarToTemplate.getWeekStartDate(startDayOfWeek);

      // Calculate day offsets for this event
      const startDayOffset = this.calendarToTemplate.calculateDayOffset(event.start, referenceDate);
      const endDayOffset = this.calendarToTemplate.calculateDayOffset(event.end, referenceDate);

      // Format times
      const startTime = this.formatTimeString(event.start);
      const endTime = this.formatTimeString(event.end);

      return this.fb.group({
        startTime: [startTime, Validators.required],
        endTime: [endTime, Validators.required],
        startDayOffset: [startDayOffset, Validators.required],
        endDayOffset: [endDayOffset, Validators.required],
      });
    }

    // Default empty form
    return this.fb.group({
      startTime: ['08:00:00', Validators.required],
      endTime: ['17:00:00', Validators.required],
      startDayOffset: [0, Validators.required],
      endDayOffset: [0, Validators.required],
    });
  }

  private initFormWithEvents(): void {
    // Clear existing entries
    while (this.timeEntryTemplates.length) {
      this.timeEntryTemplates.removeAt(0);
    }
    
    // Add form group for each event
    this.events.forEach((event, index) => {
      console.log(`Converting event ${index + 1}:`, {
        title: event.title,
        start: event.start,
        end: event.end
      });
      this.timeEntryTemplates.push(this.createTimeEntryForm(event));
    });
    
    console.log('Finished creating template form with', this.timeEntryTemplates.length, 'time entries');
  }

  // Helper to format time from Date object to HH:MM:SS string
  private formatTimeString(date: Date): string {
    return date.toTimeString().split(' ')[0];
  }

  addTimeEntry() {
    this.timeEntryTemplates.push(this.createTimeEntryForm());
  }

  removeTimeEntry(index: number) {
    if (this.timeEntryTemplates.length > 1) {
      this.timeEntryTemplates.removeAt(index);
    }
  }

  onSubmit() {
    if (this.templateForm.valid) {
      this.save.emit(this.templateForm.value as AvailabilityTemplate);
    }
  }

  onCancel() {
    this.cancel.emit();
  }
}
