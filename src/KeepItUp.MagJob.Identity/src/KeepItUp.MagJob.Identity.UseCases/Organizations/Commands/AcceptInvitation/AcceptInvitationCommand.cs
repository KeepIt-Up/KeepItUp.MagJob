using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.AcceptInvitation;

/// <summary>
/// Komenda do akceptacji zaproszenia do organizacji.
/// </summary>
public record AcceptInvitationCommand : IRequest<Result>
{
    /// <summary>
    /// Identyfikator zaproszenia.
    /// </summary>
    public Guid InvitationId { get; init; }

    /// <summary>
    /// Token zaproszenia.
    /// </summary>
    public string Token { get; init; } = string.Empty;

    /// <summary>
    /// Identyfikator użytkownika akceptującego zaproszenie.
    /// </summary>
    public Guid UserId { get; init; }
} 
