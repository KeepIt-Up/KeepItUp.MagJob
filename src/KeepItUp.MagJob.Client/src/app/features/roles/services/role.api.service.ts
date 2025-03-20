import { Injectable } from '@angular/core';
import {
  PaginatedResponse,
  PaginationOptions,
  serializePaginationOptions,
} from '@shared/components/pagination/pagination.component';
import { BaseApiService } from '@shared/services/base-api.service';
import { Observable } from 'rxjs';
import { Role } from '../models/role.model';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root',
})
export class RoleApiService extends BaseApiService<Role> {
  override readonly apiUrl = `${environment.apiUrl}/api/identity/organizations`;

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

  updateRolePermissions(
    organizationId: string,
    roleId: string,
    permissionIds: number[],
  ): Observable<void> {
    return this.http.post<void>(
      `${this.apiUrl}/${organizationId}/roles/${roleId}/permissions`,
      permissionIds,
    );
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
}
