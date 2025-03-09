import { Role } from "../role/role";

export interface Member {
    id: string;
    fullName: string;
    firstName: string;
    lastName: string;
    archived: boolean;
    organizationId: string;
    roles: Role[];
}