import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { UserBase } from '@core/models/user-base.model';
import {
  PaginatedResponse,
  PaginationOptions,
  serializePaginationOptions,
} from '@shared/components/pagination/pagination.component';
import { Observable } from 'rxjs';
import { Organization } from '../../organizations/models/organization.model';
import { Invitation } from '../../invitations/models/invitation';

@Injectable({
  providedIn: 'root',
})
export class UserApiService {
  private readonly apiUrl = '/api/users';
  private http = inject(HttpClient);

  getUserOrganizations(
    query: Record<any, any>,
    paginationOptions: PaginationOptions<Organization>,
  ): Observable<PaginatedResponse<Organization>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Organization>>(`${this.apiUrl}/organizations`, {
      params: { ...query, ...options },
    });
  }

  getUserInvitations(
    query: Record<any, any>,
    paginationOptions: PaginationOptions<Invitation>,
  ): Observable<PaginatedResponse<Invitation>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Invitation>>(`${this.apiUrl}/invitations`, {
      params: { ...query, ...options },
    });
  }

  getAll(query: Record<any, any>, paginationOptions: PaginationOptions<UserBase>) {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<UserBase>>(`${this.apiUrl}`, {
      params: { ...query, ...options },
    });
  }
}
