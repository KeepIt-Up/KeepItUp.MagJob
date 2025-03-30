export interface CurrentOrganization {
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
}
