using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationMembers;

/// <summary>
/// Zapytanie o członków organizacji.
/// </summary>
public record GetOrganizationMembersQuery : IRequest<Result<List<MemberDto>>>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego zapytanie.
    /// </summary>
    public Guid UserId { get; init; }
}
