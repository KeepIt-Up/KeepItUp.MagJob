# Standardy Kodowania dla .NET i Angular - MagJob

Ten dokument opisuje standardy kodowania dla technologii .NET i Angular używanych w projekcie MagJob.

## Standardy dla .NET

### Architektura i Organizacja Kodu

- **Podejście architektoniczne**: Clean Architecture, CQRS, Domain-Driven Design (DDD)
- **Struktura projektu**:
  ```
  src/Organizations/
  ├── Organizations.API/           # Warstwa prezentacji (API)
  ├── Organizations.Application/   # Warstwa aplikacji (use cases, CQRS)
  ├── Organizations.Domain/        # Warstwa domeny (encje, agregaty, value objects)
  ├── Organizations.Infrastructure/# Warstwa infrastruktury (repozytoria, zewnętrzne usługi)
  └── Organizations.Tests/         # Testy
  ```

### Konwencje Nazewnictwa

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

### Biblioteki i Frameworki

- **MediatR**: Do implementacji wzorca CQRS i mediator
- **Entity Framework Core**: ORM do dostępu do bazy danych
- **Mapster**: Do mapowania obiektów między warstwami
- **FluentValidation**: Do walidacji danych wejściowych

### Dobre Praktyki

- **Zasady SOLID**: Przestrzeganie zasad SOLID w projektowaniu klas i interfejsów
- **Immutability**: Preferowanie typów niemutowalnych, szczególnie w warstwie domeny
- **Asynchroniczność**: Używanie async/await dla operacji I/O
- **Walidacja**: Walidacja danych wejściowych za pomocą FluentValidation
- **Obsługa błędów**: Używanie Result Pattern zamiast wyjątków do obsługi błędów biznesowych
- **Logowanie**: Strukturalne logowanie z użyciem Serilog

### Przykład Implementacji CQRS z MediatR

```csharp
// Command
public class CreateUserCommand : IRequest<Result<UserDto>>
{
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

// Command Handler
public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<UserDto>>
{
    private readonly IUserRepository _userRepository;
    
    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }
    
    public async Task<Result<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var user = new User(request.Email, request.FirstName, request.LastName);
        
        await _userRepository.AddAsync(user, cancellationToken);
        
        return Result.Success(user.Adapt<UserDto>());
    }
}

// Query
public class GetUserByIdQuery : IRequest<Result<UserDto>>
{
    public Guid Id { get; set; }
}

// Query Handler
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

### Przykład Implementacji DDD

```csharp
// Aggregate Root
public class Organization : AggregateRoot
{
    private readonly List<Member> _members = new();
    
    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid OwnerId { get; private set; }
    public bool IsActive { get; private set; }
    
    public IReadOnlyCollection<Member> Members => _members.AsReadOnly();
    
    private Organization() { } // Dla EF Core
    
    public Organization(string name, string description, Guid ownerId)
    {
        Name = name;
        Description = description;
        OwnerId = ownerId;
        IsActive = true;
        
        AddDomainEvent(new OrganizationCreatedDomainEvent(Id, name, ownerId));
    }
    
    public void AddMember(Guid userId, Guid roleId)
    {
        if (_members.Any(m => m.UserId == userId))
            throw new DomainException("User is already a member of this organization");
        
        var member = new Member(Id, userId, roleId);
        _members.Add(member);
        
        AddDomainEvent(new MemberAddedDomainEvent(Id, userId, roleId));
    }
    
    public void Deactivate()
    {
        if (!IsActive)
            return;
        
        IsActive = false;
        
        AddDomainEvent(new OrganizationDeactivatedDomainEvent(Id));
    }
}

// Value Object
public class Address : ValueObject
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }
    
    private Address() { } // Dla EF Core
    
    public Address(string street, string city, string postalCode, string country)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return PostalCode;
        yield return Country;
    }
}
```

## Standardy dla Angular

### Struktura Projektu

- **Podejście**: Modułowa struktura oparta na funkcjonalnościach
- **Struktura katalogów**:
  ```
  src/Client/Client.Web/
  ├── app/
  │   ├── core/                # Serwisy, guardy, interceptory używane w całej aplikacji
  │   │   ├── auth/            # Autentykacja i autoryzacja
  │   │   ├── http/            # Interceptory HTTP
  │   │   └── services/        # Serwisy współdzielone
  │   ├── features/            # Moduły funkcjonalne
  │   │   ├── organizations/   # Moduł organizacji
  │   │   ├── schedules/       # Moduł grafików
  │   │   └── work-evidence/   # Moduł ewidencji czasu pracy
  │   ├── shared/              # Komponenty, dyrektywy, pipes współdzielone
  │   │   ├── components/      # Współdzielone komponenty
  │   │   ├── directives/      # Współdzielone dyrektywy
  │   │   └── pipes/           # Współdzielone pipes
  │   └── app.module.ts        # Główny moduł aplikacji
  ├── assets/                  # Statyczne zasoby (obrazy, ikony, itp.)
  └── environments/            # Konfiguracje środowiskowe
  ```

### Konwencje Nazewnictwa

- **Pliki**: kebab-case.type.ts
  ```
  user-list.component.ts
  auth.service.ts
  date-format.pipe.ts
  ```
- **Klasy**: PascalCase
  ```typescript
  export class UserListComponent
  export class AuthService
  ```
- **Zmienne, metody**: camelCase
  ```typescript
  const firstName: string;
  getUserById(): Observable<User>
  ```
- **Stałe**: UPPER_SNAKE_CASE lub PascalCase
  ```typescript
  const API_URL = 'https://api.example.com';
  const DefaultPageSize = 10;
  ```
- **Interfejsy**: PascalCase (bez prefiksu I)
  ```typescript
  export interface User
  export interface Organization
  ```
- **Enumy**: PascalCase
  ```typescript
  export enum UserRole
  export enum ScheduleStatus
  ```

### Biblioteki i Frameworki

- **RxJS**: Do reaktywnego programowania i zarządzania asynchronicznością
- **Tailwind CSS**: Do stylowania komponentów
- **Własne komponenty UI**: Zamiast zewnętrznych bibliotek UI

### Zarządzanie Stanem

- **RxJS**: Używanie serwisów z BehaviorSubject/ReplaySubject do zarządzania stanem
- **Przykład**:
  ```typescript
  @Injectable({
    providedIn: 'root'
  })
  export class OrganizationStateService {
    private _organizations = new BehaviorSubject<Organization[]>([]);
    private _selectedOrganization = new BehaviorSubject<Organization | null>(null);
    
    public readonly organizations$ = this._organizations.asObservable();
    public readonly selectedOrganization$ = this._selectedOrganization.asObservable();
    
    constructor(private apiService: ApiService) {}
    
    loadOrganizations(): Observable<Organization[]> {
      return this.apiService.getOrganizations().pipe(
        tap(organizations => this._organizations.next(organizations))
      );
    }
    
    selectOrganization(id: string): void {
      const organization = this._organizations.value.find(org => org.id === id) || null;
      this._selectedOrganization.next(organization);
    }
  }
  ```

### Testowanie

- **Jasmine**: Framework testowy
- **Karma**: Test runner
- **Testy jednostkowe**: Dla serwisów, pipes, komponentów
- **Testy integracyjne**: Dla interakcji między komponentami

### Dobre Praktyki

- **OnPush Change Detection**: Używanie strategii OnPush dla lepszej wydajności
- **Lazy Loading**: Ładowanie modułów na żądanie
- **Reactive Forms**: Używanie Reactive Forms zamiast Template-driven Forms
- **Typowanie**: Ścisłe typowanie z TypeScript
- **Immutability**: Unikanie mutacji stanu, używanie operatorów RxJS do transformacji danych

### Przykład Komponentu

```typescript
import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Observable } from 'rxjs';
import { OrganizationService } from '../../services/organization.service';
import { Organization } from '../../models/organization.model';

@Component({
  selector: 'app-organization-form',
  templateUrl: './organization-form.component.html',
  styleUrls: ['./organization-form.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class OrganizationFormComponent implements OnInit {
  organizationForm: FormGroup;
  isSubmitting = false;
  
  constructor(
    private fb: FormBuilder,
    private organizationService: OrganizationService
  ) {
    this.organizationForm = this.fb.group({
      name: ['', [Validators.required, Validators.maxLength(100)]],
      description: ['', Validators.maxLength(500)]
    });
  }
  
  ngOnInit(): void {
  }
  
  onSubmit(): void {
    if (this.organizationForm.invalid || this.isSubmitting) {
      return;
    }
    
    this.isSubmitting = true;
    
    const organization: Partial<Organization> = {
      name: this.organizationForm.get('name')?.value,
      description: this.organizationForm.get('description')?.value
    };
    
    this.organizationService.createOrganization(organization)
      .subscribe({
        next: () => {
          // Handle success
          this.organizationForm.reset();
        },
        error: (error) => {
          // Handle error
          console.error('Error creating organization', error);
        },
        complete: () => {
          this.isSubmitting = false;
        }
      });
  }
}
```

### Przykład Serwisu

```typescript
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { Organization } from '../models/organization.model';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class OrganizationService {
  private apiUrl = `${environment.apiUrl}/organizations`;
  
  constructor(private http: HttpClient) {}
  
  getOrganizations(): Observable<Organization[]> {
    return this.http.get<Organization[]>(this.apiUrl);
  }
  
  getOrganizationById(id: string): Observable<Organization> {
    return this.http.get<Organization>(`${this.apiUrl}/${id}`);
  }
  
  createOrganization(organization: Partial<Organization>): Observable<Organization> {
    return this.http.post<Organization>(this.apiUrl, organization);
  }
  
  updateOrganization(id: string, organization: Partial<Organization>): Observable<Organization> {
    return this.http.put<Organization>(`${this.apiUrl}/${id}`, organization);
  }
  
  deleteOrganization(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
``` 
