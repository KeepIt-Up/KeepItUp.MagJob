using Organizations.Application.Features.Roles.Get;

namespace Organizations.Application.Features.Members.Get;

public sealed record GetMemberResponse(
    Guid Id,
    string FirstName,
    string LastName,
    List<GetRoleResponse> Roles
);