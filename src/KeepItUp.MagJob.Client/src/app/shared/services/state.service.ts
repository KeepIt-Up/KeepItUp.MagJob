import { HttpErrorResponse } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';

export interface State<T, M = undefined> {
  loading: boolean;
  error?: HttpErrorResponse | Error;
  data?: T;
  metadata?: M;
}

@Injectable()
export class StateService<T, M = undefined> {
  protected state = signal<State<T, M>>({
    loading: true
  });

  state$ = this.state.asReadonly();

  reset() {
    this.state.set({ loading: true });
  }

  setState(state: State<T, M>) {
    this.state.set(state);
  }

  protected updateState(state: Partial<State<T, M>>) {
    this.state.update((prev) => ({
      ...prev,
      ...state
    }));
  }

  setLoading(loading: boolean) {
    this.state.update((prev) => ({
      ...prev,
      loading
    }));
  }

  setError(error: HttpErrorResponse | Error) {
    this.state.update((prev) => ({
      ...prev,
      loading: false,
      error
    }));
  }

  setData(data: T) {
    this.state.update((prev) => ({
      ...prev,
      loading: false,
      data: data
    }));
  }

}
