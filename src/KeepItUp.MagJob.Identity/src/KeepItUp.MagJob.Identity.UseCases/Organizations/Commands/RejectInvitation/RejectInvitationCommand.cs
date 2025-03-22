using Ardalis.Result;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.RejectInvitation;

/// <summary>
/// Komenda do odrzucenia zaproszenia do organizacji.
/// </summary>
public record RejectInvitationCommand : IRequest<Result>
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
    /// Identyfikator użytkownika odrzucającego zaproszenie.
    /// </summary>
    public Guid UserId { get; init; }
} 
