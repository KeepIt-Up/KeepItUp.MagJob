# Plan Implementacji Modułu Identity

Ten dokument opisuje plan implementacji modułu Identity, w tym integrację z Keycloak oraz logikę dla użytkowników, organizacji, ról i uprawnień.

## Przegląd

Plan implementacji jest podzielony na pięć głównych etapów:
1. Podstawowa Struktura
2. Integracja z Keycloak
3. Przypadki Użycia i Endpointy
4. Audyt i Bezpieczeństwo
5. Testy i Dokumentacja

## Modele Domeny (KeepItUp.MagJob.Identity.Core)

### Agregaty i Encje

#### UserAggregate

- [x] `User` (Aggregate Root)
  - [x] Podstawowe właściwości (Id, ExternalId, Email, FirstName, LastName)
  - [x] Metody biznesowe (Update, Deactivate)
  - [x] Zdarzenia domenowe

- [x] `UserProfile` (Value Object)
  - [x] Właściwości (PhoneNumber, Address, ProfileImage)
  - [x] Implementacja jako prawdziwy Value Object (niezmienność, równość)

- [ ] `ExternalIdentity` (Value Object dla integracji z Keycloak)
  - [ ] Właściwości (Provider, ExternalId, Claims)

#### OrganizationAggregate

- [x] `Organization` (Aggregate Root)
  - [x] Podstawowe właściwości (Id, Name, Description, OwnerId)
  - [x] Metody biznesowe (Update, AddMember, RemoveMember, AddRole, RemoveRole)
  - [x] Zdarzenia domenowe

- [x] `Member` (Entity)
  - [x] Podstawowe właściwości (Id, UserId, OrganizationId)
  - [x] Możliwość posiadania wielu ról
  - [x] Metody biznesowe (AssignRole, RemoveRole, HasRole)

- [x] `Invitation` (Entity)
  - [x] Podstawowe właściwości (Id, OrganizationId, Email, Token, ExpiresAt)
  - [x] Metody biznesowe (Accept, Reject, Expire)

- [x] `Role` (Entity)
  - [x] Podstawowe właściwości (Id, Name, Description, OrganizationId)
  - [x] Metody biznesowe (Update, AddPermission, RemovePermission)

- [x] `Permission` (Value Object)
  - [x] Właściwości (Name, Description)

### Zdarzenia Domenowe

#### UserAggregate

- [x] `UserCreatedEvent`
- [x] `UserUpdatedEvent`
- [x] `UserDeactivatedEvent`
- [x] `UserActivatedEvent`

#### OrganizationAggregate

- [x] `OrganizationCreatedEvent`
- [x] `OrganizationUpdatedEvent`
- [x] `OrganizationDeactivatedEvent`
- [x] `OrganizationActivatedEvent`
- [x] `MemberAddedEvent`
- [x] `MemberRemovedEvent`
- [x] `MemberRoleAssignedEvent`
- [x] `MemberRoleRevokedEvent`
- [x] `RoleCreatedEvent`
- [x] `RoleUpdatedEvent`
- [x] `RoleDeletedEvent`
- [x] `PermissionAssignedEvent`
- [x] `PermissionRevokedEvent`
- [x] `InvitationCreatedEvent`
- [x] `InvitationAcceptedEvent`
- [x] `InvitationRejectedEvent`

### Specyfikacje i Repozytoria

- [x] Wykorzystanie wzorca Specification zamiast dedykowanych repozytoriów
  - [x] Specyfikacje dla User
    - [x] `UserByIdSpec`
    - [x] `UserByEmailSpec`
    - [x] `UserByExternalIdSpec`
    - [x] `ActiveUsersSpec`
  - [x] Specyfikacje dla Organization
    - [x] `OrganizationByIdSpec`
    - [x] `OrganizationByNameSpec`
    - [x] `OrganizationWithMembersSpec`
    - [x] `OrganizationWithRolesSpec`
  - [x] Specyfikacje dla Member
    - [x] `MemberByUserIdSpec`
    - [x] `MemberByUserIdAndOrgIdSpec`
  - [x] Specyfikacje dla Role
    - [x] `RoleByIdSpec`
    - [x] `RoleByNameAndOrgIdSpec`
    - [x] `RoleWithMembersSpec`

- [x] Wykorzystanie generycznych interfejsów repozytorium z Ardalis.Specification
  - [x] `IRepository<T>` - do operacji zapisu i odczytu
  - [x] `IReadRepository<T>` - tylko do operacji odczytu

## Przypadki Użycia (KeepItUp.MagJob.Identity.UseCases)

### Użytkownicy

#### Komendy

- [ ] `CreateUserCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `UpdateUserCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `DeactivateUserCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

#### Zapytania

- [ ] `GetUserByIdQuery`
  - [ ] Request
  - [ ] Handler
  - [ ] Response

- [ ] `GetUserByExternalIdQuery`
  - [ ] Request
  - [ ] Handler
  - [ ] Response

- [ ] `GetUserOrganizationsQuery`
  - [ ] Request
  - [ ] Handler
  - [ ] Response

### Organizacje

#### Komendy

- [ ] `CreateOrganizationCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `UpdateOrganizationCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `DeactivateOrganizationCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `CreateInvitationCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `AcceptInvitationCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `RejectInvitationCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `RemoveMemberCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

#### Zapytania

- [ ] `GetOrganizationByIdQuery`
  - [ ] Request
  - [ ] Handler
  - [ ] Response

- [ ] `GetOrganizationMembersQuery`
  - [ ] Request
  - [ ] Handler
  - [ ] Response

- [ ] `GetOrganizationInvitationsQuery`
  - [ ] Request
  - [ ] Handler
  - [ ] Response

### Role i Uprawnienia

#### Komendy

- [ ] `CreateRoleCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `UpdateRoleCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `DeleteRoleCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `AssignRoleToMemberCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `RevokeRoleFromMemberCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

- [ ] `UpdateRolePermissionsCommand`
  - [ ] Request
  - [ ] Handler
  - [ ] Validator

#### Zapytania

- [ ] `GetRoleByIdQuery`
  - [ ] Request
  - [ ] Handler
  - [ ] Response

- [ ] `GetRolesByOrganizationIdQuery`
  - [ ] Request
  - [ ] Handler
  - [ ] Response

- [ ] `GetPermissionsQuery`
  - [ ] Request
  - [ ] Handler
  - [ ] Response

## Infrastruktura (KeepItUp.MagJob.Identity.Infrastructure)

### Baza Danych

- [x] `AppDbContext`
  - [x] DbSets
  - [x] Konfiguracja SaveChanges (dla zdarzeń domenowych)
  - [x] Automatyczna aktualizacja pól CreatedAt i UpdatedAt

- [ ] Konfiguracje encji
  - [ ] `UserConfiguration`
  - [ ] `OrganizationConfiguration`
  - [ ] `MemberConfiguration`
  - [ ] `RoleConfiguration`
  - [ ] `InvitationConfiguration`
  - [ ] `AuditLogConfiguration`

- [ ] Migracje
  - [ ] Inicjalna migracja
  - [ ] Seed danych (uprawnienia, role systemowe)

- [x] Implementacje repozytoriów
  - [x] Wykorzystanie `EfRepository<T>` z Ardalis.Specification.EntityFrameworkCore

### Obsługa Zdarzeń Domenowych

- [x] `IDomainEventDispatcher`
  - [x] Interfejs dla dyspozytora zdarzeń domenowych
- [x] `MediatRDomainEventDispatcher`
  - [x] Implementacja dyspozytora zdarzeń domenowych używająca MediatR

### Integracja z Keycloak

- [x] `KeycloakClient`
  - [x] Metody do komunikacji z Keycloak API
  - [x] Obsługa tokenów
  - [x] Zarządzanie użytkownikami

- [x] `KeycloakOptions`
  - [x] Konfiguracja połączenia z Keycloak

- [x] `KeycloakSyncService`
  - [x] Synchronizacja ról i uprawnień
  - [x] Synchronizacja użytkowników
  - [x] Optymalizacja pobierania danych (eager loading)

- [x] `KeycloakEventListener`
  - [x] Nasłuchiwanie zdarzeń z Keycloak
  - [x] Obsługa rejestracji użytkowników
  - [x] Obsługa logowania użytkowników

- [x] `KeycloakAttributeMapper`
  - [x] Mapowanie ról i uprawnień do atrybutów w tokenach JWT

### Audyt

- [ ] `AuditService`
  - [ ] Logowanie operacji
  - [ ] Przechowywanie historii zmian

- [ ] `AuditLogRepository`
  - [ ] Zapisywanie logów audytowych
  - [ ] Pobieranie logów audytowych

## API (KeepItUp.MagJob.Identity.Web)

### Endpointy FastEndpoints

#### Użytkownicy

- [ ] `GetUserEndpoint`
- [ ] `UpdateUserEndpoint`
- [ ] `GetUserOrganizationsEndpoint`

#### Organizacje

- [ ] `CreateOrganizationEndpoint`
- [ ] `GetOrganizationEndpoint`
- [ ] `UpdateOrganizationEndpoint`
- [ ] `DeleteOrganizationEndpoint`
- [ ] `GetOrganizationMembersEndpoint`
- [ ] `CreateInvitationEndpoint`
- [ ] `GetInvitationsEndpoint`
- [ ] `AcceptInvitationEndpoint`
- [ ] `RejectInvitationEndpoint`
- [ ] `RemoveMemberEndpoint`

#### Role i Uprawnienia

- [ ] `CreateRoleEndpoint`
- [ ] `GetRoleEndpoint`
- [ ] `UpdateRoleEndpoint`
- [ ] `DeleteRoleEndpoint`
- [ ] `GetRolesEndpoint`
- [ ] `UpdateRolePermissionsEndpoint`
- [ ] `AssignRoleToMemberEndpoint`
- [ ] `RevokeRoleFromMemberEndpoint`
- [ ] `GetPermissionsEndpoint`

### Konfiguracja

- [x] `Program.cs`
  - [x] Konfiguracja FastEndpoints
  - [x] Konfiguracja uwierzytelniania JWT
  - [x] Konfiguracja Keycloak
  - [x] Konfiguracja Swagger

- [x] Middleware
  - [x] `ErrorHandlingMiddleware` (z FastEndpoints)
  - [x] Konfiguracja uwierzytelniania i autoryzacji
  - [ ] `AuditMiddleware`

## Testy

### Testy Jednostkowe

- [ ] Testy dla modeli domeny
  - [ ] `UserTests`
  - [ ] `OrganizationTests`
  - [ ] `RoleTests`

- [ ] Testy dla przypadków użycia
  - [ ] Testy dla komend
  - [ ] Testy dla zapytań

### Testy Integracyjne

- [ ] Testy dla repozytoriów
- [ ] Testy dla endpointów API
- [ ] Testy dla integracji z Keycloak

## Dokumentacja

- [ ] Dokumentacja API (Swagger)
- [ ] Dokumentacja integracji z Keycloak
- [ ] Dokumentacja modelu domeny
- [ ] Dokumentacja przypadków użycia

## Etapy Implementacji

### Etap 1: Podstawowa Struktura

- [x] Implementacja modeli domeny
  - [x] Wykorzystanie Ardalis.SharedKernel dla klas bazowych
  - [x] Implementacja możliwości posiadania wielu ról przez członka organizacji
  - [x] Hybrydowe podejście do aktualizacji pól CreatedAt i UpdatedAt
  - [x] Implementacja UserProfile jako prawdziwego Value Object
- [x] Implementacja wzorca Specification
  - [x] Utworzenie specyfikacji dla User, Organization, Member i Role
  - [x] Wykorzystanie generycznych interfejsów IRepository<T> i IReadRepository<T>
- [x] Konfiguracja bazy danych
  - [x] Implementacja AppDbContext
  - [x] Automatyczna aktualizacja pól CreatedAt i UpdatedAt
  - [x] Konfiguracje encji (UserConfiguration, OrganizationConfiguration, MemberConfiguration, RoleConfiguration, InvitationConfiguration)
  - [x] Ujednolicenie typów danych dla identyfikatorów (int vs Guid)
  - [x] Konfiguracja dla PostgreSQL
  - [ ] Migracje
- [x] Obsługa zdarzeń domenowych
  - [x] Wykorzystanie MediatRDomainEventDispatcher z Ardalis.SharedKernel

### Etap 2: Integracja z Keycloak

- [x] Implementacja klienta Keycloak
- [x] Konfiguracja uwierzytelniania JWT
- [x] Implementacja synchronizacji danych
- [x] Optymalizacja zapytań do bazy danych
- [x] Obsługa błędów i logowania

### Etap 3: Przypadki Użycia i Endpointy

- [ ] Implementacja przypadków użycia dla użytkowników
- [ ] Implementacja przypadków użycia dla organizacji
- [ ] Implementacja przypadków użycia dla ról i uprawnień
- [ ] Implementacja endpointów FastEndpoints

### Etap 4: Audyt i Bezpieczeństwo

- [ ] Implementacja serwisu audytu
- [ ] Implementacja kontroli dostępu na podstawie ról i uprawnień
- [ ] Implementacja walidacji danych

### Etap 5: Testy i Dokumentacja

- [ ] Implementacja testów jednostkowych
- [ ] Implementacja testów integracyjnych
- [ ] Dokumentacja API (Swagger)

## Śledzenie Postępu

| Etap | Rozpoczęcie | Zakończenie | Status | Uwagi |
|------|-------------|-------------|--------|-------|
| Etap 1 | 09.03.2025 | 09.03.2025 | Zakończony | Zaimplementowano modele domeny. Wykorzystano Ardalis.SharedKernel dla klas bazowych. Zmodyfikowano klasę Member, aby mogła posiadać wiele ról. Zaimplementowano hybrydowe podejście do aktualizacji pól CreatedAt i UpdatedAt. Przekształcono UserProfile w prawdziwy Value Object. Zaimplementowano obsługę zdarzeń domenowych. Zastąpiono dedykowane repozytoria wzorcem Specification. Utworzono konfiguracje encji. Rozwiązano problem z niespójnością typów danych dla identyfikatorów (int vs Guid). Skonfigurowano bazę danych PostgreSQL. Utworzono migrację. |
| Etap 2 | 09.03.2025 | 10.03.2025 | Zakończony | Zaimplementowano integrację z Keycloak. Utworzono klienta Keycloak do komunikacji z API. Zaimplementowano serwis synchronizacji danych między modułem Identity a Keycloak. Dodano nasłuchiwanie zdarzeń z Keycloak. Zaimplementowano mapowanie atrybutów użytkownika do tokenów JWT. Skonfigurowano uwierzytelnianie JWT z Keycloak. Zoptymalizowano zapytania do bazy danych poprzez eager loading. Dodano obsługę błędów i logowanie. |
| Etap 3 | | | Nie rozpoczęto | |
| Etap 4 | | | Nie rozpoczęto | |
| Etap 5 | | | Nie rozpoczęto | |

## Zmiany i Aktualizacje

| Data | Autor | Opis zmiany |
|------|-------|-------------|
| 09.03.2025 | Claude | Utworzenie planu implementacji |
| 09.03.2025 | Claude | Implementacja modeli domeny i interfejsów repozytoriów |
| 09.03.2025 | Claude | Wykorzystanie Ardalis.SharedKernel dla klas bazowych |
| 09.03.2025 | Claude | Modyfikacja klasy Member, aby mogła posiadać wiele ról |
| 09.03.2025 | Claude | Implementacja hybrydowego podejścia do aktualizacji pól CreatedAt i UpdatedAt |
| 09.03.2025 | Claude | Przekształcenie UserProfile w prawdziwy Value Object |
| 09.03.2025 | Claude | Implementacja obsługi zdarzeń domenowych (wykorzystanie MediatRDomainEventDispatcher z Ardalis.SharedKernel) |
| 09.03.2025 | Claude | Zastąpienie dedykowanych repozytoriów wzorcem Specification |
| 09.03.2025 | Claude | Utworzenie konfiguracji encji (UserConfiguration, OrganizationConfiguration, MemberConfiguration, RoleConfiguration, InvitationConfiguration) |
| 09.03.2025 | Claude | Identyfikacja problemu z niespójnością typów danych dla identyfikatorów (int vs Guid) |
| 09.03.2025 | Claude | Rozwiązanie problemu z niespójnością typów danych dla identyfikatorów (int vs Guid) |
| 09.03.2025 | Claude | Konfiguracja bazy danych PostgreSQL |
| 09.03.2025 | Claude | Usunięcie SQLite z projektu i pełne przejście na PostgreSQL |
| 09.03.2025 | Claude | Rozwiązanie ostrzeżeń EF Core dotyczących komparatora wartości dla kolekcji i opcjonalnej zależności |
| 09.03.2025 | Claude | Implementacja integracji z Keycloak (KeycloakClient, KeycloakSyncService, KeycloakEventListener, KeycloakAttributeMapper) |
| 09.03.2025 | Claude | Konfiguracja uwierzytelniania JWT z Keycloak |
| 09.03.2025 | Claude | Integracja Swagger z uwierzytelnianiem Keycloak |
| 10.03.2025 | Claude | Optymalizacja zapytań do bazy danych poprzez eager loading |
| 10.03.2025 | Claude | Dodanie obsługi błędów i logowania w serwisach Keycloak |
| 10.03.2025 | Claude | Konfiguracja autoryzacji w aplikacji | 