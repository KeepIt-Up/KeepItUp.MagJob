import { CommonModule, NgClass } from '@angular/common';
import { Component, input, output, OnInit, DestroyRef, inject } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NgIcon } from '@ng-icons/core';
import { timer } from 'rxjs';
import { ButtonComponent } from '../button/button.component';

export type AlertType = 'info' | 'danger' | 'success' | 'warning' | 'dark';

@Component({
  selector: 'app-alert',
  imports: [CommonModule, NgClass, NgIcon, ButtonComponent],
  templateUrl: './alert.component.html',
  styleUrls: ['./alert.component.scss'],
})
export class AlertComponent implements OnInit {
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
   * @description Whether to show the view more button
   */
  showViewMoreButton = input<boolean>(true);
  /**
   * @description Whether to show the dismiss button
   */
  showDismissButton = input<boolean>(true);
  /**
   * @description Custom text for the view more button
   */
  viewMoreText = input<string>('View more');
  /**
   * @description Custom text for the dismiss button
   */
  dismissText = input<string>('Dismiss');
  /**
   * @description Custom icon name (if using an icon library)
   */
  iconName = input<string | undefined>(undefined);
  /**
   * @description Whether to show the default icon
   */
  showDefaultIcon = input<boolean>(true);
  /**
   * @description Custom CSS class for the alert
   */
  customClass = input<string>('');
  /**
   * @description Auto-hide timeout in milliseconds (0 means no auto-hide)
   */
  autoHideTimeout = input<number>(0);
  /**
   * @description The event emitted when the dismiss button is clicked
   */
  dismissClick = output<void>();
  /**
   * @description The event emitted when the view more button is clicked
   */
  viewMoreClick = output<void>();

  private destroyRef = inject(DestroyRef);

  ngOnInit(): void {
    this.setupAutoHide();
  }

  private setupAutoHide(): void {
    const timeoutMs = this.autoHideTimeout();
    if (timeoutMs > 0) {
      timer(timeoutMs)
        .pipe(takeUntilDestroyed(this.destroyRef))
        .subscribe(() => {
          if (this.alertVisible()) {
            this.dismissClick.emit();
          }
        });
    }
  }
}
