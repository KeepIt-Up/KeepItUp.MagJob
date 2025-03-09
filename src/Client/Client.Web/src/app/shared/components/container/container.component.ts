import { Component, Input, OnInit, OnDestroy, EventEmitter, Output, SimpleChanges, OnChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SpinnerComponent } from '../spinner/spinner.component';
import { Observable, Subject, takeUntil } from 'rxjs';
import { HttpErrorResponse } from '@angular/common/http';
import { HttpErrorResponseAlertComponent } from '../http-error-response-alert/http-error-response-alert.component';

export interface ContainerState<T> {
  isLoading: boolean;
  error?: HttpErrorResponse;
  data?: T;
}

@Component({
  selector: 'app-container',
  standalone: true,
  imports: [CommonModule, SpinnerComponent, HttpErrorResponseAlertComponent],
  templateUrl: './container.component.html'
})
export class ContainerComponent<T> implements OnInit, OnChanges, OnDestroy {
  @Input() source$!: Observable<T>;
  @Output() onStateChange = new EventEmitter<ContainerState<T>>();

  _state: ContainerState<T> = {
    isLoading: true
  };

  private destroy$ = new Subject<void>();

  ngOnInit() {
    this.onStateChange.emit(this._state);
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['source$']) {
      this.subscribeToSource();
    }
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private updateState(isLoading: boolean, data?: T, error?: HttpErrorResponse) {
    this._state = { isLoading, data, error };
    this.onStateChange.emit(this._state);
  }

  private subscribeToSource() {
    this.source$.pipe(
      takeUntil(this.destroy$)
    ).subscribe({
      next: (data) => this.updateState(false, data),
      error: (error) => this.updateState(false, undefined, error)
    });
  }

}
