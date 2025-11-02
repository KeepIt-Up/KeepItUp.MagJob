import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { CalendarEventExtended } from '../../models/calendar-event.model';

@Component({
  selector: 'app-employee-event-dialog',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './employee-event-dialog.component.html',
  styleUrls: ['./employee-event-dialog.component.scss']
})
export class EmployeeEventDialogComponent {
  @Input() event!: CalendarEventExtended;
  @Output() statusChange = new EventEmitter<{ event: CalendarEventExtended; status: string }>();
  @Output() cancel = new EventEmitter<void>();

  selectedStatus: string = '';

  ngOnInit(): void {
    this.selectedStatus = this.event.meta?.status || 'Pending';
  }

  onStatusChange(): void {
    // Only emit if status has actually changed
    if (this.hasStatusChanged()) {
      this.statusChange.emit({ event: this.event, status: this.selectedStatus });
    }
  }

  hasStatusChanged(): boolean {
    return this.selectedStatus !== (this.event.meta?.status || 'Pending');
  }

  onCancel(): void {
    this.cancel.emit();
  }

  getStatusOptions(): string[] {
    return ['Pending', 'Confirmed', 'Rejected'];
  }

  getStatusLabel(status: string): string {
    switch (status) {
      case 'Pending':
        return 'Pending';
      case 'Confirmed':
        return 'Confirmed';
      case 'Rejected':
        return 'Rejected';
      default:
        return status;
    }
  }
}
