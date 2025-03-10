using System.Text.Json;
using Ardalis.Specification;
using KeepItUp.MagJob.Identity.Core.OrganizationAggregate;
using KeepItUp.MagJob.Identity.Core.UserAggregate;
using KeepItUp.MagJob.Identity.Infrastructure.Keycloak.Models;

namespace KeepItUp.MagJob.Identity.Infrastructure.Keycloak;

/// <summary>
/// Implementacja serwisu do synchronizacji danych między modułem Identity a Keycloak
/// </summary>
public class KeycloakSyncService : IKeycloakSyncService
{
  private readonly IKeycloakClient _keycloakClient;
  private readonly IRepository<User> _userRepository;
  private readonly IRepository<Organization> _organizationRepository;
  private readonly ILogger<KeycloakSyncService> _logger;

  /// <summary>
  /// Inicjalizuje nową instancję klasy <see cref="KeycloakSyncService"/>
  /// </summary>
  /// <param name="keycloakClient">Klient Keycloak</param>
  /// <param name="userRepository">Repozytorium użytkowników</param>
  /// <param name="organizationRepository">Repozytorium organizacji</param>
  /// <param name="roleRepository">Repozytorium ról</param>
  /// <param name="logger">Logger</param>
  public KeycloakSyncService(
      IKeycloakClient keycloakClient,
      IRepository<User> userRepository,
      IRepository<Organization> organizationRepository,
      ILogger<KeycloakSyncService> logger)
  {
    _keycloakClient = keycloakClient ?? throw new ArgumentNullException(nameof(keycloakClient));
    _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    _organizationRepository = organizationRepository ?? throw new ArgumentNullException(nameof(organizationRepository));
    _logger = logger ?? throw new ArgumentNullException(nameof(logger));
  }

  /// <inheritdoc />
  public async Task SyncUserRolesAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    try
    {
      var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
      if (user == null)
      {
        _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze {UserId}", userId);
        return;
      }
      
      // Pobierz organizacje i role użytkownika
      var organizations = await GetUserOrganizationsWithRolesAsync(userId, cancellationToken);
      
      // Pobierz uprawnienia użytkownika w kontekście organizacji
      var permissionsByOrg = await GetUserPermissionsByOrgAsync(userId, organizations, cancellationToken);
      
      // Pobierz wszystkie uprawnienia użytkownika (dla kompatybilności wstecznej)
      var allPermissions = new HashSet<string>();
      foreach (var permissions in permissionsByOrg.Values)
      {
        foreach (var permission in permissions)
        {
          allPermissions.Add(permission);
        }
      }
      
      // Aktualizuj atrybuty użytkownika w Keycloak
      await _keycloakClient.UpdateUserAttributesAsync(user.ExternalId, new Dictionary<string, List<string>>
      {
        ["organizations"] = new List<string> { JsonSerializer.Serialize(organizations) },
        ["permissions"] = allPermissions.ToList(),
        ["permissions_by_org"] = new List<string> { JsonSerializer.Serialize(permissionsByOrg) }
      }, cancellationToken);
      
      _logger.LogInformation("Zsynchronizowano role i uprawnienia użytkownika {UserId} z Keycloak", userId);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Wystąpił błąd podczas synchronizacji ról użytkownika {UserId} z Keycloak", userId);
      throw;
    }
  }

  /// <inheritdoc />
  public async Task SyncUserDataAsync(Guid userId, CancellationToken cancellationToken = default)
  {
    try
    {
      var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
      if (user == null)
      {
        _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze {UserId}", userId);
        return;
      }

      // Pobierz dane użytkownika z Keycloak
      var keycloakUser = await _keycloakClient.GetUserByIdAsync(user.ExternalId, cancellationToken);
      if (keycloakUser == null)
      {
        _logger.LogWarning("Nie znaleziono użytkownika o identyfikatorze {ExternalId} w Keycloak", user.ExternalId);
        return;
      }

      // Aktualizuj dane użytkownika w module Identity
      user.UpdateAllDetails(
          keycloakUser.FirstName ?? string.Empty,
          keycloakUser.LastName ?? string.Empty,
          keycloakUser.Email,
          keycloakUser.Enabled
      );

      await _userRepository.UpdateAsync(user, cancellationToken);

      _logger.LogInformation("Zsynchronizowano dane użytkownika {UserId} z Keycloak", userId);
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
      // Pobierz wszystkich użytkowników z modułu Identity
      var users = await _userRepository.ListAsync(cancellationToken);

      foreach (var user in users)
      {
        try
        {
          await SyncUserDataAsync(user.Id, cancellationToken);
          await SyncUserRolesAsync(user.Id, cancellationToken);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Wystąpił błąd podczas synchronizacji użytkownika {UserId} z Keycloak", user.Id);
          // Kontynuuj synchronizację pozostałych użytkowników
        }
      }

      _logger.LogInformation("Zsynchronizowano wszystkich użytkowników z Keycloak");
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
      var existingUser = await _userRepository.FirstOrDefaultAsync(
          new UserByExternalIdSpecification(keycloakUserId),
          cancellationToken);

      if (existingUser != null)
      {
        _logger.LogInformation("Użytkownik o identyfikatorze {ExternalId} już istnieje w module Identity", keycloakUserId);
        return existingUser.Id;
      }

      // Utwórz nowego użytkownika w module Identity
      var newUser = User.Create(
          keycloakUserId,
          keycloakUser.Email,
          keycloakUser.FirstName ?? string.Empty,
          keycloakUser.LastName ?? string.Empty
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

  private async Task<List<KeycloakOrganization>> GetUserOrganizationsWithRolesAsync(Guid userId, CancellationToken cancellationToken)
  {
    var result = new List<KeycloakOrganization>();
    
    try
    {
        // Pobierz wszystkie organizacje z ich członkami i rolami dla konkretnego użytkownika w jednym zapytaniu
        var organizations = await _organizationRepository.ListAsync(
            new OrganizationsWithMembersForUserSpecification(userId),
            cancellationToken);
        
        if (organizations == null || !organizations.Any())
        {
            _logger.LogInformation("Nie znaleziono żadnych organizacji dla użytkownika {UserId}", userId);
            return result;
        }
        
        // Przetwórz organizacje, w których użytkownik jest członkiem
        foreach (var organization in organizations)
        {
            // Pobierz członkostwo użytkownika w organizacji
            var member = organization.Members.FirstOrDefault(m => m.UserId == userId);
            if (member == null)
            {
                // To nie powinno się zdarzyć, ponieważ specyfikacja już filtruje organizacje
                continue;
            }
            
            // Pobierz role użytkownika w organizacji
            var roleIds = member.RoleIds;
            
            // Pobierz nazwy ról
            var roleNames = new List<string>();
            foreach (var roleId in roleIds)
            {
                var role = organization.Roles.FirstOrDefault(r => r.Id == roleId);
                if (role != null)
                {
                    roleNames.Add(role.Name);
                }
            }
            
            // Dodaj organizację z rolami do wyniku
            result.Add(new KeycloakOrganization
            {
                Id = organization.Id.ToString(),
                Name = organization.Name,
                Roles = roleNames
            });
        }
        
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Wystąpił błąd podczas pobierania organizacji i ról użytkownika {UserId}", userId);
        throw;
    }
  }

  private async Task<Dictionary<string, List<string>>> GetUserPermissionsByOrgAsync(Guid userId, List<KeycloakOrganization> organizations, CancellationToken cancellationToken)
  {
    var result = new Dictionary<string, List<string>>();
    
    try
    {
        // Pobierz wszystkie organizacje z ich rolami w jednym zapytaniu
        var allOrganizations = await _organizationRepository.ListAsync(
            new OrganizationsWithRolesSpecification(),
            cancellationToken);
        
        if (allOrganizations == null || !allOrganizations.Any())
        {
            _logger.LogInformation("Nie znaleziono żadnych organizacji z rolami");
            return result;
        }
        
        foreach (var org in organizations)
        {
            var orgId = Guid.Parse(org.Id);
            var orgPermissions = new List<string>();
            
            // Znajdź organizację
            var organization = allOrganizations.FirstOrDefault(o => o.Id == orgId);
            if (organization == null)
            {
                _logger.LogWarning("Nie znaleziono organizacji o identyfikatorze {OrganizationId}", orgId);
                continue;
            }
            
            foreach (var roleName in org.Roles)
            {
                // Znajdź rolę w organizacji
                var role = organization.Roles.FirstOrDefault(r => r.Name == roleName);
                if (role == null)
                {
                    _logger.LogWarning("Nie znaleziono roli {RoleName} w organizacji {OrganizationId}", roleName, orgId);
                    continue;
                }
                
                // Dodaj uprawnienia roli do wyniku
                foreach (var permission in role.Permissions)
                {
                    orgPermissions.Add(permission.Name);
                }
            }
            
            // Dodaj uprawnienia organizacji do wyniku
            if (orgPermissions.Any())
            {
                result[org.Id] = orgPermissions.Distinct().ToList();
            }
        }
        
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Wystąpił błąd podczas pobierania uprawnień użytkownika {UserId} w kontekście organizacji", userId);
        throw;
    }
  }
}

/// <summary>
/// Specyfikacja do pobierania użytkownika na podstawie identyfikatora zewnętrznego
/// </summary>
public class UserByExternalIdSpecification : Specification<User>
{
  /// <summary>
  /// Inicjalizuje nową instancję klasy <see cref="UserByExternalIdSpecification"/>
  /// </summary>
  /// <param name="externalId">Identyfikator zewnętrzny użytkownika</param>
  public UserByExternalIdSpecification(string externalId)
  {
    Query.Where(u => u.ExternalId == externalId);
  }
}

/// <summary>
/// Specyfikacja do pobierania członkostw użytkownika
/// </summary>
public class UserMembershipsSpecification : Specification<Member>
{
  /// <summary>
  /// Inicjalizuje nową instancję klasy <see cref="UserMembershipsSpecification"/>
  /// </summary>
  /// <param name="userId">Identyfikator użytkownika</param>
  public UserMembershipsSpecification(Guid userId)
  {
    Query.Where(m => m.UserId == userId);
  }
}

/// <summary>
/// Specyfikacja do pobierania roli na podstawie identyfikatora
/// </summary>
public class RoleByIdSpecification : Specification<Role>
{
  /// <summary>
  /// Inicjalizuje nową instancję klasy <see cref="RoleByIdSpecification"/>
  /// </summary>
  /// <param name="roleId">Identyfikator roli</param>
  public RoleByIdSpecification(Guid roleId)
  {
    Query.Where(r => r.Id == roleId);
  }
}

/// <summary>
/// Specyfikacja do pobierania roli na podstawie nazwy i identyfikatora organizacji
/// </summary>
public class RoleByNameAndOrgIdSpecification : Specification<Role>
{
  /// <summary>
  /// Inicjalizuje nową instancję klasy <see cref="RoleByNameAndOrgIdSpecification"/>
  /// </summary>
  /// <param name="organizationId">Identyfikator organizacji</param>
  /// <param name="name">Nazwa roli</param>
  public RoleByNameAndOrgIdSpecification(Guid organizationId, string name)
  {
    Query.Where(r => r.OrganizationId == organizationId && r.Name == name);
  }
}

/// <summary>
/// Specyfikacja do pobierania organizacji wraz z członkami i rolami
/// </summary>
public class OrganizationsWithMembersAndRolesSpecification : Specification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationsWithMembersAndRolesSpecification"/>
    /// </summary>
    public OrganizationsWithMembersAndRolesSpecification()
    {
        // Załaduj członków i role organizacji
        Query.Include(o => o.Members)
             .Include(o => o.Roles);
    }
}

/// <summary>
/// Specyfikacja do pobierania organizacji wraz z rolami
/// </summary>
public class OrganizationsWithRolesSpecification : Specification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationsWithRolesSpecification"/>
    /// </summary>
    public OrganizationsWithRolesSpecification()
    {
        // Załaduj role organizacji
        Query.Include(o => o.Roles);
    }
}

/// <summary>
/// Specyfikacja do pobierania organizacji z członkami i rolami dla konkretnego użytkownika
/// </summary>
public class OrganizationsWithMembersForUserSpecification : Specification<Organization>
{
    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="OrganizationsWithMembersForUserSpecification"/>
    /// </summary>
    /// <param name="userId">Identyfikator użytkownika</param>
    public OrganizationsWithMembersForUserSpecification(Guid userId)
    {
        Query.Include(o => o.Members)
             .Include(o => o.Roles)
             .Where(o => o.Members.Any(m => m.UserId == userId));
    }
}
