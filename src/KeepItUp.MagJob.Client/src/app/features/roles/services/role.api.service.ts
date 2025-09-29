import { inject, Injectable } from '@angular/core';
import {
  PaginatedResponse,
  PaginationOptions,
  serializePaginationOptions,
} from '@shared/components/pagination/pagination.component';
import { Observable } from 'rxjs';
import { Role } from '../models/role.model';
import { environment } from '@environments/environment';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class RoleApiService {
  readonly apiUrl = `${environment.apiUrl}/api/identity/organizations`;
  private readonly http = inject(HttpClient);

  getAllRoles(
    organizationId: string,
    query: Record<any, any>,
    paginationOptions: PaginationOptions<Role>,
  ) {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Role>>(`${this.apiUrl}/${organizationId}/roles`, {
      params: { ...query, ...options },
    });
  }

  createRole(organizationId: string, name: string) {
    return this.http.post<Role>(`${this.apiUrl}/${organizationId}/roles`, { name });
  }

  updateRolePermissions(
    organizationId: string,
    roleId: string,
    permissionNames: string[],
  ): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${organizationId}/roles/${roleId}/permissions`, {
      permissions: permissionNames,
    });
  }

  addMembersToRole(organizationId: string, roleId: string, memberIds: string[]): Observable<void> {
    return this.http.post<void>(
      `${this.apiUrl}/${organizationId}/roles/${roleId}/members`,
      memberIds,
    );
  }

  removeMembersFromRole(
    organizationId: string,
    roleId: string,
    memberIds: string[],
  ): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${organizationId}/roles/${roleId}/members`, {
      body: memberIds,
    });
  }

  delete(id: string, organizationId?: string): Observable<void> {
    if (organizationId) {
      return this.http.delete<void>(`${this.apiUrl}/${organizationId}/roles/${id}`);
    } else {
      return this.http.delete<void>(`${this.apiUrl}/${id}`);
    }
  }

  getAll(query: Record<any, any>, paginationOptions: PaginationOptions<Role>) {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Role>>(`${this.apiUrl}`, {
      params: { ...query, ...options },
    });
  }

  get(id: string) {
    return this.http.get<Role>(`${this.apiUrl}/${id}`);
  }

  create(payload: Role) {
    return this.http.post<Role>(this.apiUrl, payload);
  }

  update(id: string, payload: Partial<Role>) {
    return this.http.put<Role>(`${this.apiUrl}/${id}`, payload);
  }
}
