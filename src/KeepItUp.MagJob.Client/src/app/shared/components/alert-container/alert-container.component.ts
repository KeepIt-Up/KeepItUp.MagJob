import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AlertComponent } from '@shared/components/alert/alert.component';
import { AlertConfig, AlertService } from '@shared/services/alert.service';

@Component({
  selector: 'app-alert-container',
  standalone: true,
  imports: [CommonModule, AlertComponent],
  template: `
    <div class="app-alert-container">
      @for (alert of alerts; track alert.id) {
        <app-alert
          [title]="alert.title"
          [description]="alert.description"
          [alertType]="alert.type"
          [alertVisible]="true"
          [showActionButtons]="alert.showActionButtons ?? false"
          [showViewMoreButton]="alert.showViewMoreButton ?? false"
          [showDismissButton]="alert.showDismissButton ?? true"
          [viewMoreText]="alert.viewMoreText ?? 'View more'"
          [dismissText]="alert.dismissText ?? 'Dismiss'"
          [iconName]="alert.iconName ?? undefined"
          [showDefaultIcon]="alert.showDefaultIcon ?? true"
          [customClass]="alert.customClass ?? ''"
          [autoHideTimeout]="alert.autoHideTimeout ?? 0"
          (viewMoreClick)="onViewMore(alert)"
          (dismissClick)="onDismiss(alert)"
        ></app-alert>
      }
    </div>
  `,
  styles: `
    .app-alert-container {
      position: fixed;
      top: 20px;
      right: 20px;
      z-index: 1000;
      display: flex;
      flex-direction: column;
      gap: 10px;
      max-width: 400px;

      app-alert {
        animation: fadeIn 0.3s ease-in-out;
      }

      @keyframes fadeIn {
        from {
          opacity: 0;
          transform: translateY(-20px);
        }
        to {
          opacity: 1;
          transform: translateY(0);
        }
      }
    }
  `,
})
export class AlertContainerComponent implements OnInit {
  alerts: AlertConfig[] = [];
  private alertService = inject(AlertService);

  ngOnInit(): void {
    this.alertService.alerts$.subscribe(alerts => {
      this.alerts = alerts;
    });
  }

  onViewMore(alert: AlertConfig): void {
    if (alert.onViewMore) {
      alert.onViewMore();
    }
    // Don't dismiss on view more unless explicitly configured to do so
  }

  onDismiss(alert: AlertConfig): void {
    if (alert.onDismiss) {
      alert.onDismiss();
    }
    if (alert.id) {
      this.alertService.clearAlert(alert.id);
    }
  }
}
