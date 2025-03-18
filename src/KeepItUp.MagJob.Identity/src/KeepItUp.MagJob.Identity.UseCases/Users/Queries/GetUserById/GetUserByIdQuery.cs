using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;

/// <summary>
/// Zapytanie o użytkownika na podstawie identyfikatora.
/// </summary>
public record GetUserByIdQuery : IRequest<Result<UserDto>>
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid Id { get; init; }
}
