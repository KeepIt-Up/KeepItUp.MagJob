using Organizations.Application.Features.Members.Get;

namespace Organizations.Application.Features.Members.Update;

public sealed record UpdateMemberRequest(
    Guid Id,
    string? FirstName,
    string? LastName,
    string? Notes
) : IRequest<GetMemberResponse>;