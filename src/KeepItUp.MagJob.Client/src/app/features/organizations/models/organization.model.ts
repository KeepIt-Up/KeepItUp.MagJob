import { Invitation } from '../../invitations/models/invitation';
import { Member } from '../../members/models/member';

export interface Organization {
  id: string;
  name: string;
  description?: string;
  logoUrl?: string;
  websiteUrl?: string;
  location?: string;
  archived: boolean;
  ownerId: string;
  createdAt: Date;
  updatedAt: Date;
  bannerUrl?: string;
  userRoles?: string[];
  isActive: boolean;
  invitations: Invitation[];
  members: Member[];
}
