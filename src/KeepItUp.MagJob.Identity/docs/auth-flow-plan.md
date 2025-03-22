# Plan przepływu uwierzytelniania i autoryzacji

Ten dokument opisuje plan przepływu logowania, rejestracji oraz aktualizacji ról użytkownika w kontekście integracji z Keycloak, a także sposób odwzorowania ról użytkownika w tokenie JWT.

## 1. Generowanie tokenów

Token JWT będzie tworzony bezpośrednio przez Keycloak, który jest dedykowanym serwerem uwierzytelniania i autoryzacji. Moduł Identity będzie odpowiedzialny za synchronizację danych między własną bazą danych a Keycloak, aby zapewnić spójność informacji o użytkownikach, rolach i uprawnieniach.

## 2. Proces rejestracji użytkownika

### Flow rejestracji:

1. **Rejestracja w Keycloak**:
   - Użytkownik wypełnia formularz rejestracyjny w aplikacji klienckiej
   - Aplikacja wysyła żądanie rejestracji do Keycloak (bezpośrednio lub przez API Gateway)
   - Keycloak tworzy konto użytkownika i generuje unikalny identyfikator (externalId)

2. **Synchronizacja z modułem Identity**:
   - Keycloak wysyła zdarzenie o utworzeniu nowego użytkownika
   - `KeycloakEventListener` w module Identity przechwytuje to zdarzenie
   - Moduł Identity tworzy odpowiadający rekord użytkownika w swojej bazie danych, używając `CreateUserCommand`
   - Użytkownik otrzymuje podstawowe role systemowe

3. **Potwierdzenie rejestracji**:
   - Keycloak wysyła email weryfikacyjny do użytkownika
   - Po weryfikacji, status użytkownika jest aktualizowany zarówno w Keycloak, jak i w module Identity

## 3. Proces logowania użytkownika

### Flow logowania:

1. **Uwierzytelnianie w Keycloak**:
   - Użytkownik wprowadza dane logowania w aplikacji klienckiej
   - Aplikacja wysyła żądanie uwierzytelnienia do Keycloak
   - Keycloak weryfikuje dane logowania

2. **Generowanie tokenu JWT**:
   - Po pomyślnej weryfikacji, Keycloak generuje token JWT
   - Token zawiera podstawowe informacje o użytkowniku (sub, email, name)
   - Token zawiera również role i uprawnienia użytkownika jako claims

3. **Przekazanie tokenu do aplikacji**:
   - Token JWT jest przekazywany do aplikacji klienckiej
   - Aplikacja przechowuje token i używa go do autoryzacji żądań do API

4. **Weryfikacja tokenu w API**:
   - Każde żądanie do API zawiera token JWT w nagłówku Authorization
   - API Gateway lub poszczególne mikrousługi weryfikują token
   - Weryfikacja obejmuje sprawdzenie podpisu, daty ważności oraz innych claims

## 4. Aktualizacja ról użytkownika i odwzorowanie w tokenie

### Flow aktualizacji ról:

1. **Przypisanie roli w module Identity**:
   - Administrator przypisuje rolę użytkownikowi w organizacji poprzez `AssignRoleToMemberCommand`
   - Moduł Identity aktualizuje relacje między użytkownikiem a rolami w swojej bazie danych
   - Generowane jest zdarzenie domenowe `MemberRoleAssignedEvent`

2. **Synchronizacja z Keycloak**:
   - Handler zdarzenia `MemberRoleAssignedEvent` wywołuje `KeycloakSyncService`
   - `KeycloakSyncService` aktualizuje role użytkownika w Keycloak poprzez `KeycloakClient`
   - Role są mapowane do formatu zrozumiałego dla Keycloak przez `KeycloakAttributeMapper`

3. **Odwzorowanie ról w tokenie**:
   - Przy kolejnym logowaniu lub odświeżeniu tokenu, Keycloak uwzględnia zaktualizowane role
   - Role są dodawane do tokenu JWT jako claims w sekcji payload
   - Format claims dla ról może być dostosowany przez `KeycloakAttributeMapper`

4. **Struktura claims w tokenie**:
   - Role systemowe: `realm_access.roles` (np. ["user", "admin"])
   - Role w organizacjach: `resource_access.[client-id].roles` (np. {"org-123": ["owner", "editor"]})
   - Uprawnienia: `permissions` (np. ["users:read", "organizations:write"])

## 5. Odświeżanie tokenu

1. **Wygaśnięcie tokenu dostępowego**:
   - Token JWT ma ograniczony czas ważności (np. 15 minut)
   - Aplikacja kliencka wykrywa zbliżające się wygaśnięcie tokenu

2. **Użycie tokenu odświeżającego**:
   - Aplikacja używa tokenu odświeżającego do uzyskania nowego tokenu dostępowego
   - Żądanie jest wysyłane do Keycloak
   - Keycloak weryfikuje token odświeżający i generuje nowy token dostępowy

3. **Aktualizacja ról w nowym tokenie**:
   - Nowy token zawiera aktualne role i uprawnienia użytkownika
   - Jeśli role zostały zmienione od czasu ostatniego logowania, nowy token będzie zawierał zaktualizowane informacje

## 6. Implementacja techniczna

### Komponenty po stronie Keycloak:

1. **Konfiguracja Realm**:
   - Utworzenie dedykowanego realm dla aplikacji
   - Konfiguracja protokołów (OpenID Connect)
   - Ustawienie polityk haseł i sesji

2. **Konfiguracja Client**:
   - Utworzenie klienta dla aplikacji webowej
   - Konfiguracja dozwolonych URL przekierowań
   - Ustawienie mapowania tokenów

3. **Mapowanie atrybutów**:
   - Konfiguracja Protocol Mappers dla tokenów JWT
   - Mapowanie ról i uprawnień do claims w tokenie
   - Dostosowanie formatu claims dla specyficznych potrzeb aplikacji

### Komponenty po stronie modułu Identity:

1. **KeycloakClient**:
   - Komunikacja z API Keycloak
   - Zarządzanie użytkownikami, rolami i grupami
   - Obsługa tokenów i sesji

2. **KeycloakSyncService**:
   - Synchronizacja danych między modułem Identity a Keycloak
   - Mapowanie modeli domenowych do modeli Keycloak
   - Obsługa konfliktów i błędów synchronizacji

3. **KeycloakEventListener**:
   - Nasłuchiwanie zdarzeń z Keycloak
   - Reagowanie na zdarzenia związane z użytkownikami
   - Aktualizacja lokalnej bazy danych na podstawie zdarzeń

4. **KeycloakAttributeMapper**:
   - Mapowanie ról i uprawnień do atrybutów w tokenach JWT
   - Dostosowanie formatu claims dla specyficznych potrzeb aplikacji
   - Obsługa złożonych struktur uprawnień

## 7. Bezpieczeństwo i audyt

1. **Zabezpieczenie komunikacji**:
   - Cała komunikacja między komponentami odbywa się przez HTTPS
   - Tokeny są przesyłane bezpiecznie i weryfikowane kryptograficznie

2. **Audyt operacji**:
   - Logowanie wszystkich operacji związanych z uwierzytelnianiem i autoryzacją
   - Śledzenie zmian w rolach i uprawnieniach użytkowników
   - Monitorowanie nieudanych prób logowania i podejrzanych działań

3. **Obsługa wyjątków**:
   - Graceful degradation w przypadku niedostępności Keycloak
   - Retry mechanizmy dla operacji synchronizacji
   - Powiadomienia o błędach krytycznych

# Checklista implementacyjna

## Keycloak

- [ ] **Konfiguracja Realm**
  - [ ] Utworzenie dedykowanego realm dla aplikacji (magjob-realm)
  - [ ] Konfiguracja protokołów (OpenID Connect)
  - [ ] Ustawienie polityk haseł i sesji
  - [ ] Konfiguracja wyglądu logowania i rejestracji

- [ ] **Konfiguracja Client**
  - [ ] Utworzenie klienta dla aplikacji webowej (keepitup.magjob.client)
  - [ ] Konfiguracja dozwolonych URL przekierowań
  - [ ] Ustawienie mapowania tokenów
  - [ ] Konfiguracja czasu życia tokenów

- [ ] **Konfiguracja ról**
  - [ ] Utworzenie ról systemowych (user, admin)
  - [ ] Konfiguracja ról domyślnych dla nowych użytkowników

- [ ] **Konfiguracja mapowania atrybutów**
  - [ ] Utworzenie mapperów dla ról i uprawnień
  - [ ] Konfiguracja mapowania atrybutów użytkownika do tokenów

- [ ] **Konfiguracja webhooków**
  - [ ] Konfiguracja webhooków dla zdarzeń użytkowników
  - [ ] Konfiguracja webhooków dla zdarzeń ról

## Moduł Identity

- [ ] **KeycloakClient**
  - [ ] Implementacja metod do zarządzania użytkownikami
  - [ ] Implementacja metod do zarządzania rolami
  - [ ] Implementacja metod do zarządzania atrybutami
  - [ ] Obsługa tokenów i uwierzytelniania

- [ ] **KeycloakSyncService**
  - [ ] Implementacja synchronizacji użytkowników
  - [ ] Implementacja synchronizacji ról
  - [ ] Implementacja synchronizacji uprawnień
  - [ ] Obsługa konfliktów i błędów synchronizacji

- [ ] **KeycloakEventListener**
  - [ ] Implementacja nasłuchiwania zdarzeń z Keycloak
  - [ ] Implementacja obsługi zdarzeń rejestracji użytkowników
  - [ ] Implementacja obsługi zdarzeń aktualizacji użytkowników
  - [ ] Implementacja obsługi zdarzeń logowania użytkowników

- [ ] **KeycloakAttributeMapper**
  - [ ] Implementacja mapowania ról do atrybutów w tokenach
  - [ ] Implementacja mapowania uprawnień do atrybutów w tokenach
  - [ ] Implementacja transformacji claimów

- [ ] **Handlery zdarzeń domenowych**
  - [ ] Implementacja handlera dla MemberRoleAssignedEvent
  - [ ] Implementacja handlera dla MemberRoleRevokedEvent
  - [ ] Implementacja handlera dla UserCreatedEvent
  - [ ] Implementacja handlera dla UserUpdatedEvent

- [ ] **Middleware uwierzytelniania i autoryzacji**
  - [ ] Konfiguracja middleware uwierzytelniania JWT
  - [ ] Konfiguracja middleware autoryzacji na podstawie ról
  - [ ] Konfiguracja middleware autoryzacji na podstawie uprawnień

## Aplikacja kliencka (Angular)

- [ ] **Konfiguracja biblioteki angular-oauth2-oidc**
  - [ ] Konfiguracja połączenia z Keycloak
  - [ ] Konfiguracja automatycznego odświeżania tokenów
  - [ ] Konfiguracja obsługi wygaśnięcia sesji

- [ ] **Implementacja interceptora tokenów**
  - [ ] Dodawanie tokenu do żądań HTTP
  - [ ] Obsługa błędów uwierzytelniania
  - [ ] Obsługa odświeżania tokenów

- [ ] **Implementacja guard'ów**
  - [ ] Implementacja AuthGuard do ochrony tras
  - [ ] Implementacja RoleGuard do ochrony tras na podstawie ról
  - [ ] Implementacja PermissionGuard do ochrony tras na podstawie uprawnień

- [ ] **Implementacja komponentów uwierzytelniania**
  - [ ] Implementacja komponentu logowania
  - [ ] Implementacja komponentu rejestracji
  - [ ] Implementacja komponentu resetowania hasła
  - [ ] Implementacja komponentu profilu użytkownika

- [ ] **Implementacja serwisów uwierzytelniania**
  - [ ] Implementacja serwisu uwierzytelniania
  - [ ] Implementacja serwisu zarządzania użytkownikami
  - [ ] Implementacja serwisu zarządzania rolami
  - [ ] Implementacja serwisu zarządzania uprawnieniami

## API Gateway

- [ ] **Konfiguracja uwierzytelniania**
  - [ ] Konfiguracja weryfikacji tokenów JWT
  - [ ] Konfiguracja przekazywania tokenów do mikrousług
  - [ ] Konfiguracja obsługi błędów uwierzytelniania

- [ ] **Konfiguracja autoryzacji**
  - [ ] Konfiguracja autoryzacji na podstawie ról
  - [ ] Konfiguracja autoryzacji na podstawie uprawnień
  - [ ] Konfiguracja obsługi błędów autoryzacji

## Testy

- [ ] **Testy jednostkowe**
  - [ ] Testy dla KeycloakClient
  - [ ] Testy dla KeycloakSyncService
  - [ ] Testy dla KeycloakEventListener
  - [ ] Testy dla KeycloakAttributeMapper

- [ ] **Testy integracyjne**
  - [ ] Testy dla przepływu rejestracji
  - [ ] Testy dla przepływu logowania
  - [ ] Testy dla przepływu aktualizacji ról
  - [ ] Testy dla przepływu odświeżania tokenów

- [ ] **Testy end-to-end**
  - [ ] Testy dla przepływu rejestracji użytkownika
  - [ ] Testy dla przepływu logowania użytkownika
  - [ ] Testy dla przepływu zarządzania rolami
  - [ ] Testy dla przepływu zarządzania uprawnieniami

## Dokumentacja

- [ ] **Dokumentacja dla deweloperów**
  - [ ] Dokumentacja konfiguracji Keycloak
  - [ ] Dokumentacja integracji z Keycloak
  - [ ] Dokumentacja przepływu uwierzytelniania i autoryzacji
  - [ ] Dokumentacja struktury tokenów JWT

- [ ] **Dokumentacja dla administratorów**
  - [ ] Dokumentacja konfiguracji Keycloak
  - [ ] Dokumentacja zarządzania użytkownikami
  - [ ] Dokumentacja zarządzania rolami i uprawnieniami
  - [ ] Dokumentacja monitorowania i audytu

- [ ] **Dokumentacja dla użytkowników**
  - [ ] Dokumentacja procesu rejestracji
  - [ ] Dokumentacja procesu logowania
  - [ ] Dokumentacja zarządzania profilem użytkownika
  - [ ] Dokumentacja zarządzania organizacjami i rolami 