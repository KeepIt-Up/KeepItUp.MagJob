import { query } from '@angular/animations';
import { inject, Injectable, signal } from '@angular/core';
import { RoleApiService } from '@features/apis/role.api.service';
import { Permission, Role } from '@features/models/role/role';
import { PaginatedResponse, PaginationOptions } from '@shared/components/pagination/pagination.component';
import { NotificationService } from '@shared/services/notification.service';
import { StateService } from '@shared/services/state.service';
import { catchError, tap, throwError } from 'rxjs';
import { PermissionService } from './permission.service';
import { ListStateService } from '@shared/services/list-state.service';

@Injectable({
  providedIn: 'root'
})
export class RoleService {
  private readonly apiService = inject(RoleApiService);
  private readonly notificationService = inject(NotificationService);
  private readonly permissionService = inject(PermissionService);
  private readonly roleStateService = new ListStateService<Role, { endOfData: boolean }>();

  permissions$ = this.permissionService.permissionState$;
  roles$ = this.roleStateService.state$;

  paginationOptions$ = signal<PaginationOptions<Role>>({
    pageNumber: 1,
    pageSize: 10,
    sortField: "id",
    ascending: true
  });


  selectedRole$ = signal<Role | undefined>(undefined);
  selectRole(role: Role) {
    this.selectedRole$.set(role);
  }

  getRoles(organizationId: string) {
    const query = { id: organizationId };
    return this.apiService.getAllRoles(query, this.paginationOptions$()).pipe(
      tap((response: PaginatedResponse<Role>) => {
        response.items.forEach((role) => this.roleStateService.add(role));
        this.roleStateService.setMetadata({ endOfData: !response.hasNextPage });
      }),
      catchError((error) => {
        this.roleStateService.setError(error);
        throw error;
      })
    );
  }

  createRole(organizationId: string, name: string) {
    return this.apiService.create({ organizationId: organizationId, name: name }).pipe(
      tap((createdRole) => {
        this.roleStateService.setData([...this.roles$().data ?? [], createdRole]);
      }),
      catchError((error) => {
        this.roleStateService.setError(error);
        this.notificationService.show('Failed to create role', 'error');
        return throwError(() => error);
      })
    );
  }

  updateRole(roleId: string, role: Partial<Role>) {
    return this.apiService.update(roleId, role).pipe(
      tap((updatedRole) => {
        this.roleStateService.update(updatedRole);
        this.notificationService.show('Role updated successfully', 'success');
      }),
      catchError((error) => {
        this.notificationService.show('Failed to update role', 'error');
        return throwError(() => error);
      })
    );
  }

  deleteRole() {
    return this.apiService.delete(this.selectedRole$()!.id).pipe(
      tap(() => {
        this.roleStateService.remove(this.selectedRole$()!);
        this.selectedRole$.set(undefined);
        this.notificationService.show('Role deleted successfully', 'success');
      }),
      catchError((error) => {
        this.notificationService.show('Failed to delete role', 'error');
        return throwError(() => error);
      })
    );
  }

  updateRolePermissions(roleId: string, permissionIds: number[]) {
    return this.apiService.updateRolePermissions(roleId, permissionIds).pipe(
      tap(() => {
        this.notificationService.show('Permissions updated successfully', 'success');
      }),
      catchError((error) => {
        this.notificationService.show('Failed to update permissions', 'error');
        return throwError(() => error);
      })
    );
  }

  addMembersToRole(roleId: string, memberIds: string[]) {
    return this.apiService.addMembersToRole(roleId, memberIds).pipe(
      tap(() => {
        this.notificationService.show('Members assigned successfully', 'success');
      }),
      catchError((error) => {
        this.notificationService.show('Failed to assign members', 'error');
        return throwError(() => error);
      })
    );
  }

  removeMembersFromRole(roleId: string, memberIds: string[]) {
    return this.apiService.removeMembersFromRole(roleId, memberIds).pipe(
      tap(() => {
        this.notificationService.show('Members removed successfully', 'success');
      }),
      catchError((error) => {
        this.notificationService.show('Failed to remove members', 'error');
        return throwError(() => error);
      })
    );
  }
}
