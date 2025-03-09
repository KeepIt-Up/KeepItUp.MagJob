import { CommonModule, NgClass } from '@angular/common';
import { Component, input, output } from '@angular/core';

export type AlertType = 'info' | 'danger' | 'success' | 'warning' | 'dark';

@Component({
  selector: 'app-alert',
  imports: [CommonModule, NgClass],
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.scss']
})
export class AlertComponent {
  /**
   * @description The title of the alert
   */
  title = input.required<string>();
  /**
   * @description Whether to show the action buttons
   */
  showActionButtons = input<boolean>(false);
  /**
   * @description The type of the alert
   */
  alertType = input<AlertType>('dark');
  /**
   * @description The description of the alert
   */
  description = input<string>('');
  /**
   * @description Whether the alert is visible
   */
  alertVisible = input<boolean>(true);
  /**
   * @description The event emitted when the dismiss button is clicked
   */
  dismissClick = output<void>();
  /**
   * @description The event emitted when the view more button is clicked
   */
  viewMoreClick = output<void>();
}
