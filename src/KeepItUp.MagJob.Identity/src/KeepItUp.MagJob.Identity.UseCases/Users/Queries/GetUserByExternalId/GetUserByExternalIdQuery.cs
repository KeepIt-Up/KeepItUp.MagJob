using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserByExternalId;

/// <summary>
/// Zapytanie o użytkownika na podstawie identyfikatora zewnętrznego.
/// </summary>
public record GetUserByExternalIdQuery : IRequest<Result<UserDto>>
{
    /// <summary>
    /// Identyfikator zewnętrzny użytkownika.
    /// </summary>
    public Guid ExternalId { get; init; }
}
