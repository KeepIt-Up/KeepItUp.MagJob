# Integracja z Keycloak - MagJob Identity

Ten dokument opisuje szczegóły integracji modułu Identity z Keycloak jako zewnętrznym dostawcą tożsamości.

## Przegląd

Keycloak jest używany jako centralny system zarządzania tożsamością i dostępem (IAM) dla aplikacji MagJob. Moduł Identity integruje się z Keycloak, aby zapewnić spójne zarządzanie użytkownikami, rolami i uprawnieniami.

## Podział Odpowiedzialności

### Keycloak

- **Przechowywanie danych wrażliwych** użytkowników (hasła, dane osobowe)
- **Uwierzytelnianie użytkowników** (logowanie, weryfikacja tożsamości)
- **Zarządzanie sesjami** użytkowników
- **Generowanie tokenów JWT** zawierających informacje o użytkowniku i jego uprawnieniach
- **Implementacja standardów OAuth 2.0 i OpenID Connect**
- **Integracja z zewnętrznymi dostawcami tożsamości** (opcjonalnie)
- **Obsługa logowania i rejestracji użytkowników** poprzez dedykowane interfejsy

### Moduł Identity

- **Zarządzanie organizacjami** (tworzenie, aktualizacja, usuwanie)
- **Zarządzanie członkostwem** użytkowników w organizacjach
- **Zarządzanie rolami i uprawnieniami** w kontekście organizacji
- **Synchronizacja danych** z Keycloak (przekazywanie informacji o rolach i organizacjach)
- **Audyt operacji** związanych z tożsamością i dostępem

## Przepływ Danych

### Synchronizacja Ról i Organizacji

Moduł Identity przekazuje do Keycloak informacje o rolach i organizacjach użytkownika, aby mogły być mapowane do tokenów JWT. Dzięki temu aplikacje klienckie mogą podejmować decyzje autoryzacyjne na podstawie tokenów, bez konieczności dodatkowych zapytań do API.

```
┌─────────────┐      ┌─────────────┐      ┌─────────────┐
│             │      │             │      │             │
│   Identity  │──────▶  Keycloak   │──────▶   Klient    │
│    Moduł    │      │             │      │             │
│             │      │             │      │             │
└─────────────┘      └─────────────┘      └─────────────┘
       │                    │                    │
       │                    │                    │
       │                    │                    │
       ▼                    ▼                    ▼
┌─────────────┐      ┌─────────────┐      ┌─────────────┐
│  Zarządzanie│      │ Generowanie │      │  Decyzje    │
│   rolami i  │      │  tokenów z  │      │autoryzacyjne│
│organizacjami│      │ informacjami│      │ na podstawie│
│             │      │   o rolach  │      │   tokenów   │
└─────────────┘      └─────────────┘      └─────────────┘
```

### Mapowanie Atrybutów

Keycloak mapuje informacje o rolach i organizacjach użytkownika do atrybutów w tokenach JWT:

```json
{
  "sub": "1234567890",
  "name": "Jan Kowalski",
  "email": "jan.kowalski@example.com",
  "organizations": [
    {
      "id": "org-123",
      "name": "Firma XYZ",
      "roles": ["admin", "manager"]
    },
    {
      "id": "org-456",
      "name": "Firma ABC",
      "roles": ["user"]
    }
  ],
  "permissions": [
    "ManageUsers",
    "ManageOrganization",
    "ViewSchedules"
  ]
}
```

## Proces Logowania i Rejestracji

### Wybrana Opcja: Logowanie i Rejestracja przez Keycloak

Zdecydowaliśmy się na przekierowanie użytkownika do Keycloak w celu logowania lub rejestracji.

**Zalety:**
- Standardowy przepływ OAuth 2.0 / OpenID Connect
- Łatwiejsza integracja z zewnętrznymi dostawcami tożsamości
- Mniej kodu do napisania i utrzymania
- Większe bezpieczeństwo dzięki wykorzystaniu sprawdzonego rozwiązania

**Wyzwania i rozwiązania:**
- **Mniejsza kontrola nad interfejsem użytkownika** - Rozwiązanie: Dostosowanie motywu Keycloak do stylu aplikacji MagJob
- **Trudniejsza integracja z procesami biznesowymi** - Rozwiązanie: Implementacja webhooków lub event listenera do reagowania na zdarzenia rejestracji/logowania w Keycloak

### Przepływ Logowania

1. Użytkownik klika przycisk "Zaloguj się" w aplikacji MagJob
2. Aplikacja przekierowuje użytkownika do Keycloak
3. Użytkownik wprowadza dane logowania w Keycloak
4. Po pomyślnym uwierzytelnieniu, Keycloak przekierowuje użytkownika z powrotem do aplikacji MagJob z kodem autoryzacyjnym
5. Aplikacja wymienia kod na token dostępu i token odświeżania
6. Aplikacja używa tokenu dostępu do uwierzytelniania żądań do API

### Przepływ Rejestracji

1. Użytkownik klika przycisk "Zarejestruj się" w aplikacji MagJob
2. Aplikacja przekierowuje użytkownika do Keycloak
3. Użytkownik wypełnia formularz rejestracji w Keycloak
4. Po pomyślnej rejestracji, Keycloak przekierowuje użytkownika z powrotem do aplikacji MagJob
5. Aplikacja MagJob otrzymuje powiadomienie o nowym użytkowniku (poprzez webhook lub event listener)
6. Moduł Identity tworzy odpowiednie rekordy w swojej bazie danych

## Implementacja

### Konfiguracja FastEndpoints

Używamy biblioteki FastEndpoints zamiast standardowych kontrolerów ASP.NET Core:

```csharp
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Dodanie FastEndpoints
        builder.Services.AddFastEndpoints();

        // Konfiguracja uwierzytelniania
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = builder.Configuration["Keycloak:Authority"];
            options.Audience = builder.Configuration["Keycloak:Audience"];
            options.RequireHttpsMetadata = true;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true
            };
        });

        var app = builder.Build();

        // Konfiguracja middleware
        app.UseAuthentication();
        app.UseAuthorization();
        
        // Konfiguracja FastEndpoints
        app.UseFastEndpoints();

        app.Run();
    }
}
```

### Przykład Endpointu z FastEndpoints

```csharp
public class GetOrganizationEndpoint : Endpoint<GetOrganizationRequest, OrganizationResponse>
{
    private readonly IMediator _mediator;

    public GetOrganizationEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override void Configure()
    {
        Get("api/organizations/{id}");
        AllowAnonymous(false); // Wymaga uwierzytelnienia
        Description(b => b
            .WithName("GetOrganization")
            .Produces<OrganizationResponse>(200, "application/json")
            .Produces(404)
        );
    }

    public override async Task HandleAsync(GetOrganizationRequest req, CancellationToken ct)
    {
        // Sprawdzenie uprawnień w kontekście organizacji i roli użytkownika
        var hasAccess = await CheckOrganizationAccess(req.Id, "ViewOrganization");
        if (!hasAccess)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        var result = await _mediator.Send(new GetOrganizationByIdQuery { Id = req.Id }, ct);

        if (result.IsSuccess)
            await SendOkAsync(result.Value, ct);
        else
            await SendNotFoundAsync(ct);
    }

    private async Task<bool> CheckOrganizationAccess(Guid organizationId, string requiredPermission)
    {
        // Pobierz informacje o organizacjach i rolach użytkownika z tokenu
        var organizationsClaim = User.FindFirst("organizations")?.Value;
        if (string.IsNullOrEmpty(organizationsClaim))
            return false;

        var organizations = JsonSerializer.Deserialize<List<KeycloakOrganization>>(organizationsClaim);
        var organization = organizations?.FirstOrDefault(o => o.Id == organizationId.ToString());
        
        if (organization == null)
            return false;

        // Sprawdź, czy którakolwiek z ról użytkownika w tej organizacji ma wymagane uprawnienie
        foreach (var role in organization.Roles)
        {
            // Pobierz uprawnienia dla roli w kontekście tej organizacji
            var permissions = await GetPermissionsForRoleInOrganization(organizationId, role);
            if (permissions.Contains(requiredPermission))
                return true;
        }

        return false;
    }
}
```

### Model Uprawnień

Uprawnienia są przyznawane na podstawie kontekstu organizacji oraz roli użytkownika:

```csharp
public class PermissionService : IPermissionService
{
    private readonly IOrganizationRepository _organizationRepository;
    
    public PermissionService(IOrganizationRepository organizationRepository)
    {
        _organizationRepository = organizationRepository;
    }
    
    public async Task<bool> HasPermissionAsync(
        Guid userId, 
        Guid organizationId, 
        string permission,
        CancellationToken cancellationToken = default)
    {
        // Pobierz role użytkownika w organizacji
        var roles = await _organizationRepository.GetUserRolesInOrganizationAsync(
            userId, 
            organizationId, 
            cancellationToken);
            
        if (roles == null || !roles.Any())
            return false;
            
        // Sprawdź, czy którakolwiek z ról ma wymagane uprawnienie
        foreach (var role in roles)
        {
            if (role.Permissions.Any(p => p.ToString() == permission))
                return true;
        }
        
        return false;
    }
    
    public async Task<IEnumerable<string>> GetUserPermissionsInOrganizationAsync(
        Guid userId, 
        Guid organizationId,
        CancellationToken cancellationToken = default)
    {
        // Pobierz role użytkownika w organizacji
        var roles = await _organizationRepository.GetUserRolesInOrganizationAsync(
            userId, 
            organizationId, 
            cancellationToken);
            
        if (roles == null || !roles.Any())
            return Enumerable.Empty<string>();
            
        // Zbierz wszystkie unikalne uprawnienia ze wszystkich ról
        var permissions = new HashSet<string>();
        foreach (var role in roles)
        {
            foreach (var permission in role.Permissions)
            {
                permissions.Add(permission.ToString());
            }
        }
        
        return permissions;
    }
}
```

### Synchronizacja Danych z Keycloak

```csharp
public class KeycloakSyncService : IKeycloakSyncService
{
    private readonly IKeycloakClient _keycloakClient;
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    
    public KeycloakSyncService(
        IKeycloakClient keycloakClient,
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository)
    {
        _keycloakClient = keycloakClient;
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
    }
    
    public async Task SyncUserRolesAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            throw new NotFoundException("User not found");
            
        var memberships = await _userRepository.GetUserMembershipsAsync(userId);
        var organizationsWithRoles = new List<KeycloakOrganization>();
        
        foreach (var membership in memberships)
        {
            var organization = await _organizationRepository.GetByIdAsync(membership.OrganizationId);
            var role = await _organizationRepository.GetRoleByIdAsync(membership.RoleId);
            
            // Sprawdź, czy organizacja już istnieje w liście
            var existingOrg = organizationsWithRoles.FirstOrDefault(o => o.Id == organization.Id.ToString());
            if (existingOrg != null)
            {
                // Dodaj rolę do istniejącej organizacji
                if (!existingOrg.Roles.Contains(role.Name))
                {
                    existingOrg.Roles.Add(role.Name);
                }
            }
            else
            {
                // Dodaj nową organizację z rolą
                organizationsWithRoles.Add(new KeycloakOrganization
                {
                    Id = organization.Id.ToString(),
                    Name = organization.Name,
                    Roles = new List<string> { role.Name }
                });
            }
        }
        
        // Zbierz wszystkie uprawnienia użytkownika we wszystkich organizacjach
        var allPermissions = new HashSet<string>();
        foreach (var org in organizationsWithRoles)
        {
            var orgId = Guid.Parse(org.Id);
            foreach (var roleName in org.Roles)
            {
                var role = await _organizationRepository.GetRoleByNameAsync(orgId, roleName);
                foreach (var permission in role.Permissions)
                {
                    allPermissions.Add(permission.ToString());
                }
            }
        }
        
        // Aktualizuj atrybuty użytkownika w Keycloak
        await _keycloakClient.UpdateUserAttributesAsync(user.ExternalId, new Dictionary<string, List<string>>
        {
            ["organizations"] = new List<string> { JsonSerializer.Serialize(organizationsWithRoles) },
            ["permissions"] = allPermissions.ToList()
        });
    }
}
```

## Bezpieczeństwo

### Ochrona Endpointów z FastEndpoints

```csharp
// Bazowa klasa dla endpointów wymagających uwierzytelnienia
public abstract class AuthenticatedEndpoint<TRequest, TResponse> : Endpoint<TRequest, TResponse>
    where TRequest : notnull
{
    protected IPermissionService PermissionService => Resolve<IPermissionService>();
    
    public override void Configure()
    {
        AllowAnonymous(false); // Wymaga uwierzytelnienia
    }
    
    protected async Task<bool> HasOrganizationPermission(Guid organizationId, string permission)
    {
        var userId = User.GetUserId();
        return await PermissionService.HasPermissionAsync(userId, organizationId, permission);
    }
    
    protected async Task<bool> IsOrganizationAdmin(Guid organizationId)
    {
        var userId = User.GetUserId();
        var permissions = await PermissionService.GetUserPermissionsInOrganizationAsync(userId, organizationId);
        return permissions.Contains("ManageOrganization");
    }
}

// Przykład endpointu dziedziczącego po AuthenticatedEndpoint
public class CreateRoleEndpoint : AuthenticatedEndpoint<CreateRoleRequest, RoleResponse>
{
    private readonly IMediator _mediator;
    
    public CreateRoleEndpoint(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public override void Configure()
    {
        base.Configure();
        Post("api/organizations/{OrganizationId}/roles");
        Description(b => b
            .WithName("CreateRole")
            .Produces<RoleResponse>(201, "application/json")
            .Produces(400)
            .Produces(403)
        );
    }
    
    public override async Task HandleAsync(CreateRoleRequest req, CancellationToken ct)
    {
        // Sprawdź, czy użytkownik ma uprawnienie do zarządzania rolami w tej organizacji
        if (!await HasOrganizationPermission(req.OrganizationId, "ManageRoles"))
        {
            await SendForbiddenAsync(ct);
            return;
        }
        
        var command = new CreateRoleCommand
        {
            OrganizationId = req.OrganizationId,
            Name = req.Name,
            Description = req.Description,
            Permissions = req.Permissions
        };
        
        var result = await _mediator.Send(command, ct);
        
        if (result.IsSuccess)
            await SendCreatedAtAsync<GetRoleEndpoint>(
                new { id = result.Value.Id, organizationId = req.OrganizationId },
                result.Value,
                cancellation: ct);
        else
            await SendErrorsAsync(400, result.Error, ct);
    }
}
```

## Wyzwania i Rozwiązania

### Synchronizacja Danych

Wyzwanie: Utrzymanie spójności danych między modułem Identity a Keycloak.

Rozwiązanie:
- Implementacja mechanizmu synchronizacji danych
- Używanie zdarzeń domenowych do wyzwalania synchronizacji
- Okresowe zadania sprawdzające spójność danych

### Wydajność

Wyzwanie: Minimalizacja opóźnień związanych z komunikacją z Keycloak.

Rozwiązanie:
- Cachowanie tokenów i informacji o użytkownikach
- Asynchroniczna synchronizacja danych
- Optymalizacja zapytań do Keycloak

### Bezpieczeństwo

Wyzwanie: Zapewnienie bezpiecznej komunikacji między modułem Identity a Keycloak.

Rozwiązanie:
- Używanie HTTPS dla wszystkich połączeń
- Uwierzytelnianie klienta przy komunikacji z Keycloak
- Walidacja tokenów JWT
- Ograniczenie dostępu do API Keycloak

## Podsumowanie

Integracja z Keycloak pozwala na rozdzielenie odpowiedzialności między systemem uwierzytelniania a logiką biznesową modułu Identity. Keycloak przechowuje dane wrażliwe i zarządza uwierzytelnianiem, podczas gdy moduł Identity koncentruje się na zarządzaniu organizacjami, rolami i uprawnieniami.

Wybraliśmy opcję logowania i rejestracji przez Keycloak, co zapewnia standardowy i bezpieczny przepływ uwierzytelniania, jednocześnie minimalizując ilość kodu do napisania i utrzymania. Uprawnienia są przyznawane na podstawie kontekstu organizacji oraz roli użytkownika, co zapewnia precyzyjną kontrolę dostępu do funkcji systemu. 