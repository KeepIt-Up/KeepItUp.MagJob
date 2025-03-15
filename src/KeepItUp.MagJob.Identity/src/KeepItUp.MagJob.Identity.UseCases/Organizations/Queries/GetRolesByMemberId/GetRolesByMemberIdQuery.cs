using Ardalis.Result;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetRolesByMemberId;

/// <summary>
/// Zapytanie o role przypisane do członka organizacji.
/// </summary>
public record GetRolesByMemberIdQuery : IRequest<Result<List<RoleDto>>>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika członka, którego role chcemy pobrać.
    /// </summary>
    public Guid MemberUserId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego zapytanie.
    /// </summary>
    public Guid RequestingUserId { get; init; }
} 
