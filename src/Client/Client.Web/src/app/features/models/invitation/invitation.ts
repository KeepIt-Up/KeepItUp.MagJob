export interface Invitation {
    id: string;
    userName: string;
    createdAt: Date;
    status: InvitationStatus;
    organizationName: string;
    organizationId: string;
    userId: string;
}

export enum InvitationStatus {
    PENDING = 0,
    ACCEPTED = 1,
    REJECTED = 2,
    CANCELLED = 3
}