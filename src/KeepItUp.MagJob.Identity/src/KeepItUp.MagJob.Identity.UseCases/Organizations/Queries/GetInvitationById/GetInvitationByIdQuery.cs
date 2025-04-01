using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetInvitationById;

/// <summary>
/// Zapytanie o zaproszenie na podstawie jego identyfikatora.
/// </summary>
public record GetInvitationByIdQuery : IRequest<Result<InvitationDto>>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Identyfikator zaproszenia.
    /// </summary>
    public Guid InvitationId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego zapytanie.
    /// </summary>
    public Guid UserId { get; init; }
}
