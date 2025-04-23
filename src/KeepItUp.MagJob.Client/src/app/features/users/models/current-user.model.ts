export interface CurrentUser {
  id: string;
  externalId: string;
  email: string;
  firstName: string;
  lastName: string;
  profileImageUrl: string | null;
  phoneNumber?: string;
  address?: string;
}
