import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Organization } from '../models/organization.model';
import {
  PaginatedResponse,
  PaginationOptions,
  serializePaginationOptions,
} from '@shared/components/pagination/pagination.component';
import { Invitation } from '../../invitations/models/invitation';
import { BaseApiService } from '@shared/services/base-api.service';
import { environment } from '@environments/environment';
export interface CreateOrganizationPayload {
  name: string;
  description?: string;
}

export interface UpdateOrganizationPayload {
  name?: string;
  description?: string;
  profileImage?: string;
  bannerImage?: string;
}

@Injectable({
  providedIn: 'root',
})
export class OrganizationApiService extends BaseApiService<Organization> {
  override readonly apiUrl = `${environment.apiUrl}/api/identity/Organizations`;

  getInvitations(
    query: Record<any, any>,
    paginationOptions: PaginationOptions<Invitation>,
  ): Observable<PaginatedResponse<Invitation>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Invitation>>(`${this.apiUrl}/invitations`, {
      params: { ...query, ...options },
    });
  }
}
