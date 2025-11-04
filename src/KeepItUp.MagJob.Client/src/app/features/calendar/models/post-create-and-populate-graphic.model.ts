export interface PostCreateAndPopulateGraphic {
  availabilityTemplateId: string;
  startDate: string;
  name: string;
  description?: string;
  managerId: string;
  organizationId: string;
}

export interface CreateGraphicResponse {
  id: string;
  name: string;
  description?: string;
  startDate: string;
  availabilityTemplateId: string;
  timeEntries: {
    id: string;
    startDateTime: string;
    endDateTime: string;
  }[];
}
