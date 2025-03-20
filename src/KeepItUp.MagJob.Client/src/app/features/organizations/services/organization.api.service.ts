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

  archiveMember(memberId: string) {
    return this.http.put(`${this.apiUrl}/${memberId}/archive`, {});
  }

  getMembers(
    query: Record<any, any>,
    paginationOptions: PaginationOptions<Member>,
  ): Observable<PaginatedResponse<Member>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Member>>(`${this.apiUrl}/members`, {
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
