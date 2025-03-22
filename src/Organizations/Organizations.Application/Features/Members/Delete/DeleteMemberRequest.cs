namespace Organizations.Application.Features.Members.Delete;

public sealed record DeleteMemberRequest(Guid id) : IRequest<DeleteMemberResponse>;