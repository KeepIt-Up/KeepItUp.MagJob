import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CalendarEventExtended } from '../../models/calendar-event.model';

@Component({
  selector: 'app-edit-event-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './edit-event-dialog.component.html',
  styleUrls: ['./edit-event-dialog.component.scss']
})
export class EditEventDialogComponent {
  @Input() event!: CalendarEventExtended;
  @Output() save = new EventEmitter<CalendarEventExtended>();
  @Output() cancel = new EventEmitter<void>();

  onSave(): void {
    this.save.emit(this.event);
  }

  onCancel(): void {
    this.cancel.emit();
  }
} 