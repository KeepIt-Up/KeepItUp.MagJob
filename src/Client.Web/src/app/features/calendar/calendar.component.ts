import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';

interface TimeSelection {
  datetime: Date;
}

// Add new interface for storing multiple selections
interface Selection {
  id: number;
  start: TimeSelection;
  end: TimeSelection;
}

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './calendar.component.html',
  styleUrl: './calendar.component.css'
})
export class CalendarComponent {
  dayNames: string[] = ['NIEDZ.', 'PON.', 'WT.', 'ŚR.', 'CZW.', 'PT.', 'SOB.'];
  weekDays: string[] = ['15', '16', '17', '18', '19', '20', '21'];
  hours: string[] = [
    '1:00', '2 :00', '3:00', '4:00', '5:00', '6:00',
    '7:00', '8:00', '9:00', '10:00', '11:00', '12:00',
    '13:00', '14:00', '15:00', '16:00', '17:00', '18:00',
    '19:00', '20:00', '21:00', '22:00', '23:00', '24:00',
  ];

  // Update selection-related variables
  isSelecting: boolean = false;
  selectionStart: TimeSelection | null = null;
  selectionEnd: TimeSelection | null = null;
  selections: Selection[] = []; // Array to store multiple selections

  startSelection(day: string, hour: string, quarter: number): void {
    const datetime = this.createDateTime(day, hour, quarter);
    this.isSelecting = true;
    this.selectionStart = { datetime };
    this.selectionEnd = { datetime };
  }

  updateSelection(day: string, hour: string, quarter: number): void {
    if (this.isSelecting) {
      this.selectionEnd = { datetime: this.createDateTime(day, hour, quarter) };
    }
  }

  endSelection(): void {
    this.isSelecting = false;
    if (this.selectionStart && this.selectionEnd) {
      this.selections.push({
        id: this.selections.length,
        start: this.selectionStart,
        end: this.selectionEnd
      });
      // Reset current selection
      this.selectionStart = null;
      this.selectionEnd = null;
    }
    console.log('All selections:', this.selections);
  }

  isSelected(day: string, hour: string, quarter: number): boolean {
    const currentDateTime = this.createDateTime(day, hour, quarter);
    const quarterEnd = new Date(currentDateTime.getTime() + 15 * 60000);

    // Check active selection
    if (this.selectionStart?.datetime && this.selectionEnd?.datetime) {
      const [startTime, endTime] = [
        this.selectionStart.datetime,
        this.selectionEnd.datetime
      ].sort((a, b) => a.getTime() - b.getTime());

      if (currentDateTime >= startTime && quarterEnd <= endTime) {
        return true;
      }
    }

    // Check saved selections
    return this.selections.some(selection => {
      const [startTime, endTime] = [
        selection.start.datetime,
        selection.end.datetime
      ].sort((a, b) => a.getTime() - b.getTime());

      return currentDateTime >= startTime && quarterEnd <= endTime;
    });
  }

  isCorner(day: string, hour: string, quarter: number): boolean {
    const currentDateTime = this.createDateTime(day, hour, quarter);

    // Check active selection
    if (this.selectionStart?.datetime) {
      if (currentDateTime.getTime() === this.selectionStart.datetime.getTime()) {
        return true;
      }
    }

    // Check saved selections
    return this.selections.some(selection =>
      currentDateTime.getTime() === selection.start.datetime.getTime()
    );
  }

  removeAllSelections(): void {
    this.selections = [];
    this.selectionStart = null;
    this.selectionEnd = null;
    this.isSelecting = false;
  }

  removeSelectionById(id: number): void {
    this.selections = this.selections.filter(selection => selection.id !== id);
  }

  private createDateTime(day: string, hour: string, quarter: number): Date {
    const currentYear = new Date().getFullYear();
    const currentMonth = new Date().getMonth();

    // Parse hour (now in 24-hour format)
    const hours = parseInt(hour.split(':')[0]);

    // Calculate minutes based on quarter (0-3)
    const minutes = quarter * 15;

    return new Date(currentYear, currentMonth, parseInt(day), hours, minutes);
  }

  getSelectionId(day: string, hour: string, quarter: number): number {
    const currentDateTime = this.createDateTime(day, hour, quarter);
    const selection = this.selections.find(s =>
      s.start.datetime.getTime() === currentDateTime.getTime()
    );
    return selection?.id ?? -1;
  }
}
