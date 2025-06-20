import { inject, Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { Organization } from "../models/organization.model";
import {
  PaginatedResponse,
  PaginationOptions,
  serializePaginationOptions,
} from "@shared/components/pagination/pagination.component";
import { Invitation } from "../../invitations/models/invitation";
import { environment } from "@environments/environment";
import { HttpClient } from "@angular/common/http";

export interface CreateOrganizationPayload {
  name: string;
  description?: string;
}

export interface UpdateOrganizationPayload {
  name?: string;
  description?: string;
  isActive?: boolean;
}

@Injectable({
  providedIn: "root",
})
export class OrganizationApiService {
  readonly apiUrl = `${environment.apiUrl}/api/identity/Organizations`;
  private readonly http = inject(HttpClient);

  /**
   * Updates logo for an organization using FormData
   */
  updateLogo(
    organizationId: string,
    logoFile: File
  ): Observable<{ logoUrl: string }> {
    const formData = new FormData();
    formData.append("logoFile", logoFile);

    return this.http.put<{ logoUrl: string }>(
      `${this.apiUrl}/${organizationId}/Logo`,
      formData
    );
  }

  /**
   * Updates banner for an organization using FormData
   */
  updateBanner(
    organizationId: string,
    bannerFile: File
  ): Observable<{ bannerUrl: string }> {
    const formData = new FormData();
    formData.append("bannerFile", bannerFile);

    return this.http.put<{ bannerUrl: string }>(
      `${this.apiUrl}/${organizationId}/Banner`,
      formData
    );
  }

  getInvitations(
    organizationId: string,
    query: Record<any, any>,
    paginationOptions: PaginationOptions<Invitation>
  ): Observable<PaginatedResponse<Invitation>> {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Invitation>>(
      `${this.apiUrl}/${organizationId}/invitations`,
      {
        params: { ...query, ...options },
      }
    );
  }

  archiveMember(organizationId: string, memberId: string) {
    return this.http.put(
      `${this.apiUrl}/${organizationId}/members/${memberId}/archive`,
      {}
    );
  }

  getAll(
    query: Record<any, any>,
    paginationOptions: PaginationOptions<Organization>
  ) {
    const options = serializePaginationOptions(paginationOptions);
    return this.http.get<PaginatedResponse<Organization>>(`${this.apiUrl}`, {
      params: { ...query, ...options },
    });
  }

  get(id: string) {
    return this.http.get<Organization>(`${this.apiUrl}/${id}`);
  }

  create(payload: CreateOrganizationPayload) {
    return this.http.post<Organization>(this.apiUrl, payload);
  }

  update(id: string, payload: Partial<Organization>) {
    return this.http.put<Organization>(`${this.apiUrl}/${id}`, payload);
  }

  delete(id: string) {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
