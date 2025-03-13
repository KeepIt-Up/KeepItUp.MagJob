import { HttpErrorResponse } from '@angular/common/http';
import { Signal, computed, signal } from '@angular/core';

export interface BaseState<T> {
    data: T | null;
    loading: boolean;
    error: string | HttpErrorResponse | undefined;
}

//TODO to remove
export abstract class BaseStore<T> {
    protected readonly state = signal<BaseState<T>>({
        data: null,
        loading: true,
        error: undefined
    });

    constructor() {
        this.state.set({
            data: null,
            loading: true,
            error: undefined
        });
    }



    // Selectors
    readonly data = computed(() => this.state().data);
    readonly loading = computed(() => this.state().loading);
    readonly error = computed(() => this.state().error);

    protected setLoading(): void {
        this.state.update(state => ({ ...state, loading: true, error: undefined }));
    }

    protected setData(data: T): void {
        this.state.update(state => ({
            ...state,
            data,
            loading: false
        }));
    }

    setError(error: string | HttpErrorResponse): void {
        this.state.update(state => ({
            ...state,
            error,
            loading: false,
            data: null
        }));
    }

    clear(): void {
        this.state.update(state => ({
            ...state,
            data: null,
            error: undefined
        }));
    }
} 