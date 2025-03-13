import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Organization } from '@features/models/organization/organization';
import {
  PaginatedResponse,
  PaginationOptions,
  serializePaginationOptions,
} from '@shared/components/pagination/pagination.component';
import { Invitation } from '@features/models/invitation/invitation';
import { BaseApiService } from '@shared/services/base-api.service';

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
  override readonly apiUrl = '/api/organizations';

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
