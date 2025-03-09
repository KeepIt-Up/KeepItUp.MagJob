import { inject, Injectable, signal } from '@angular/core';
import { UserApiService } from '../apis/user.api';
import { StateService } from '@shared/services/state.service';
import { OAuthEvent, OAuthService } from 'angular-oauth2-oidc';
import { catchError, Observable, tap } from 'rxjs';
import { PaginatedResponse, PaginationOptions } from '@shared/components/pagination/pagination.component';
import { Organization } from '@features/models/organization/organization';
import { Invitation } from '@features/models/invitation/invitation';
import { ListStateService } from '@shared/services/list-state.service';
import { User } from '@features/models/user/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private userState = new StateService<User>();
  private invitationStateService = new ListStateService<Invitation, { endOfData: boolean }>();
  private organizationStateService = new ListStateService<Organization, { endOfData: boolean }>();

  private authService = inject(OAuthService);
  private api = inject(UserApiService);

  constructor() {
    if (this.authService.hasValidAccessToken()) {
      this.getUser().subscribe();
      return;
    }

    this.authService.events.subscribe((event: OAuthEvent) => {
      switch (event.type) {
        case 'token_received':
          this.getUser().subscribe();
      }
    });
  }

  userState$ = this.userState.state$;
  invitationState$ = this.invitationStateService.state$;
  organizationState$ = this.organizationStateService.state$;

  organizationsPaginationOptions$ = signal<PaginationOptions<Organization>>({
    pageNumber: 1,
    pageSize: 10,
    sortField: "id",
    ascending: true
  });

  invitationsPaginationOptions$ = signal<PaginationOptions<Invitation>>({
    pageNumber: 1,
    pageSize: 10,
    sortField: "id",
    ascending: true
  });

  getUserOrganizations(query: Record<any, any>): Observable<PaginatedResponse<Organization>> {
    return this.api.getUserOrganizations(query, this.organizationsPaginationOptions$()).pipe(
      tap((response: PaginatedResponse<Organization>) => {
        if (response.items.length > 0) {
          this.organizationStateService.add(response.items);
          this.organizationStateService.setMetadata({ endOfData: !response.hasNextPage });
        } else {
          this.organizationStateService.setError(new Error('You have no organizations'));
        }
      }),
      catchError((error) => {
        this.organizationStateService.setError(error);
        throw error;
      })
    );
  }

  getUserInvitations(query: Record<any, any>): Observable<PaginatedResponse<Invitation>> {
    return this.api.getUserInvitations(query, this.invitationsPaginationOptions$()).pipe(
      tap((response: PaginatedResponse<Invitation>) => {
        if (response.items.length > 0) {
          this.invitationStateService.add(response.items);
          this.invitationStateService.setMetadata({ endOfData: !response.hasNextPage });
        } else {
          this.invitationStateService.setError(new Error('You have no invitations'));
        }
      }),
      catchError((error) => {
        this.invitationStateService.setError(error);
        throw error;
      })
    );
  }


  getUser() {
    return this.api.getUser().pipe(
      tap((user: User) => {
        this.userState.setData(user);
      }),
      catchError((error) => {
        window.alert("We can recognize you. Please contact with our support or try again");
        //this.authService.logOut();
        return error;
      })
    )
  }

}
