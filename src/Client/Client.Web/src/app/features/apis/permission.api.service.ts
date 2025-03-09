import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Permission } from '@features/models/role/role';
import { Observable } from 'rxjs';
import { environment } from '@environments/environment';


@Injectable({
  providedIn: 'root'
})
export class PermissionApiService {

  private readonly apiUrl = '/api/permissions';
  private http = inject(HttpClient);

  getAllPermissions(): Observable<Permission[]> {
    return this.http.get<Permission[]>(this.apiUrl);
  }
}
