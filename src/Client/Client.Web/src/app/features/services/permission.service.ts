import { inject, Injectable } from '@angular/core';
import { PermissionApiService } from '@features/apis/permission.api.service';
import { Permission } from '@features/models/role/role';
import { StateService } from '@shared/services/state.service';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {
  private readonly permissionApiService = inject(PermissionApiService);
  private readonly permissionStateService = new StateService<Permission[]>();

  permissionState$ = this.permissionStateService.state$;

  constructor() {
    this.getPermissions();
  }

  getPermissions() {
    this.permissionApiService.getAllPermissions().subscribe({
      next: (permissions) => this.permissionStateService.setData(permissions),
      error: (error) => this.permissionStateService.setError(error)
    });
  }
}
