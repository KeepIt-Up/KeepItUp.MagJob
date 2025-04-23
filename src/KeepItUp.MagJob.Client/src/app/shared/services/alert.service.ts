import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AlertType } from '@shared/components/alert/alert.component';

export interface AlertConfig {
  id?: string;
  title: string;
  description: string;
  type: AlertType;
  showActionButtons?: boolean;
  showViewMoreButton?: boolean;
  showDismissButton?: boolean;
  viewMoreText?: string;
  dismissText?: string;
  iconName?: string | null;
  showDefaultIcon?: boolean;
  customClass?: string;
  autoHideTimeout?: number;
  onViewMore?: () => void;
  onDismiss?: () => void;
}

@Injectable({
  providedIn: 'root',
})
export class AlertService {
  private alertsSubject = new BehaviorSubject<AlertConfig[]>([]);
  public alerts$: Observable<AlertConfig[]> = this.alertsSubject.asObservable();

  /**
   * Show an alert with the given configuration
   */
  showAlert(config: AlertConfig): string {
    const id = config.id || this.generateId();
    const alerts = this.alertsSubject.value;

    const alertConfig: AlertConfig = {
      ...config,
      id,
    };

    this.alertsSubject.next([...alerts, alertConfig]);
    return id;
  }

  /**
   * Show a success alert
   */
  success(title: string, description: string, options: Partial<AlertConfig> = {}): string {
    return this.showAlert({
      title,
      description,
      type: 'success',
      autoHideTimeout: 5000, // Auto-hide after 5 seconds by default
      ...options,
    });
  }

  /**
   * Show an error alert
   */
  error(title: string, description: string, options: Partial<AlertConfig> = {}): string {
    return this.showAlert({
      title,
      description,
      type: 'danger',
      ...options,
    });
  }

  /**
   * Show an info alert
   */
  info(title: string, description: string, options: Partial<AlertConfig> = {}): string {
    return this.showAlert({
      title,
      description,
      type: 'info',
      autoHideTimeout: 5000, // Auto-hide after 5 seconds by default
      ...options,
    });
  }

  /**
   * Show a warning alert
   */
  warning(title: string, description: string, options: Partial<AlertConfig> = {}): string {
    return this.showAlert({
      title,
      description,
      type: 'warning',
      ...options,
    });
  }

  /**
   * Clear a specific alert by ID
   */
  clearAlert(id: string): void {
    const alerts = this.alertsSubject.value;
    const filteredAlerts = alerts.filter(alert => alert.id !== id);
    this.alertsSubject.next(filteredAlerts);
  }

  /**
   * Clear all alerts
   */
  clearAll(): void {
    this.alertsSubject.next([]);
  }

  /**
   * Generate a unique ID for an alert
   */
  private generateId(): string {
    return `alert-${Date.now()}-${Math.random().toString(36).substr(2, 9)}`;
  }
}
