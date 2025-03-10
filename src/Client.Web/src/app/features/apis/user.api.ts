import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { environment } from '@environments/environment';
import { Invitation } from '@features/models/invitation/invitation';
import { Organization } from '@features/models/organization/organization';
import { User } from '@features/models/user/user';
import { PaginatedResponse, PaginationOptions, serializePaginationOptions } from '@shared/components/pagination/pagination.component';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserApiService {

  private readonly apiUrl = '/api/users';
  private http = inject(HttpClient);

  getUserOrganizations(query: Record<any, any>, paginationOptions: PaginationOptions<Organization>): Observable<PaginatedResponse<Organization>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Organization>>(`${this.apiUrl}/organizations`, { params: { ...query, ...options } });
  }

  getUserInvitations(query: Record<any, any>, paginationOptions: PaginationOptions<Invitation>): Observable<PaginatedResponse<Invitation>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Invitation>>(`${this.apiUrl}/invitations`, { params: { ...query, ...options } });
  }

  getAll(query: Record<any, any>, paginationOptions: PaginationOptions<User>) {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<User>>(`${this.apiUrl}`, { params: { ...query, ...options } });
  }

  create(): Observable<User> {
    return this.http.post<User>(`${this.apiUrl}`, {});
  }

  getUser() {
    return this.http.get<User>(`${this.apiUrl}/me`);
  }

  delete() {

  }

  update() {

  }

}
