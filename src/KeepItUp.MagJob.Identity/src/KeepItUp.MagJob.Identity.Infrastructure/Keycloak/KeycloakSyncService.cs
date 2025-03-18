using Ardalis.Specification;
using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate.Repositories;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate.Repositories;
using KeepItUp.MagJob.Identity.Infrastructure.Data;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Implementacja serwisu do synchronizacji danych między modułem Identity a Keycloak
/// </summary>
public class KeycloakSyncService : IKeycloakSyncService
{
  private readonly IKeycloakClient _keycloakClient;
  private readonly IUserRepository _userRepository;
  private readonly IOrganizationRepository _organizationRepository;
  private readonly ILogger<KeycloakSyncService> _logger;

  /// <summary>
  /// Inicjalizuje nową instancję klasy <see cref="KeycloakSyncService"/>
  /// </summary>
  /// <param name="keycloakClient">Klient Keycloak</param>
  /// <param name="userRepository">Repozytorium użytkowników</param>
  /// <param name="organizationRepository">Repozytorium organizacji</param>
  /// <param name="logger">Logger</param>
  public KeycloakSyncService(
      IKeycloakClient keycloakClient,
      IUserRepository userRepository,
      IOrganizationRepository organizationRepository,
      ILogger<KeycloakSyncService> logger)
  {
    _keycloakClient = keycloakClient ?? throw new ArgumentNullException(nameof(keycloakClient));
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  /// <inheritdoc />
  public async Task SyncUserRolesAsync(string userId, CancellationToken cancellationToken = default)
  {
    try
    {
      _logger.LogInformation("Rozpoczęto synchronizację ról użytkownika {UserId} z Keycloak", userId);

      // Pobierz użytkownika z naszej bazy danych
      var user = await _userRepository.GetByExternalIdAsync(userId, cancellationToken);
      if (user == null)
      {
        _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze zewnętrznym {ExternalId} podczas synchronizacji ról", userId);
        return;
      }

      // Pobierz role użytkownika z Keycloak
      var keycloakRoles = await _keycloakClient.GetUserRolesAsync(userId, cancellationToken);

      // Mapuj role Keycloak na uprawnienia w naszej aplikacji
      var permissions = MapRolesToPermissions(keycloakRoles);

      // Aktualizuj uprawnienia użytkownika
      user.UpdatePermissions(permissions);
      await _userRepository.UpdateAsync(user, cancellationToken);

      // Aktualizuj atrybuty użytkownika w Keycloak
      var keycloakUser = await _keycloakClient.GetUserByIdAsync(userId, cancellationToken);
      if (keycloakUser != null)
      {
        // Dodaj informacje o organizacjach użytkownika jako atrybuty
        var organizations = await _organizationRepository.GetByUserIdAsync(user.Id, cancellationToken);
        var organizationIds = organizations.Select(o => o.Id.ToString()).ToList();

        if (keycloakUser.Attributes == null)
        {
          keycloakUser.Attributes = new Dictionary<string, List<string>>();
        }

        keycloakUser.Attributes["organizations"] = organizationIds;
        keycloakUser.Attributes["permissions"] = permissions;

        await _keycloakClient.UpdateUserAsync(keycloakUser, cancellationToken);
      }

      _logger.LogInformation("Zakończono synchronizację ról użytkownika {UserId} z Keycloak", userId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas synchronizacji ról użytkownika {UserId} z Keycloak", userId);
      throw;
    }
  }

  /// <inheritdoc />
  public async Task SyncUserDataAsync(string userId, CancellationToken cancellationToken = default)
  {
    try
    {
      _logger.LogInformation("Rozpoczęto synchronizację danych użytkownika {UserId} z Keycloak", userId);

      // Pobierz dane użytkownika z Keycloak
      var keycloakUser = await _keycloakClient.GetUserByIdAsync(userId, cancellationToken);
      if (keycloakUser == null)
      {
        _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze {UserId} w Keycloak", userId);
        return;
      }

      // Sprawdź, czy użytkownik już istnieje w naszej bazie danych
      var existingUser = await _userRepository.GetByExternalIdAsync(userId, cancellationToken);

      if (existingUser == null)
      {
        // Utwórz nowego użytkownika
        var newUser = User.Create(
            keycloakUser.FirstName ?? string.Empty,
            keycloakUser.LastName ?? string.Empty,
            keycloakUser.Email,
            keycloakUser.Username ?? keycloakUser.Email,
            userId,
            true
        );

        await _userRepository.AddAsync(newUser, cancellationToken);
        _logger.LogInformation("Utworzono nowego użytkownika {UserId} na podstawie danych z Keycloak", newUser.Id);
      }
      else
      {
        // Aktualizuj istniejącego użytkownika
        existingUser.UpdateAllDetails(
            keycloakUser.FirstName ?? string.Empty,
            keycloakUser.LastName ?? string.Empty,
            keycloakUser.Email,
            keycloakUser.Username,
            keycloakUser.Enabled
        );

        await _userRepository.UpdateAsync(existingUser, cancellationToken);
        _logger.LogInformation("Zaktualizowano dane użytkownika {UserId} na podstawie danych z Keycloak", existingUser.Id);
      }

      _logger.LogInformation("Zakończono synchronizację danych użytkownika {UserId} z Keycloak", userId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas synchronizacji danych użytkownika {UserId} z Keycloak", userId);
      throw;
    }
  }

  /// <inheritdoc />
  public async Task SyncAllUsersAsync(CancellationToken cancellationToken = default)
  {
    try
    {
      _logger.LogInformation("Rozpoczęto synchronizację wszystkich użytkowników z Keycloak");

      // Pobierz wszystkich użytkowników z Keycloak
      var keycloakUsers = await _keycloakClient.GetAllUsersAsync(cancellationToken);

      foreach (var keycloakUser in keycloakUsers)
      {
        try
        {
          await SyncUserDataAsync(keycloakUser.Id, cancellationToken);
          await SyncUserRolesAsync(keycloakUser.Id, cancellationToken);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Wystąpił błąd podczas synchronizacji użytkownika {UserId} z Keycloak", keycloakUser.Id);
          // Kontynuuj synchronizację pozostałych użytkowników
        }
      }

      _logger.LogInformation("Zakończono synchronizację wszystkich użytkowników z Keycloak");
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas synchronizacji wszystkich użytkowników z Keycloak");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task<Guid> ImportUserFromKeycloakAsync(string keycloakUserId, CancellationToken cancellationToken = default)
  {
    try
    {
      // Pobierz dane użytkownika z Keycloak
      var keycloakUser = await _keycloakClient.GetUserByIdAsync(keycloakUserId, cancellationToken);
      if (keycloakUser == null)
      {
        throw new InvalidOperationException($"Nie znaleziono użytkownika o identyfikatorze {keycloakUserId} w Keycloak");
      }

      // Sprawdź, czy użytkownik już istnieje w module Identity
      var existingUser = await _userRepository.GetByExternalIdAsync(keycloakUserId, cancellationToken);

      if (existingUser != null)
      {
        _logger.LogInformation("Użytkownik o identyfikatorze {ExternalId} już istnieje w module Identity", keycloakUserId);
        return existingUser.Id;
      }

      // Utwórz nowego użytkownika w module Identity
      var newUser = User.Create(
          keycloakUser.FirstName ?? string.Empty,
          keycloakUser.LastName ?? string.Empty,
          keycloakUser.Email,
          keycloakUser.Username ?? keycloakUser.Email,
          keycloakUserId,
          true
      );

      await _userRepository.AddAsync(newUser, cancellationToken);

      _logger.LogInformation("Zaimportowano użytkownika {ExternalId} z Keycloak do modułu Identity", keycloakUserId);

      return newUser.Id;
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas importowania użytkownika {ExternalId} z Keycloak", keycloakUserId);
      throw;
    }
  }

  /// <summary>
  /// Mapuje role Keycloak na uprawnienia w naszej aplikacji
  /// </summary>
  /// <param name="roles">Role z Keycloak</param>
  /// <returns>Lista uprawnień</returns>
  private List<string> MapRolesToPermissions(List<string> roles)
  {
    var permissions = new List<string>();

    // Mapowanie ról na uprawnienia
    // To jest przykładowa implementacja, którą należy dostosować do rzeczywistych potrzeb
    foreach (var role in roles)
    {
      switch (role)
      {
        case "admin":
          permissions.Add("users.view");
          permissions.Add("users.create");
          permissions.Add("users.edit");
          permissions.Add("users.delete");
          permissions.Add("organizations.view");
          permissions.Add("organizations.create");
          permissions.Add("organizations.edit");
          permissions.Add("organizations.delete");
          break;

        case "manager":
          permissions.Add("users.view");
          permissions.Add("organizations.view");
          permissions.Add("organizations.create");
          permissions.Add("organizations.edit");
          break;

        case "user":
          permissions.Add("users.view.self");
          permissions.Add("organizations.view");
          break;
      }
    }

    // Usuń duplikaty
    return permissions.Distinct().ToList();
  }
}

