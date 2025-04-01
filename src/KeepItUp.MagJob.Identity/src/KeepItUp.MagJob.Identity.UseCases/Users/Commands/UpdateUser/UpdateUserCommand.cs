using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;

/// <summary>
/// Komenda do aktualizacji istniejącego użytkownika.
/// </summary>
public record UpdateUserCommand : IRequest<Result>
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Imię użytkownika.
    /// </summary>
    public string FirstName { get; init; } = string.Empty;

    /// <summary>
    /// Nazwisko użytkownika.
    /// </summary>
    public string LastName { get; init; } = string.Empty;

    /// <summary>
    /// Numer telefonu użytkownika (opcjonalny).
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Adres użytkownika (opcjonalny).
    /// </summary>
    public string? Address { get; init; }

    /// <summary>
    /// URL do zdjęcia profilowego użytkownika (opcjonalny).
    /// </summary>
    public string? ProfileImageUrl { get; init; }
}
