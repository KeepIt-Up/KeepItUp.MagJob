export interface TimeEntryMemberResponse {
  id: string;
  status: string;
  memberId: string;
  firstName?: string;
  lastName?: string;
  timeEntry: {
    id: string;
    startDateTime: string;
    endDateTime: string;
  };
}

export interface GetTimeEntryMembersResponse {
  timeEntryMemberList: TimeEntryMemberResponse[];
  count: number;
}
