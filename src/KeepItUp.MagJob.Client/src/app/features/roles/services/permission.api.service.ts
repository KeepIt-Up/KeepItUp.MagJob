import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Permission } from '../models/role.model';
import { environment } from '@environments/environment';
@Injectable({
  providedIn: 'root',
})
export class PermissionApiService {
  private readonly apiUrl = `${environment.apiUrl}/api/identity/permissions`;
  private http = inject(HttpClient);

  getAllPermissions(): Observable<Permission[]> {
    return this.http.get<Permission[]>(this.apiUrl);
  }
}
