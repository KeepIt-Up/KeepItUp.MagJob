import { inject } from '@angular/core';
import { OrganizationContextService } from '../services/organization-context.service';

/**
 * Hook to access the current organization context
 * This helps access the organization context from anywhere in the application
 */
export function useOrganization() {
  const organizationContextService = inject(OrganizationContextService);

  return {
    organization$: organizationContextService.organization$,
    loading$: organizationContextService.loading$,
    error$: organizationContextService.error$,
    getCurrentOrganization: () => organizationContextService.getCurrentOrganization(),
  };
}
