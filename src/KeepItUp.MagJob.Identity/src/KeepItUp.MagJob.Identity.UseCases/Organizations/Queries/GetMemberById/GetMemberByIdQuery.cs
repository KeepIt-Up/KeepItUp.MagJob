using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetMemberById;

/// <summary>
/// Query to get a member of an organization by their identifier.
/// </summary>
public record GetMemberByIdQuery : IRequest<Result<MemberDto>>
{
    /// <summary>
    /// Organization identifier.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// User identifier of the member we want to get.
    /// </summary>
    public Guid MemberUserId { get; init; }

    /// <summary>
    /// User identifier performing the query.
    /// </summary>
    public Guid RequestingUserId { get; init; }
}
