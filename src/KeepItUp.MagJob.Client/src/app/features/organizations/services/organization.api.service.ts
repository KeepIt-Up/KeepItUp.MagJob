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
import { Member } from '@members/models/member';

export interface CreateOrganizationPayload {
  name: string;
  description?: string;
}

export interface UpdateOrganizationPayload {
  name?: string;
  description?: string;
}

@Injectable({
  providedIn: 'root',
})
export class OrganizationApiService extends BaseApiService<Organization> {
  override readonly apiUrl = `${environment.apiUrl}/api/identity/Organizations`;

  /**
   * Updates logo for an organization using FormData
   */
  updateLogo(organizationId: string, logoFile: File): Observable<{ logoUrl: string }> {
    const formData = new FormData();
    formData.append('logoFile', logoFile);

    return this.http.put<{ logoUrl: string }>(`${this.apiUrl}/${organizationId}/Logo`, formData);
  }

  /**
   * Updates banner for an organization using FormData
   */
  updateBanner(organizationId: string, bannerFile: File): Observable<{ bannerUrl: string }> {
    const formData = new FormData();
    formData.append('bannerFile', bannerFile);

    return this.http.put<{ bannerUrl: string }>(
      `${this.apiUrl}/${organizationId}/Banner`,
      formData,
    );
  }

  getInvitations(
    organizationId: string,
    query: Record<any, any>,
    paginationOptions: PaginationOptions<Invitation>,
  ): Observable<PaginatedResponse<Invitation>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Invitation>>(
      `${this.apiUrl}/${organizationId}/invitations`,
      {
        params: { ...query, ...options },
      },
    );
  }

  archiveMember(organizationId: string, memberId: string) {
    return this.http.put(`${this.apiUrl}/${organizationId}/members/${memberId}/archive`, {});
  }

  getMembers(
    organizationId: string,
    query: Record<any, any>,
    paginationOptions: PaginationOptions<Member>,
  ): Observable<PaginatedResponse<Member>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Member>>(`${this.apiUrl}/${organizationId}/members`, {
      params: { ...query, ...options },
    });
  }

  searchMembers(
    query: Record<any, any>,
    paginationOptions: PaginationOptions<Member>,
  ): Observable<PaginatedResponse<Member>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Member>>(`${this.apiUrl}/members/search`, {
      params: { ...query, ...options },
    });
  }
}
