import { inject, Injectable, signal } from '@angular/core';
import { catchError, EMPTY, Observable, tap } from 'rxjs';
import { NotificationService } from '@shared/services/notification.service';
import { MemberApiService } from '@features/apis/member.api.service';
import { Member } from '@features/models/member/member';
import { PaginatedResponse, PaginationOptions } from '@shared/components/pagination/pagination.component';
import { ListStateService } from '@shared/services/list-state.service';
import { StateService } from '@shared/services/state.service';

@Injectable({
  providedIn: 'root'
})
export class MemberService {

  private api = inject(MemberApiService);
  private notificationService = inject(NotificationService);

  private memberStateService = new StateService<PaginatedResponse<Member>>();
  private memberSearchStateService = new ListStateService<Member, { endOfData: boolean }>();

  membersState$ = this.memberStateService.state$;
  memberSearchState$ = this.memberSearchStateService.state$;

  membersPaginationOptions$ = signal<PaginationOptions<Member>>({
    pageNumber: 1,
    pageSize: 10,
    sortField: "id",
    ascending: true
  });

  memberSearchPaginationOptions$ = signal<PaginationOptions<Member>>({
    pageNumber: 1,
    pageSize: 10,
    sortField: "id",
    ascending: true
  });

  archiveMember(memberId: string) {
    return this.api.archiveMember(memberId).pipe(
      tap(() => {
        var data = this.memberStateService.state$().data;
        var index = data?.items?.findIndex((m: Member) => m.id === memberId);
        if (index !== undefined) {
          data!.items[index].archived = true;
        }
        this.memberStateService.setData(data!);
        this.notificationService.show('Member archived successfully', 'success');
      }),
      catchError(() => {
        this.notificationService.show('Failed to archive member', 'error');
        return EMPTY;
      })
    );
  }

  updateMember(memberId: string, member: Partial<Member>) {
    return this.api.update(memberId, member).pipe(
      tap((member: Member) => {
        var data = this.memberStateService.state$().data;
        var index = data?.items?.findIndex((m: Member) => m.id === member.id);
        if (index !== undefined) {
          data!.items[index] = member;
        }
        this.memberStateService.setData(data!);
        this.notificationService.show('Member updated successfully', 'success');
      }),
      catchError(() => {
        this.notificationService.show('Failed to update member', 'error');
        return EMPTY;
      })
    );
  }

  getMembersByOrganizationId(organizationId: string): Observable<any> {
    const query = { Id: organizationId };
    return this.api.getMembers(query, this.membersPaginationOptions$()).pipe(
      tap((response: PaginatedResponse<Member>) => {
        this.memberStateService.setData(response);
      }),
    );
  }

  searchMembers(name: string, organizationId: string) {
    const query = { organizationId, name };
    return this.api.searchMembers(query, this.memberSearchPaginationOptions$()).pipe(
      tap((response: PaginatedResponse<Member>) => {
        this.memberSearchStateService.setData(response.items);
        this.memberSearchStateService.setMetadata({ endOfData: response.hasNextPage });
      }),
      catchError(() => {
        this.notificationService.show('Failed to search members', 'error');
        return EMPTY;
      })
    );
  }
}
