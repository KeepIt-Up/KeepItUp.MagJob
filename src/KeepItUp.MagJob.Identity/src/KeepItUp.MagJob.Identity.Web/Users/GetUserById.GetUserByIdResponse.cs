﻿namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Odpowiedź dla endpointu GetUserEndpoint.
/// </summary>
public class GetUserByIdResponse
{
    /// <summary>
    /// Identyfikator użytkownika.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Zewnętrzny identyfikator użytkownika (np. z Keycloak).
    /// </summary>
    public Guid ExternalId { get; set; }

    /// <summary>
    /// Adres email użytkownika.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Imię użytkownika.
    /// </summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// Nazwisko użytkownika.
    /// </summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// Czy użytkownik jest aktywny.
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// URL do zdjęcia profilowego użytkownika.
    /// </summary>
    public string? ProfileImageUrl { get; set; }
}
