export interface GraphicResponse {
  id: string;
  name: string;
  managerId: string;
  organizationId: string;
  timeEntryMembers: TimeEntryMemberResponse[];
  timeEntries: TimeEntryResponse[];
}

export interface TimeEntryResponse {
  id: string;
  startDateTime: string;
  endDateTime: string;
  graphicId?: string;
}

export interface TimeEntryMemberResponse {
  id: string;
  memberId: string;
  status: string;
  timeEntry: {
    id: string;
    startDateTime: string;
    endDateTime: string;
  };
}

export interface GetGraphicsResponse {
  graphicsResponse: GraphicResponse[]; // Changed from graphicResponseList to graphicsResponse
  count: number;
  graphicResponseList?: GraphicResponse[];
}

export interface TimeEntryMemberAssignment {
  memberId: string;
  status: string;
}

export interface CreateTimeEntryMembersBulkRequest {
  timeEntryId: string;
  memberAssignments: TimeEntryMemberAssignment[];
}

export interface PatchTimeEntryMemberRequest {
  status: string;
}
