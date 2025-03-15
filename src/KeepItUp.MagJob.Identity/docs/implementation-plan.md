# Plan Implementacji Modułu Identity

Ten dokument opisuje plan implementacji modułu Identity, w tym integrację z Keycloak oraz logikę dla użytkowników, organizacji, ról i uprawnień.

## Przegląd

Plan implementacji jest podzielony na pięć głównych etapów:
1. Podstawowa Struktura ✅
2. Integracja z Keycloak ✅
3. Przypadki Użycia i Endpointy ✅
4. Audyt i Bezpieczeństwo
5. Testy i Dokumentacja

## Modele Domeny (KeepItUp.MagJob.Identity.Core)

### Agregaty i Encje

#### UserAggregate ✅

- [x] `User` (Aggregate Root)
- [x] `UserProfile` (Value Object)
- [ ] `ExternalIdentity` (Value Object dla integracji z Keycloak)

#### OrganizationAggregate ✅

- [x] `Organization` (Aggregate Root)
- [x] `Member` (Entity)
- [x] `Invitation` (Entity)
- [x] `Role` (Entity)
- [x] `Permission` (Value Object)

#### ContributorAggregate ✅

- [x] `Contributor` (Aggregate Root)
- [x] `ContributorStatus` (Smart Enum)
- [x] `PhoneNumber` (Value Object)

### Zdarzenia Domenowe ✅

- [x] Zdarzenia dla UserAggregate
- [x] Zdarzenia dla OrganizationAggregate
- [x] Zdarzenia dla ContributorAggregate

### Specyfikacje i Repozytoria ✅

- [x] Wykorzystanie wzorca Specification zamiast dedykowanych repozytoriów
- [x] Wykorzystanie generycznych interfejsów repozytorium z Ardalis.Specification

## Przypadki Użycia (KeepItUp.MagJob.Identity.UseCases) ✅

### Użytkownicy ✅

- [x] Komendy (Create, Update, Deactivate)
- [x] Zapytania (GetById, GetByExternalId, GetUserOrganizations)

### Organizacje ✅

- [x] Komendy (Create, Update, Deactivate, CreateInvitation, AcceptInvitation, RejectInvitation, RemoveMember)
- [x] Zapytania (GetById, GetMembers, GetInvitations, GetMemberById, GetInvitationById)

### Role i Uprawnienia ✅

- [x] Komendy (CreateRole, UpdateRole, DeleteRole, AssignRoleToMember, RevokeRoleFromMember, UpdateRolePermissions)
- [x] Zapytania (GetRoleById, GetRolesByOrganizationId, GetRolesByMemberId, GetPermissions)

### Kontrybutorzy ✅

- [x] Komendy (Create, Update, Delete)
- [x] Zapytania (GetById, List)

## Infrastruktura (KeepItUp.MagJob.Identity.Infrastructure)

### Baza Danych ✅

- [x] `AppDbContext`
- [x] Konfiguracje encji
  - [x] `UserConfiguration`
  - [x] `OrganizationConfiguration`
  - [x] `MemberConfiguration`
  - [x] `RoleConfiguration`
  - [x] `InvitationConfiguration`
  - [x] `ContributorConfiguration`
  - [ ] `AuditLogConfiguration`

- [x] Migracje
  - [x] Inicjalna migracja
  - [x] Migracja PhoneNumber
  - [ ] Seed danych (uprawnienia, role systemowe)

- [x] Implementacje repozytoriów

### Obsługa Zdarzeń Domenowych ✅

- [x] `IDomainEventDispatcher`
- [x] `MediatRDomainEventDispatcher`

### Integracja z Keycloak ✅

- [x] `KeycloakClient`
- [x] `KeycloakOptions`
- [x] `KeycloakSyncService`
- [x] `KeycloakEventListener`
- [x] `KeycloakAttributeMapper`

### Email ✅

- [x] Serwis do wysyłania emaili

### Audyt

- [ ] `AuditService`
  - [ ] Logowanie operacji
  - [ ] Przechowywanie historii zmian

- [ ] `AuditLogRepository`
  - [ ] Zapisywanie logów audytowych
  - [ ] Pobieranie logów audytowych

## API (KeepItUp.MagJob.Identity.Web) ✅

### Endpointy FastEndpoints

#### Użytkownicy ✅

- [x] `GetUserEndpoint`
- [x] `UpdateUserEndpoint`
- [x] `GetUserOrganizationsEndpoint`

#### Organizacje ✅

- [x] `CreateOrganizationEndpoint`
- [x] `GetOrganizationEndpoint`
- [x] `UpdateOrganizationEndpoint`
- [x] `DeleteOrganizationEndpoint`
- [x] `GetOrganizationMembersEndpoint`
- [x] `CreateInvitationEndpoint`
- [x] `GetOrganizationInvitationsEndpoint`
- [x] `AcceptInvitationEndpoint`
- [x] `RejectInvitationEndpoint`
- [x] `RemoveMemberEndpoint`

#### Role i Uprawnienia ✅

- [x] `CreateRoleEndpoint`
- [x] `GetOrganizationRolesEndpoint`
- [x] `UpdateRoleEndpoint`
- [x] `DeleteRoleEndpoint`
- [x] `UpdateRolePermissionsEndpoint`
- [x] `AssignRoleToMemberEndpoint`
- [x] `RevokeRoleFromMemberEndpoint`
- [x] `GetPermissionsEndpoint`

#### Kontrybutorzy ✅

- [x] `CreateContributorEndpoint`
- [x] `GetContributorByIdEndpoint`
- [x] `UpdateContributorEndpoint`
- [x] `DeleteContributorEndpoint`
- [x] `ListContributorsEndpoint`

### Konfiguracja ✅

- [x] `Program.cs`
- [x] Middleware
  - [x] `ErrorHandlingMiddleware` (z FastEndpoints)
  - [x] Konfiguracja uwierzytelniania i autoryzacji
  - [ ] `AuditMiddleware`

## Testy

### Testy Jednostkowe

- [x] Testy dla modeli domeny
  - [x] `ContributorTests`
  - [ ] `UserTests`
  - [ ] `OrganizationTests`
  - [ ] `RoleTests`

- [ ] Testy dla przypadków użycia
  - [ ] Testy dla komend
  - [ ] Testy dla zapytań

### Testy Integracyjne

- [x] Testy dla repozytoriów
  - [x] `EfRepositoryAdd`
  - [x] `EfRepositoryUpdate`
  - [x] `EfRepositoryDelete`
- [ ] Testy dla endpointów API
- [ ] Testy dla integracji z Keycloak

### Testy Funkcjonalne

- [x] Konfiguracja testów funkcjonalnych
  - [x] `CustomWebApplicationFactory`
- [x] Testy dla endpointów API
  - [x] `ContributorGetById`
  - [x] `ContributorList`
  - [ ] Testy dla pozostałych endpointów

## Dokumentacja

- [ ] Dokumentacja API (Swagger)
- [ ] Dokumentacja integracji z Keycloak
- [ ] Dokumentacja modelu domeny
- [ ] Dokumentacja przypadków użycia

## Etapy Implementacji

### Etap 1: Podstawowa Struktura ✅

- [x] Implementacja modeli domeny
- [x] Implementacja wzorca Specification
- [x] Konfiguracja bazy danych
- [x] Obsługa zdarzeń domenowych

### Etap 2: Integracja z Keycloak ✅

- [x] Implementacja klienta Keycloak
- [x] Konfiguracja uwierzytelniania JWT
- [x] Implementacja synchronizacji danych
- [x] Optymalizacja zapytań do bazy danych
- [x] Obsługa błędów i logowania

### Etap 3: Przypadki Użycia i Endpointy ✅

- [x] Implementacja przypadków użycia dla użytkowników
- [x] Implementacja przypadków użycia dla organizacji
- [x] Implementacja przypadków użycia dla ról i uprawnień
- [x] Implementacja przypadków użycia dla kontrybutorów
- [x] Implementacja endpointów FastEndpoints

### Etap 4: Audyt i Bezpieczeństwo

- [ ] Implementacja serwisu audytu
- [ ] Implementacja kontroli dostępu na podstawie ról i uprawnień
- [ ] Implementacja walidacji danych

### Etap 5: Testy i Dokumentacja

- [x] Implementacja podstawowych testów jednostkowych
- [x] Implementacja podstawowych testów integracyjnych
- [x] Implementacja podstawowych testów funkcjonalnych
- [ ] Rozszerzenie pokrycia testami
- [ ] Dokumentacja API (Swagger)

## Śledzenie Postępu

| Etap | Rozpoczęcie | Zakończenie | Status | Uwagi |
|------|-------------|-------------|--------|-------|
| Etap 1 | 09.03.2025 | 09.03.2025 | Zakończony | Zaimplementowano modele domeny, wzorzec Specification, konfigurację bazy danych i obsługę zdarzeń domenowych. |
| Etap 2 | 09.03.2025 | 10.03.2025 | Zakończony | Zaimplementowano integrację z Keycloak, uwierzytelnianie JWT, synchronizację danych i optymalizację zapytań. |
| Etap 3 | 11.03.2025 | 11.03.2025 | Zakończony | Zaimplementowano przypadki użycia i endpointy dla użytkowników, organizacji, ról, uprawnień i kontrybutorów. |
| Etap 4 | | | Nie rozpoczęto | |
| Etap 5 | 12.03.2025 | | W trakcie | Zaimplementowano podstawowe testy jednostkowe, integracyjne i funkcjonalne. Konieczne rozszerzenie pokrycia testami i dokumentacja. |

## Zmiany i Aktualizacje

| Data | Autor | Opis |
|------|-------|------|
| 09.03.2025 | Claude | Utworzenie planu implementacji |
| 09.03.2025 | Claude | Implementacja modeli domeny i infrastruktury |
| 10.03.2025 | Claude | Implementacja integracji z Keycloak |
| 11.03.2025 | Claude | Implementacja przypadków użycia i endpointów |
| 12.03.2025 | Claude | Implementacja podstawowych testów |
| 12.03.2025 | Claude | Dodanie modelu ContributorAggregate |
| 12.03.2025 | Claude | Aktualizacja planu implementacji na podstawie aktualnego stanu projektu | 