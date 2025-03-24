using Ardalis.Result;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AssignRoleToMember;

/// <summary>
/// Komenda do przypisania roli członkowi organizacji.
/// </summary>
public record AssignRoleToMemberCommand : IRequest<Result>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika, któremu ma zostać przypisana rola.
    /// </summary>
    public Guid MemberUserId { get; init; }

    /// <summary>
    /// Identyfikator roli do przypisania.
    /// </summary>
    public Guid RoleId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego operację.
    /// </summary>
    public Guid RequestingUserId { get; init; }
} 
