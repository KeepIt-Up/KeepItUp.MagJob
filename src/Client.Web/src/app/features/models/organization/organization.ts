import { Invitation } from "../invitation/invitation";
import { Member } from "../member/member";

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
    profileImage?: string;
    bannerImage?: string;
    invitations: Invitation[];
    members: Member[];
}