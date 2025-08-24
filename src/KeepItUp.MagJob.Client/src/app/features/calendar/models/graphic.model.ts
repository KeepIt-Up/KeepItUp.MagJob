export interface GraphicResponse {
  id: string;
  name: string;
  managerId: string;
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
  userId: string;
  userName?: string;
  userEmail?: string;
  timeEntryId: string;
}

export interface GetGraphicsResponse {
  graphicsResponse: GraphicResponse[]; // Changed from graphicResponseList to graphicsResponse
  count: number;
  graphicResponseList?: GraphicResponse[];
}
