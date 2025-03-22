# Standardy Kodowania dla .NET - MagJob Identity

Ten dokument opisuje standardy kodowania dla technologii .NET używanej w module Identity projektu MagJob.

## Architektura i Organizacja Kodu

- **Podejście architektoniczne**: Clean Architecture, CQRS, Domain-Driven Design (DDD)
- **Struktura projektu**:
  ```
  src/KeepItUp.MagJob.Identity/
  ├── src/
  │   ├── KeepItUp.MagJob.Identity.Core/           # Warstwa domeny (encje, agregaty, value objects)
  │   ├── KeepItUp.MagJob.Identity.UseCases/       # Warstwa aplikacji (use cases, CQRS)
  │   ├── KeepItUp.MagJob.Identity.Infrastructure/ # Warstwa infrastruktury (repozytoria, zewnętrzne usługi)
  │   └── KeepItUp.MagJob.Identity.Web/            # Warstwa prezentacji (API)
  ├── tests/                                        # Testy
  └── docs/                                         # Dokumentacja
  ```

## Konwencje Nazewnictwa

- **Klasy, Interfejsy, Metody, Properties, Events, Delegates**: PascalCase
  ```csharp
  public class UserService
  public interface IUserRepository
  public void ProcessRequest()
  public string FirstName { get; set; }
  ```
- **Zmienne lokalne, parametry**: camelCase
  ```csharp
  var userId = 123;
  public void ProcessUser(int userId)
  ```
- **Pola prywatne**: _camelCase (z podkreślnikiem)
  ```csharp
  private string _firstName;
  ```
- **Stałe**: PascalCase
  ```csharp
  public const string DefaultRole = "User";
  ```
- **Interfejsy**: I + PascalCase
  ```csharp
  public interface IUserRepository
  ```
- **Przestrzenie nazw**: PascalCase, zgodne z hierarchią katalogów
  ```csharp
  namespace KeepItUp.MagJob.Identity.Core.Interfaces
  ```

## Biblioteki i Frameworki

- **MediatR**: Do implementacji wzorca CQRS i mediator
- **Entity Framework Core**: ORM do dostępu do bazy danych
- **Mapster**: Do mapowania obiektów między warstwami
- **FluentValidation**: Do walidacji danych wejściowych
- **Ardalis.SharedKernel**: Do implementacji wzorców DDD
- **Serilog**: Do strukturalnego logowania

## Dobre Praktyki

### Ogólne

- **Zasady SOLID**: Przestrzeganie zasad SOLID w projektowaniu klas i interfejsów
- **Immutability**: Preferowanie typów niemutowalnych, szczególnie w warstwie domeny
- **Asynchroniczność**: Używanie async/await dla operacji I/O
- **Walidacja**: Walidacja danych wejściowych za pomocą FluentValidation
- **Obsługa błędów**: Używanie Result Pattern zamiast wyjątków do obsługi błędów biznesowych
- **Logowanie**: Strukturalne logowanie z użyciem Serilog
- **Audyt**: Śledzenie wszystkich operacji modyfikujących dane

### Warstwa Domeny (Core)

- **Encapsulation**: Pola prywatne, właściwości z prywatnymi setterami
- **Rich Domain Model**: Logika biznesowa w encjach i agregatach
- **Value Objects**: Używanie Value Objects dla konceptów, które są identyfikowane przez ich wartość, nie tożsamość
- **Domain Events**: Używanie zdarzeń domenowych do komunikacji między agregatami
- **Specifications**: Używanie specyfikacji do enkapsulacji zapytań

### Warstwa Aplikacji (UseCases)

- **CQRS**: Rozdzielenie operacji odczytu i zapisu
- **Mediator Pattern**: Używanie MediatR do implementacji wzorca mediatora
- **Thin Controllers**: Kontrolery powinny tylko delegować pracę do warstwy aplikacji
- **DTOs**: Używanie Data Transfer Objects do komunikacji między warstwami
- **Validation**: Walidacja danych wejściowych przed przekazaniem do warstwy domeny

### Warstwa Infrastruktury (Infrastructure)

- **Repository Pattern**: Implementacja repozytoriów dla dostępu do danych
- **Unit of Work**: Zarządzanie transakcjami
- **External Services**: Integracja z zewnętrznymi usługami
- **Caching**: Implementacja mechanizmów cachowania
- **Audyt**: Implementacja mechanizmów śledzenia zmian

### Warstwa Prezentacji (Web)

- **API Versioning**: Wersjonowanie API
- **API Documentation**: Dokumentacja API za pomocą Swagger/OpenAPI
- **Content Negotiation**: Obsługa różnych formatów danych (JSON, XML)
- **Error Handling**: Globalna obsługa błędów
- **Authentication/Authorization**: Implementacja uwierzytelniania i autoryzacji

## Implementacja CQRS z MediatR

### Komendy (Commands)

Komendy reprezentują intencję zmiany stanu systemu. Powinny być nazwane w trybie rozkazującym (np. `CreateUser`, `UpdateProfile`).

```csharp
public class CreateUserCommand : IRequest<Result<UserDto>>
{
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditService _auditService;
    
    public CreateUserCommandHandler(
        IUserRepository userRepository,
        IAuditService auditService)
    {
        _userRepository = userRepository;
        _auditService = auditService;
    }
    
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User(request.Email, request.FirstName, request.LastName);
        
        await _userRepository.AddAsync(user, cancellationToken);
        await _auditService.LogAsync(AuditAction.Create, "User", user.Id, null, user);
        
        return Result.Success(user.Adapt<UserDto>());
    }
}
```

### Zapytania (Queries)

Zapytania reprezentują intencję pobrania danych bez zmiany stanu systemu. Powinny być nazwane w formie pytającej (np. `GetUserById`, `ListOrganizations`).

```csharp
public class GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public Guid Id { get; set; }
}

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    
    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);
        
        if (user == null)
            return Result.Failure<UserDto>("User not found");
        
        return Result.Success(user.Adapt<UserDto>());
    }
}
```

## Implementacja DDD

### Aggregate Root

```csharp
public class User : EntityBase, IAggregateRoot
{
    public string Email { get; private set; }
    public string FirstName { get; private set; }
    public string LastName { get; private set; }
    public bool IsActive { get; private set; } = true;
    
    private readonly List<UserRole> _roles = new();
    public IReadOnlyCollection<UserRole> Roles => _roles.AsReadOnly();
    
    private User() { } // Dla EF Core
    
    public User(string email, string firstName, string lastName)
    {
        Email = Guard.Against.NullOrEmpty(email, nameof(email));
        FirstName = Guard.Against.NullOrEmpty(firstName, nameof(firstName));
        LastName = Guard.Against.NullOrEmpty(lastName, nameof(lastName));
        
        AddDomainEvent(new UserCreatedEvent(this));
    }
    
    public void AddRole(Role role)
    {
        if (_roles.Any(r => r.RoleId == role.Id))
            return;
            
        var userRole = new UserRole(Id, role.Id);
        _roles.Add(userRole);
        
        AddDomainEvent(new UserRoleAddedEvent(Id, role.Id));
    }
    
    public void Deactivate()
    {
        if (!IsActive)
            return;
            
        IsActive = false;
        
        AddDomainEvent(new UserDeactivatedEvent(Id));
    }
}

public class Organization : EntityBase, IAggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid OwnerId { get; private set; }
    public bool IsActive { get; private set; } = true;
    
    private readonly List<Member> _members = new();
    public IReadOnlyCollection<Member> Members => _members.AsReadOnly();
    
    private readonly List<Role> _roles = new();
    public IReadOnlyCollection<Role> Roles => _roles.AsReadOnly();
    
    private Organization() { } // Dla EF Core
    
    public Organization(string name, string description, Guid ownerId)
    {
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Description = description ?? string.Empty;
        OwnerId = ownerId;
        
        // Dodanie predefiniowanej roli "everyone"
        var everyoneRole = new Role(this.Id, "everyone", "Default role for all members");
        _roles.Add(everyoneRole);
        
        AddDomainEvent(new OrganizationCreatedEvent(this));
    }
    
    public void AddMember(Guid userId, Guid roleId)
    {
        if (_members.Any(m => m.UserId == userId))
            throw new DomainException("User is already a member of this organization");
            
        if (!_roles.Any(r => r.Id == roleId))
            throw new DomainException("Role does not exist in this organization");
            
        var member = new Member(Id, userId, roleId);
        _members.Add(member);
        
        AddDomainEvent(new MemberAddedEvent(Id, userId, roleId));
    }
    
    public Role CreateRole(string name, string description)
    {
        if (_roles.Any(r => r.Name == name))
            throw new DomainException($"Role with name '{name}' already exists");
            
        var role = new Role(Id, name, description);
        _roles.Add(role);
        
        AddDomainEvent(new RoleCreatedEvent(Id, role.Id));
        
        return role;
    }
    
    public void AssignPermissionToRole(Guid roleId, Permission permission)
    {
        var role = _roles.FirstOrDefault(r => r.Id == roleId);
        if (role == null)
            throw new DomainException("Role does not exist in this organization");
            
        role.AddPermission(permission);
        
        AddDomainEvent(new PermissionAssignedEvent(Id, roleId, permission));
    }
    
    public void Deactivate()
    {
        if (!IsActive)
            return;
            
        IsActive = false;
        
        AddDomainEvent(new OrganizationDeactivatedEvent(Id));
    }
}

public class Role : EntityBase
{
    public Guid OrganizationId { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    private readonly List<Permission> _permissions = new();
    public IReadOnlyCollection<Permission> Permissions => _permissions.AsReadOnly();
    
    private Role() { } // Dla EF Core
    
    public Role(Guid organizationId, string name, string description)
    {
        OrganizationId = organizationId;
        Name = Guard.Against.NullOrEmpty(name, nameof(name));
        Description = description ?? string.Empty;
    }
    
    public void AddPermission(Permission permission)
    {
        if (_permissions.Any(p => p == permission))
            return;
            
        _permissions.Add(permission);
    }
    
    public void RemovePermission(Permission permission)
    {
        _permissions.Remove(permission);
    }
}
```

### Value Object

```csharp
public class Address : ValueObject
{
    public string Street { get; private set; }
    public string City { get; private set; }
    public string PostalCode { get; private set; }
    public string Country { get; private set; }
    
    private Address() { } // Dla EF Core
    
    public Address(string street, string city, string postalCode, string country)
    {
        Street = Guard.Against.NullOrEmpty(street, nameof(street));
        City = Guard.Against.NullOrEmpty(city, nameof(city));
        PostalCode = Guard.Against.NullOrEmpty(postalCode, nameof(postalCode));
        Country = Guard.Against.NullOrEmpty(country, nameof(country));
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return Country;
    }
}

public enum Permission
{
    ManageUsers,
    ManageRoles,
    ManageOrganization,
    DeleteOrganization,
    ManageSchedules,
    ViewSchedules,
    ManageWorkEvidence,
    ViewWorkEvidence
}
```

## Audyt

### Interfejs Serwisu Audytu

```csharp
public enum AuditAction
{
    Create,
    Update,
    Delete,
    Login,
    Logout,
    PasswordChange,
    PermissionChange
}

public interface IAuditService
{
    Task LogAsync<T>(
        AuditAction action,
        string entityType,
        Guid entityId,
        T? oldValue,
        T? newValue,
        CancellationToken cancellationToken = default);
        
    Task<IEnumerable<AuditLog>> GetAuditLogsForEntityAsync(
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken = default);
}
```

### Implementacja Serwisu Audytu

```csharp
public class AuditService : IAuditService
{
    private readonly IRepository<AuditLog> _auditLogRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IJsonSerializer _jsonSerializer;
    
    public AuditService(
        IRepository<AuditLog> auditLogRepository,
        ICurrentUserService currentUserService,
        IJsonSerializer jsonSerializer)
    {
        _auditLogRepository = auditLogRepository;
        _currentUserService = currentUserService;
        _jsonSerializer = jsonSerializer;
    }
    
    public async Task LogAsync<T>(
        AuditAction action,
        string entityType,
        Guid entityId,
        T? oldValue,
        T? newValue,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            Timestamp = DateTime.UtcNow,
            UserId = _currentUserService.UserId,
            OldValue = oldValue != null ? _jsonSerializer.Serialize(oldValue) : null,
            NewValue = newValue != null ? _jsonSerializer.Serialize(newValue) : null
        };
        
        await _auditLogRepository.AddAsync(auditLog, cancellationToken);
    }
    
    public async Task<IEnumerable<AuditLog>> GetAuditLogsForEntityAsync(
        string entityType,
        Guid entityId,
        CancellationToken cancellationToken = default)
    {
        var spec = new AuditLogsByEntitySpec(entityType, entityId);
        return await _auditLogRepository.ListAsync(spec, cancellationToken);
    }
}
```

### Encja AuditLog

```csharp
public class AuditLog : EntityBase
{
    public AuditAction Action { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid? UserId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
}
```

## Testowanie

### Testy Jednostkowe

- **Naming**: [UnitOfWork]_[Scenario]_[ExpectedBehavior]
- **Arrange-Act-Assert**: Struktura testów
- **Mocking**: Używanie Moq do mockowania zależności
- **Test Data Builders**: Używanie wzorca Test Data Builder do tworzenia danych testowych

```csharp
[Fact]
public async Task GetUserById_WithExistingId_ReturnsUser()
{
    // Arrange
    var userId = Guid.NewGuid();
    var user = new User("test@example.com", "John", "Doe") { Id = userId };
    
    var repositoryMock = new Mock<IUserRepository>();
    repositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
        .ReturnsAsync(user);
        
    var handler = new GetUserByIdQueryHandler(repositoryMock.Object);
    var query = new GetUserByIdQuery { Id = userId };
    
    // Act
    var result = await handler.Handle(query, CancellationToken.None);
    
    // Assert
    Assert.True(result.IsSuccess);
    Assert.Equal(userId, result.Value.Id);
    Assert.Equal("John", result.Value.FirstName);
    Assert.Equal("Doe", result.Value.LastName);
}
```

### Testy Integracyjne

- **TestServer**: Używanie TestServer do testowania API
- **In-Memory Database**: Używanie bazy danych w pamięci do testów integracyjnych
- **WebApplicationFactory**: Używanie WebApplicationFactory do konfiguracji środowiska testowego

```csharp
public class UserControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;
    
    public UserControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Konfiguracja serwisów dla testów
            });
        });
    }
    
    [Fact]
    public async Task GetUserById_WithExistingId_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        var userId = Guid.NewGuid();
        
        // Act
        var response = await client.GetAsync($"/api/users/{userId}");
        
        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var user = JsonSerializer.Deserialize<UserDto>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        
        Assert.NotNull(user);
        Assert.Equal(userId, user.Id);
    }
}
```

## Baza Danych

### Migracje

- **Code-First**: Używanie podejścia Code-First do tworzenia schematu bazy danych
- **Migrations**: Używanie EF Core Migrations do zarządzania zmianami schematu
- **Seed Data**: Używanie metody `OnModelCreating` do seedowania danych

### Konfiguracja Encji

```csharp
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", "identity");
        
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();
            
        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.HasMany(u => u.Roles)
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.ToTable("Organizations", "identity");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Name)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(o => o.Description)
            .HasMaxLength(500);
            
        builder.HasMany(o => o.Members)
            .WithOne()
            .HasForeignKey(m => m.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasMany(o => o.Roles)
            .WithOne()
            .HasForeignKey(r => r.OrganizationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("AuditLogs", "identity");
        
        builder.HasKey(a => a.Id);
        
        builder.Property(a => a.EntityType)
            .HasMaxLength(100)
            .IsRequired();
            
        builder.Property(a => a.OldValue)
            .HasColumnType("jsonb");
            
        builder.Property(a => a.NewValue)
            .HasColumnType("jsonb");
            
        builder.HasIndex(a => new { a.EntityType, a.EntityId });
        builder.HasIndex(a => a.Timestamp);
        builder.HasIndex(a => a.UserId);
    }
}
```

## API

### Kontrolery

```csharp
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    
    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery { Id = id });
        
        if (result.IsSuccess)
            return Ok(result.Value);
            
        return NotFound(result.Error);
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateUserCommand command)
    {
        var result = await _mediator.Send(command);
        
        if (result.IsSuccess)
            return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
            
        return BadRequest(result.Error);
    }
}
```

## Bezpieczeństwo

### Uwierzytelnianie i Autoryzacja

- **JWT**: Używanie JSON Web Tokens do uwierzytelniania
- **Claims-Based Authorization**: Używanie autoryzacji opartej na claimach
- **Policy-Based Authorization**: Używanie autoryzacji opartej na politykach

```csharp
[Authorize(Policy = "RequireAdminRole")]
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(Guid id)
{
    var result = await _mediator.Send(new DeleteUserCommand { Id = id });
    
    if (result.IsSuccess)
        return NoContent();
        
    return NotFound(result.Error);
}
```

## Logowanie

### Serilog

```csharp
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();
```

## Obsługa Błędów

### Middleware

```csharp
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    
    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception has occurred");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = JsonSerializer.Serialize(new { error = "An error occurred while processing your request." });
        
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        
        return context.Response.WriteAsync(result);
    }
}
```

## Dokumentacja API

### Swagger/OpenAPI

```csharp
services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "MagJob Identity API",
        Version = "v1",
        Description = "API for managing users and organizations in MagJob"
    });
    
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
``` 