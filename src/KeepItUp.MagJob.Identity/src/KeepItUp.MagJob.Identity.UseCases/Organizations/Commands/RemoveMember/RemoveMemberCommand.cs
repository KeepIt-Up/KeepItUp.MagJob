using Ardalis.Result;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RemoveMember;

/// <summary>
/// Komenda do usunięcia członka z organizacji.
/// </summary>
public record RemoveMemberCommand : IRequest<Result>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika do usunięcia.
    /// </summary>
    public Guid MemberUserId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego operację.
    /// </summary>
    public Guid RequestingUserId { get; init; }
} 
