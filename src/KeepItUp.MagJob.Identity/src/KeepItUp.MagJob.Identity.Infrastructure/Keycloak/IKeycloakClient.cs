using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using KeepItUp.MagJob.Identity.Infrastructure.Keycloak.Models;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Interfejs klienta do komunikacji z API Keycloak
/// </summary>
public interface IKeycloakClient
{
    /// <summary>
    /// Pobiera użytkownika z Keycloak na podstawie identyfikatora
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Dane użytkownika lub null, jeśli użytkownik nie istnieje</returns>
    Task<KeycloakUser?> GetUserByIdAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Pobiera użytkownika z Keycloak na podstawie adresu email
    /// </summary>
    /// <param name="email">Adres email użytkownika</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Dane użytkownika lub null, jeśli użytkownik nie istnieje</returns>
    Task<KeycloakUser?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Pobiera listę użytkowników z Keycloak
    /// </summary>
    /// <param name="search">Opcjonalny parametr wyszukiwania</param>
    /// <param name="first">Indeks pierwszego elementu do pobrania</param>
    /// <param name="max">Maksymalna liczba elementów do pobrania</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Lista użytkowników</returns>
    Task<List<KeycloakUser>> GetUsersAsync(string? search = null, int first = 0, int max = 100, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Tworzy nowego użytkownika w Keycloak
    /// </summary>
    /// <param name="user">Dane użytkownika do utworzenia</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Identyfikator utworzonego użytkownika</returns>
    Task<string> CreateUserAsync(KeycloakUser user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Aktualizuje dane użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="user">Zaktualizowane dane użytkownika</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task UpdateUserAsync(string userId, KeycloakUser user, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Dezaktywuje użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task DeactivateUserAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Aktywuje użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task ActivateUserAsync(string userId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Aktualizuje atrybuty użytkownika w Keycloak
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika w Keycloak</param>
    /// <param name="attributes">Atrybuty do zaktualizowania</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Task reprezentujący asynchroniczną operację</returns>
    Task UpdateUserAttributesAsync(string userId, Dictionary<string, List<string>> attributes, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Pobiera token dostępu do API Keycloak
    /// </summary>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Token dostępu</returns>
    Task<string> GetAdminAccessTokenAsync(CancellationToken cancellationToken = default);
} 
