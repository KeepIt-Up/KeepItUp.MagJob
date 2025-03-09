import { Component, computed, effect, input } from '@angular/core';
import { ErrorAlertComponent } from '@shared/components/error-alert/error-alert.component';
import { SpinnerComponent } from '@shared/components/spinner/spinner.component';
import { State } from '@shared/services/state.service';

@Component({
  selector: 'app-statefull-container',
  imports: [SpinnerComponent, ErrorAlertComponent],
  template: `
    @if (state$().loading) {
    <app-spinner></app-spinner>
    }

    @if (state$().error; as error) {
    <app-error-alert [error]="error"></app-error-alert>
    }

    @if (state$().data) {
      <ng-content></ng-content>
    }
  `
})
export class StatefullContainerComponent<T, M = undefined> {
  state$ = input.required<State<T, M>>();

  constructor() {
    effect(() => {
      console.log(this.state$());
    });
  }
}
