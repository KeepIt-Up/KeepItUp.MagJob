import { inject, Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { PermissionApiService } from '../services/permission.api.service';
import { StateService } from '@shared/services/state.service';
import { Permission } from './role.model';

@Injectable({
  providedIn: 'root',
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
      next: permissions => this.permissionStateService.setData(permissions),
      error: error => this.permissionStateService.setError(error as HttpErrorResponse),
    });
  }
}
