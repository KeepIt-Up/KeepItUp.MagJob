import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, Observable, tap, throwError } from 'rxjs';
import { NotificationService } from '@shared/services/notification.service';
import { StateService } from '@shared/services/state.service';
import {
  PaginatedResponse,
  PaginationOptions,
} from '@shared/components/pagination/pagination.component';
import { Organization } from '../models/organization.model';
import { Invitation } from '../../invitations/models/invitation';
import {
  CreateOrganizationPayload,
  OrganizationApiService,
  UpdateOrganizationPayload,
} from './organization.api.service';

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
    sortField: 'id',
    ascending: true,
  });

  getOrganization(organizationId: string) {
    return this.apiService.get(organizationId).pipe(
      tap(organization => {
        this.stateService.setData(organization);
      }),
      catchError(error => {
        this.stateService.setError(error);
        return throwError(() => error);
      }),
    );
  }

  updateOrganization(organizationId: string, payload: UpdateOrganizationPayload): Observable<any> {
    this.stateService.setLoading(true);
    return this.apiService.update(organizationId, payload).pipe(
      tap(organization => {
        this.stateService.setData(organization);
        this.notificationService.show('Organization updated successfully', 'success');
      }),
      catchError(error => {
        this.stateService.setError(error);
        this.notificationService.show('Failed to update organization', 'error');
        return throwError(() => error);
      }),
    );
  }

  /**
   * Update organization logo with file upload
   */
  updateLogo(organizationId: string, logoFile: File): Observable<{ logoUrl: string }> {
    this.stateService.setLoading(true);
    return this.apiService.updateLogo(organizationId, logoFile).pipe(
      tap(response => {
        // Get current organization and update its logo URL
        const currentOrg = this.$organization();
        if (currentOrg) {
          const updatedOrg = { ...currentOrg, logoUrl: response.logoUrl };
          this.stateService.setData(updatedOrg);
        }
        this.notificationService.show('Organization logo updated successfully', 'success');
      }),
      catchError(error => {
        this.stateService.setError(error);
        this.notificationService.show('Failed to update organization logo', 'error');
        return throwError(() => error);
      }),
    );
  }

  /**
   * Update organization banner with file upload
   */
  updateBanner(organizationId: string, bannerFile: File): Observable<{ bannerUrl: string }> {
    this.stateService.setLoading(true);
    return this.apiService.updateBanner(organizationId, bannerFile).pipe(
      tap(response => {
        // Get current organization and update its banner URL
        const currentOrg = this.$organization();
        if (currentOrg) {
          const updatedOrg = { ...currentOrg, bannerUrl: response.bannerUrl };
          this.stateService.setData(updatedOrg);
        }
        this.notificationService.show('Organization banner updated successfully', 'success');
      }),
      catchError(error => {
        this.stateService.setError(error);
        this.notificationService.show('Failed to update organization banner', 'error');
        return throwError(() => error);
      }),
    );
  }

  createOrganization(payload: CreateOrganizationPayload): Observable<any> {
    return this.apiService.create(payload).pipe(
      tap(organization => {
        this.stateService.setData(organization);
        this.notificationService.show('Organization created successfully', 'success');
      }),
      catchError(error => {
        this.stateService.setError(error);
        this.notificationService.show('Failed to create organization', 'error');
        return throwError(() => error);
      }),
    );
  }

  getInvitations(): Observable<any> {
    const query = { id: this.$organization()?.id };
    return this.apiService
      .getInvitations(this.$organization()?.id ?? '', query, this.invitationsPaginationOptions$())
      .pipe(
        tap((response: PaginatedResponse<Invitation>) => {
          if (response.items.length === 0) {
            throw new Error('No invitations found');
          }
          this.invitationStateService.setData(response);
        }),
        catchError(error => {
          console.log(error);
          this.invitationStateService.setError(error);
          throw error;
        }),
      );
  }
}
