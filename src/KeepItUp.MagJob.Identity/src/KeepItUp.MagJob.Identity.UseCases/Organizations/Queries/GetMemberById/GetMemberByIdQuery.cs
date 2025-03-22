using Ardalis.Result;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetMemberById;

/// <summary>
/// Zapytanie o członka organizacji na podstawie jego identyfikatora.
/// </summary>
public record GetMemberByIdQuery : IRequest<Result<MemberDto>>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika członka, którego chcemy pobrać.
    /// </summary>
    public Guid MemberUserId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego zapytanie.
    /// </summary>
    public Guid RequestingUserId { get; init; }
} 
