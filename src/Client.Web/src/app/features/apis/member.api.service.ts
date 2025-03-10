import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Member } from '@features/models/member/member';
import { BaseApiService } from '@shared/services/base-api.service';
import { PaginationOptions } from '@shared/components/pagination/pagination.component';
import { PaginatedResponse } from '@shared/components/pagination/pagination.component';
import { serializePaginationOptions } from '@shared/components/pagination/pagination.component';
import { environment } from '@environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MemberApiService extends BaseApiService<Member> {

  override readonly apiUrl = '/api/members';

  archiveMember(memberId: string) {
    return this.http.put(`${this.apiUrl}/${memberId}/archive`, {});
  }

  getMembers(query: Record<any, any>, paginationOptions: PaginationOptions<Member>): Observable<PaginatedResponse<Member>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Member>>(`/api/organizations/members`, { params: { ...query, ...options } });
  }

  searchMembers(query: Record<any, any>, paginationOptions: PaginationOptions<Member>): Observable<PaginatedResponse<Member>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Member>>(`/api/members/search`, { params: { ...query, ...options } });
  }
}
