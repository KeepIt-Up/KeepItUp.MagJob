namespace Organizations.Application.Features.Members.Get;

public sealed record GetMemberRequest(Guid id) : IRequest<GetMemberResponse>;