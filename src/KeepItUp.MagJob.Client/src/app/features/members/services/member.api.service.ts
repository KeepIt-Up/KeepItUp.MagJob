import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from '../models/member';
import { BaseApiService } from '@shared/services/base-api.service';
import { PaginationOptions } from '@shared/components/pagination/pagination.component';
import { PaginatedResponse } from '@shared/components/pagination/pagination.component';
import { serializePaginationOptions } from '@shared/components/pagination/pagination.component';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root',
})
export class MemberApiService extends BaseApiService<Member> {
  override readonly apiUrl = `${environment.apiUrl}/api/identity/organizations`;

  archiveMember(memberId: string) {
    return this.http.put(`${this.apiUrl}/${memberId}/archive`, {});
  }

  getMembers(
    organizationId: string,
    paginationOptions: PaginationOptions<Member>,
  ): Observable<PaginatedResponse<Member>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Member>>(`${this.apiUrl}/${organizationId}/members`, {
      params: { ...options },
    });
  }

  searchMembers(
    organizationId: string,
    query: Record<any, any>,
    paginationOptions: PaginationOptions<Member>,
  ): Observable<PaginatedResponse<Member>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Member>>(
      `${this.apiUrl}/${organizationId}/members/search`,
      {
        params: { ...query, ...options },
      },
    );
  }
}
