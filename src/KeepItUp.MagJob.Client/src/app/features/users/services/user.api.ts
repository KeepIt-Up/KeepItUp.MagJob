import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { User } from '@users/models/user.model';
import {
  PaginatedResponse,
  PaginationOptions,
  serializePaginationOptions,
} from '@shared/components/pagination/pagination.component';
import { catchError, Observable, throwError } from 'rxjs';
import { Organization } from '../../organizations/models/organization.model';
import { Invitation } from '../../invitations/models/invitation';
import { environment } from '@environments/environment';
import { CurrentUser } from '@users/models/current-user.model';

@Injectable({
  providedIn: 'root',
})
export class UserApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/identity/users`;
  private http = inject(HttpClient);

  getUserOrganizations(
    query: Record<any, any>,
    paginationOptions: PaginationOptions<Organization>,
  ): Observable<PaginatedResponse<Organization>> {
    const id = '0195b59c-39dc-7c7e-8f5a-66814c8f88b0';
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Organization>>(`${this.apiUrl}/${id}/organizations`, {
      params: { ...options },
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

  getAll(query: Record<any, any>, paginationOptions: PaginationOptions<User>) {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<User>>(`${this.apiUrl}`, {
      params: { ...query, ...options },
    });
  }

  getCurrentUser(): Observable<CurrentUser> {
    return this.http
      .get<CurrentUser>(`${this.apiUrl}/me`)
      .pipe(catchError((error: HttpErrorResponse) => throwError(() => new Error(error.message))));
  }

  getUserById(id: string): Observable<CurrentUser> {
    return this.http
      .get<CurrentUser>(`${this.apiUrl}/${id}`)
      .pipe(catchError((error: HttpErrorResponse) => throwError(() => new Error(error.message))));
  }
}
