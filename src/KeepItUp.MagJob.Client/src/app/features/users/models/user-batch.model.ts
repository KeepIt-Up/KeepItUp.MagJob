export interface UserBatchResponse {
  users: UserBatchItem[];
}

export interface UserBatchItem {
  id: string;
  externalId: string;
  email: string;
  firstName: string;
  lastName: string;
  isActive: boolean;
}

