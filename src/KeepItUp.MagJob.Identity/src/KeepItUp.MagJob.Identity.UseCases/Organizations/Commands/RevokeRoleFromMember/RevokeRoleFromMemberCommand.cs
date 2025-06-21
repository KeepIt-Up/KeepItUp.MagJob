using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RevokeRoleFromMember;

/// <summary>
/// Komenda do odebrania roli członkowi organizacji.
/// </summary>
public record RevokeRoleFromMemberCommand : IRequest<Result>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika, któremu ma zostać odebrana rola.
    /// </summary>
    public Guid MemberUserId { get; init; }

    /// <summary>
    /// Identyfikator roli do odebrania.
    /// </summary>
    public Guid RoleId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego operację.
    /// </summary>
    public Guid RequestingUserId { get; init; }
}
