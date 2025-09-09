import { Role } from '../../roles/models/role.model';

export interface Member {
  id: string;
  userId: string;
  fullName: string;
  firstName: string;
  lastName: string;
  displayName: string;
  archived: boolean;
  organizationId: string;
  roles: Role[];
}
