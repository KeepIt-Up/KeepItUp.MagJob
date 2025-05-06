import { Component, EventEmitter, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CalendarEventExtended } from '../../models/calendar-event.model';

@Component({
  selector: 'app-add-event-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './add-event-dialog.component.html',
  styleUrls: ['./add-event-dialog.component.scss']
})
export class AddEventDialogComponent {
  event: CalendarEventExtended = {
    title: '',
    start: new Date(),
    end: new Date(new Date().getTime() + 60 * 60 * 1000), // 1 hour later
    description: '',
    draggable: true,
    resizable: {
      beforeStart: true,
      afterEnd: true,
    },
  };

  @Output() add = new EventEmitter<CalendarEventExtended>();
  @Output() cancel = new EventEmitter<void>();

  onAdd(): void {
    this.add.emit(this.event);
  }

  onCancel(): void {
    this.cancel.emit();
  }
} 