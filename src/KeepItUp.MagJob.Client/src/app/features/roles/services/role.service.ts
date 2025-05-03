import { inject, Injectable, signal } from '@angular/core';
import {
  PaginatedResponse,
  PaginationOptions,
} from '@shared/components/pagination/pagination.component';
import { NotificationService } from '@shared/services/notification.service';
import { catchError, tap, throwError } from 'rxjs';
import { PermissionService } from '../models/permission.service';
import { ListStateService } from '@shared/services/list-state.service';
import { RoleApiService } from './role.api.service';
import { Role } from '../models/role.model';
import { OrganizationService } from '@organizations/services/organization.service';
import { HttpErrorResponse } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class RoleService {
  private readonly apiService = inject(RoleApiService);
  private readonly notificationService = inject(NotificationService);
  private readonly permissionService = inject(PermissionService);
  private readonly roleStateService = new ListStateService<Role, { endOfData: boolean }>();
  private readonly organizationService = inject(OrganizationService);

  permissions$ = this.permissionService.permissionState$;
  roles$ = this.roleStateService.state$;

  $organization = this.organizationService.state$;

  paginationOptions$ = signal<PaginationOptions<Role>>({
    pageNumber: 1,
    pageSize: 10,
    sortField: 'id',
    ascending: true,
  });

  selectedRole$ = signal<Role | undefined>(undefined);
  selectRole(role: Role) {
    this.selectedRole$.set(role);
  }

  getRoles(organizationId: string) {
    const query = { id: organizationId };
    return this.apiService.getAllRoles(organizationId, query, this.paginationOptions$()).pipe(
      tap((response: PaginatedResponse<Role>) => {
        response.items.forEach(role => this.roleStateService.add(role));
        this.roleStateService.setMetadata({ endOfData: !response.hasNextPage });
      }),
      catchError((error: Error | HttpErrorResponse) => {
        this.roleStateService.setError(error);
        return throwError(() => error);
      }),
    );
  }

  createRole(organizationId: string, name: string) {
    return this.apiService.createRole(organizationId, name).pipe(
      tap(createdRole => {
        this.roleStateService.setData([...(this.roles$().data ?? []), createdRole]);
      }),
      catchError((error: Error | HttpErrorResponse) => {
        this.roleStateService.setError(error);
        this.notificationService.show('Failed to create role', 'error');
        return throwError(() => error);
      }),
    );
  }

  updateRole(roleId: string, role: Partial<Role>) {
    return this.apiService.update(roleId, role).pipe(
      tap(updatedRole => {
        this.roleStateService.update(updatedRole);
        this.notificationService.show('Role updated successfully', 'success');
      }),
      catchError(error => {
        this.notificationService.show('Failed to update role', 'error');
        return throwError(() => error);
      }),
    );
  }

  deleteRole() {
    const organizationData = this.$organization().data;
    if (!organizationData || !this.selectedRole$()) {
      this.notificationService.show('Organization data or role is not available', 'error');
      return throwError(() => new Error('Organization data or role is not available'));
    }

    return this.apiService.delete(this.selectedRole$()!.id, organizationData.id).pipe(
      tap(() => {
        this.roleStateService.remove(this.selectedRole$()!);
        this.selectedRole$.set(undefined);
        this.notificationService.show('Role deleted successfully', 'success');
      }),
      catchError(error => {
        this.notificationService.show('Failed to delete role', 'error');
        return throwError(() => error);
      }),
    );
  }

  updateRolePermissions(roleId: string, permissionNames: string[]) {
    const organizationData = this.$organization().data;
    if (!organizationData) {
      this.notificationService.show('Organization data is not available', 'error');
      return throwError(() => new Error('Organization data is not available'));
    }

    return this.apiService.updateRolePermissions(organizationData.id, roleId, permissionNames).pipe(
      tap(() => {
        if (this.selectedRole$()) {
          const allPermissions = this.permissions$().data ?? [];

          const selectedPermissions = allPermissions
            .filter(p => permissionNames.includes(p.name))
            .map(p => ({ ...p }));

          const updatedRole = {
            ...this.selectedRole$()!,
            permissions: selectedPermissions,
          };

          this.selectedRole$.set(updatedRole);
          this.roleStateService.update(updatedRole);
        }

        this.notificationService.show('Permissions updated successfully', 'success');
      }),
      catchError(error => {
        this.notificationService.show('Failed to update permissions', 'error');
        return throwError(() => error);
      }),
    );
  }

  addMembersToRole(roleId: string, memberIds: string[]) {
    const organizationData = this.$organization().data;
    if (!organizationData) {
      this.notificationService.show('Organization data is not available', 'error');
      return throwError(() => new Error('Organization data is not available'));
    }

    return this.apiService.addMembersToRole(organizationData.id, roleId, memberIds).pipe(
      tap(() => {
        this.notificationService.show('Members assigned successfully', 'success');
      }),
      catchError(error => {
        this.notificationService.show('Failed to assign members', 'error');
        return throwError(() => error);
      }),
    );
  }

  removeMembersFromRole(roleId: string, memberIds: string[]) {
    const organizationData = this.$organization().data;
    if (!organizationData) {
      this.notificationService.show('Organization data is not available', 'error');
      return throwError(() => new Error('Organization data is not available'));
    }

    return this.apiService.removeMembersFromRole(organizationData.id, roleId, memberIds).pipe(
      tap(() => {
        this.notificationService.show('Members removed successfully', 'success');
      }),
      catchError(error => {
        this.notificationService.show('Failed to remove members', 'error');
        return throwError(() => error);
      }),
    );
  }
}
