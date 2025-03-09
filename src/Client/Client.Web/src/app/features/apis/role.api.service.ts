import { Injectable } from "@angular/core";
import { Role } from "@features/models/role/role";
import { PaginatedResponse, PaginationOptions, serializePaginationOptions } from "@shared/components/pagination/pagination.component";
import { BaseApiService } from "@shared/services/base-api.service";
import { Observable } from "rxjs";
import { environment } from "@environments/environment";

@Injectable({
  providedIn: 'root'
})
export class RoleApiService extends BaseApiService<Role> {
  override readonly apiUrl = '/api/roles';

  getAllRoles(query: Record<any, any>, paginationOptions: PaginationOptions<Role>) {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Role>>(`/api/organizations/roles`, { params: { ...query, ...options } });
  }

  updateRolePermissions(roleId: string, permissionIds: number[]): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${roleId}/permissions`, permissionIds);
  }

  addMembersToRole(roleId: string, memberIds: string[]): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${roleId}/members`, memberIds);
  }

  removeMembersFromRole(roleId: string, memberIds: string[]): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${roleId}/members`, { body: memberIds });
  }
}
