import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { PaginatedResponse, PaginationOptions, serializePaginationOptions } from '@shared/components/pagination/pagination.component';

@Injectable({
  providedIn: 'root'
})
export class BaseApiService<T extends { id: string }> {
  protected readonly apiUrl: string = '';
  protected http = inject(HttpClient);

  getAll(query: Record<any, any>, paginationOptions: PaginationOptions<T>) {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<T>>(`${this.apiUrl}`, { params: { ...query, ...options } });
  }

  get(id: string) {
    return this.http.get<T>(`${this.apiUrl}/${id}`);
  }

  create<TPayload>(payload: TPayload) {
    return this.http.post<T>(this.apiUrl, payload);
  }

  update<TPayload>(id: string, payload: TPayload) {
    return this.http.put<T>(`${this.apiUrl}/${id}`, payload);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
