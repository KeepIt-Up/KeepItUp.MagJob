using Ardalis.Result;
using MediatR;

namespace KeepItUp.MagJob.Identity.UseCases.Users.Commands.CreateUser;

/// <summary>
/// Komenda do tworzenia nowego użytkownika.
/// </summary>
public record CreateUserCommand : IRequest<Result<Guid>>
{
    /// <summary>
    /// Identyfikator użytkownika w systemie zewnętrznym (Keycloak).
    /// </summary>
    public string ExternalId { get; init; } = string.Empty;

    /// <summary>
    /// Adres e-mail użytkownika.
    /// </summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>
    /// Nazwa użytkownika.
    /// </summary>
    public string Username { get; init; } = string.Empty;

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
