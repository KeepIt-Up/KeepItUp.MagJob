export interface DataState<T> {
  data: T | null;
  loading: boolean;
  error: string | null;
  timestamp: number;
}

export const createInitialState = <T>(): DataState<T> => ({
  data: null,
  loading: false,
  error: null,
  timestamp: Date.now(),
});

export const createLoadingState = <T>(previousData: T | null = null): DataState<T> => ({
  data: previousData,
  loading: true,
  error: null,
  timestamp: Date.now(),
});

export const createSuccessState = <T>(data: T): DataState<T> => ({
  data,
  loading: false,
  error: null,
  timestamp: Date.now(),
});

export const createErrorState = <T>(
  error: string,
  previousData: T | null = null,
): DataState<T> => ({
  data: previousData,
  loading: false,
  error,
  timestamp: Date.now(),
});
