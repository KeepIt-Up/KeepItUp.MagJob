import { inject, Injectable, signal } from '@angular/core';
import { UserApiService } from './user.api';
import { catchError, Observable, of, tap } from 'rxjs';
import {
  PaginatedResponse,
  PaginationOptions,
} from '@shared/components/pagination/pagination.component';
import { ListStateService } from '@shared/services/list-state.service';
import { HttpErrorResponse } from '@angular/common/http';
import { Invitation } from '../../invitations/models/invitation';
import { Organization } from '../../organizations/models/organization.model';
import { HttpClient } from '@angular/common/http';
import { environment } from '@environments/environment';
import { CurrentUser } from '@users/models/current-user.model';
import { throwError } from 'rxjs';
import { UserContextService } from '@users/services/user-context.service';

interface UpdateUserRequest {
  id: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  address?: string;
}

@Injectable({
  providedIn: 'root',
})
export class UserService {
  userContext: CurrentUser | null = null;
  private userContextService = inject(UserContextService);
  private invitationStateService = new ListStateService<Invitation, { endOfData: boolean }>();
  private organizationStateService = new ListStateService<Organization, { endOfData: boolean }>();

  private api = inject(UserApiService);
  private http = inject(HttpClient);

  constructor() {
    this.userContextService.userContext$.subscribe(userContext => {
      if (userContext.data) {
        this.userContext = userContext.data;
      }
    });
  }

  private readonly apiUrl = `${environment.apiUrl}/api/identity/users`;

  invitationState$ = this.invitationStateService.state$;
  organizationState$ = this.organizationStateService.state$;

  organizationsPaginationOptions$ = signal<PaginationOptions<Organization>>({
    pageNumber: 1,
    pageSize: 10,
    sortField: 'id',
    ascending: true,
  });

  invitationsPaginationOptions$ = signal<PaginationOptions<Invitation>>({
    pageNumber: 1,
    pageSize: 10,
    sortField: 'id',
    ascending: true,
  });

  getUserOrganizations(): Observable<PaginatedResponse<Organization>> {
    return this.api
      .getUserOrganizations(this.userContext!.id, this.organizationsPaginationOptions$())
      .pipe(
        tap((response: PaginatedResponse<Organization>) => {
          if (response.items.length > 0) {
            this.organizationStateService.add(response.items);
            this.organizationStateService.setMetadata({ endOfData: !response.hasNextPage });
          } else {
            this.organizationStateService.setError(new Error('You have no organizations'));
          }
        }),
        catchError(error => {
          this.organizationStateService.setError(error as HttpErrorResponse);
          throw error;
        }),
      );
  }

  getUserInvitations(): Observable<PaginatedResponse<Invitation>> {
    const query = {
      id: this.userContext?.id,
    };
    return this.api.getUserInvitations(query, this.invitationsPaginationOptions$()).pipe(
      tap((response: PaginatedResponse<Invitation>) => {
        if (response.items.length > 0) {
          this.invitationStateService.add(response.items);
          this.invitationStateService.setMetadata({ endOfData: !response.hasNextPage });
        } else {
          this.invitationStateService.setError(new Error('You have no invitations'));
        }
      }),
      catchError(error => {
        this.invitationStateService.setError(error as HttpErrorResponse);
        throw error;
      }),
    );
  }

  /**
   * Updates user information
   */
  updateUser(request: UpdateUserRequest): Observable<CurrentUser> {
    return this.http
      .put<CurrentUser>(`${this.apiUrl}/${request.id}`, request)
      .pipe(catchError((error: HttpErrorResponse) => throwError(() => new Error(error.message))));
  }

  /**
   * Updates user profile picture
   */
  updateProfilePicture(
    userId: string,
    fileData: FormData,
  ): Observable<{ profileImageUrl?: string }> {
    return this.http
      .put<{ profileImageUrl?: string }>(`${this.apiUrl}/${userId}/profile-picture`, fileData)
      .pipe(catchError((error: HttpErrorResponse) => throwError(() => new Error(error.message))));
  }
}
