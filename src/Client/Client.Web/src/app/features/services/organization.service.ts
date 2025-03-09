import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { NotificationService } from '@shared/services/notification.service';
import { StateService } from '@shared/services/state.service';
import { PaginatedResponse, PaginationOptions } from '@shared/components/pagination/pagination.component';
import { Invitation } from '@features/models/invitation/invitation';
import { CreateOrganizationPayload, OrganizationApiService, UpdateOrganizationPayload } from '@features/apis/organization.api.service';
import { Organization } from '@features/models/organization/organization';

@Injectable({
  providedIn: 'root',
})
export class OrganizationService {

  private stateService = new StateService<Organization>();
  private invitationStateService = new StateService<PaginatedResponse<Invitation>>();


  private apiService = inject(OrganizationApiService);
  private notificationService = inject(NotificationService);

  state$ = this.stateService.state$;
  $organization = computed(() => this.stateService.state$().data);
  invitationsState$ = this.invitationStateService.state$;


  invitationsPaginationOptions$ = signal<PaginationOptions<Invitation>>({
    pageNumber: 1,
    pageSize: 10,
    sortField: "id",
    ascending: true
  });

  getOrganization(organizationId: string) {
    return this.apiService.get(organizationId).pipe(
      tap((organization) => {
        this.stateService.setData(organization);
      }),
      catchError((error) => {
        this.stateService.setError(error);
        return throwError(() => error);
      })
    );
  }

  updateOrganization(organizationId: string, payload: UpdateOrganizationPayload): Observable<any> {
    this.stateService.setLoading(true);
    return this.apiService.update(organizationId, payload).pipe(
      tap((organization) => {
        this.stateService.setData(organization);
        this.notificationService.show('Organization updated successfully', 'success');
      }),
      catchError((error) => {
        this.stateService.setError(error);
        this.notificationService.show('Failed to update organization', 'error');
        return throwError(() => error);
      })
    );
  }

  createOrganization(payload: CreateOrganizationPayload): Observable<any> {
    return this.apiService.create(payload).pipe(
      tap((organization) => {
        this.stateService.setData(organization);
        this.notificationService.show('Organization created successfully', 'success');
      }),
      catchError((error) => {
        this.stateService.setError(error);
        this.notificationService.show('Failed to create organization', 'error');
        return throwError(() => error);
      })
    );
  }

  getInvitations(): Observable<any> {
    const query = { id: this.$organization()?.id };
    return this.apiService.getInvitations(query, this.invitationsPaginationOptions$()).pipe(
      tap((response: PaginatedResponse<Invitation>) => {
        if (response.items.length === 0) {
          throw new Error('No invitations found');
        }
        this.invitationStateService.setData(response);
      }),
      catchError((error) => {
        console.log(error);
        this.invitationStateService.setError(error);
        throw error;
      })
    );
  }
}
