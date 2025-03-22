using Ardalis.Result;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.CreateInvitation;

/// <summary>
/// Komenda do tworzenia zaproszenia do organizacji.
/// </summary>
public record CreateInvitationCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Identyfikator organizacji.
    /// </summary>
    public Guid OrganizationId { get; init; }

    /// <summary>
    /// Adres e-mail osoby zapraszanej.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Identyfikator roli, która zostanie przypisana po akceptacji zaproszenia.
    /// </summary>
    public Guid RoleId { get; init; }

    /// <summary>
    /// Identyfikator użytkownika wykonującego operację.
    /// </summary>
    public Guid UserId { get; init; }
} 
