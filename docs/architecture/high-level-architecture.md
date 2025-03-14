# Architektura Wysokiego Poziomu - MagJob

## Przegląd Systemu

MagJob to system wsparcia w rozwoju oraz komunikacji w małych i średnich firmach, który rozwiązuje problemy związane z komunikacją w zespole, zbieraniem dyspozycyjności oraz zarządzaniem pracownikami. System jest zaprojektowany jako aplikacja oparta na mikrousługach, z centralną aplikacją frontendową i API Gateway.

## Główne Komponenty Systemu

![Architektura Wysokiego Poziomu](../images/high-level-architecture.png)

### 1. Aplikacja Frontendowa

- **Technologia**: Angular v19
- **Struktura projektu**: `src/Client/`
- **Funkcjonalności**:
  - Interfejs użytkownika dla wszystkich funkcji systemu
  - Responsywny design działający na urządzeniach mobilnych i desktopowych
  - Własne komponenty UI
- **Komunikacja**: Komunikacja z backendem poprzez API Gateway

### 2. API Gateway

- **Struktura projektu**: `src/KeepItUp.MagJob.APIGateway/`
- **Funkcjonalności**:
  - Centralizacja dostępu do mikrousług
  - Routing zapytań do odpowiednich mikrousług
  - Uwierzytelnianie i autoryzacja zapytań
  - Limitowanie liczby zapytań
  - Logowanie i monitoring
- **Komunikacja**: REST API

### 3. Keycloak (Identity Provider)

- **Struktura projektu**: `src/Keycloak/`
- **Funkcjonalności**:
  - Zarządzanie tożsamością użytkowników
  - Uwierzytelnianie użytkowników
  - Zarządzanie sesjami
  - Integracja z zewnętrznymi dostawcami tożsamości (opcjonalnie)
- **Komunikacja**: OAuth 2.0 / OpenID Connect

### 4. Mikrousługi

#### 4.1. Serwis Tożsamości (.NET)

- **Struktura projektu**: `src/KeepItUp.MagJob.Identity/`
- **Funkcjonalności**:
  - Zarządzanie użytkownikami
  - Zarządzanie organizacjami
  - Zarządzanie zaproszeniami
  - Zarządzanie rolami i uprawnieniami
- **Baza danych**: PostgreSQL (schemat identity)
- **Komunikacja**: REST API, publikowanie zdarzeń do kolejek

#### 4.2. Serwis Zarządzania Dyspozycyjnością i Grafikami (Spring)

- **Struktura projektu**: `src/Schedules/`
- **Funkcjonalności**:
  - Zbieranie informacji o dyspozycyjności pracowników
  - Tworzenie i zarządzanie grafikami pracy
  - Powiadomienia o zmianach w grafiku
- **Baza danych**: PostgreSQL (schemat schedules)
- **Komunikacja**: REST API, konsumpcja i publikowanie zdarzeń do kolejek

#### 4.3. Serwis Ewidencji Czasu Pracy (Spring)

- **Struktura projektu**: `src/WorkEvidence/`
- **Funkcjonalności**:
  - Rejestracja czasu pracy
  - Generowanie raportów czasu pracy
  - Zarządzanie nieobecnościami
  - Integracja z grafikami
- **Baza danych**: PostgreSQL (schemat workevidence)
- **Komunikacja**: REST API, konsumpcja i publikowanie zdarzeń do kolejek

### 5. Kolejki Wiadomości

- **Funkcjonalności**:
  - Asynchroniczna komunikacja między mikrousługami
  - Zapewnienie niezawodności dostarczania wiadomości
  - Obsługa zdarzeń domenowych
- **Potencjalne zdarzenia**:
  - UserCreated, UserUpdated, UserDeleted
  - OrganizationCreated, OrganizationUpdated, OrganizationDeleted
  - ScheduleCreated, ScheduleUpdated
  - TimeEntryRecorded

### 6. Baza Danych

- **Technologia**: PostgreSQL
- **Struktura**: Jedna baza danych z oddzielnymi schematami dla każdej mikrousługi
- **Migracje**: Każda mikrousługa zarządza swoimi migracjami (EF Migrations dla .NET)

## Przepływy Danych

### Rejestracja Użytkownika

1. Użytkownik wypełnia formularz rejestracji w aplikacji frontendowej
2. Aplikacja frontendowa wysyła żądanie do API Gateway
3. API Gateway przekierowuje żądanie do Serwisu Użytkowników
4. Serwis Użytkowników tworzy konto użytkownika w bazie danych
5. Serwis Użytkowników publikuje zdarzenie UserCreated do kolejki
6. Keycloak tworzy konto użytkownika dla uwierzytelniania

### Tworzenie Grafiku

1. Manager wypełnia formularz tworzenia grafiku w aplikacji frontendowej
2. Aplikacja frontendowa wysyła żądanie do API Gateway
3. API Gateway przekierowuje żądanie do Serwisu Zarządzania Dyspozycyjnością i Grafikami (Schedules)
4. Serwis pobiera informacje o dyspozycyjności pracowników
5. Serwis tworzy grafik i zapisuje go w bazie danych
6. Serwis publikuje zdarzenie ScheduleCreated do kolejki
7. Serwis Ewidencji Czasu Pracy (WorkEvidence) konsumuje zdarzenie i aktualizuje swoje dane

## Skalowalność i Wydajność

System jest zaprojektowany z myślą o obsłudze do 50 000 użytkowników miesięcznie. Architektura mikrousługowa pozwala na niezależne skalowanie poszczególnych komponentów w zależności od obciążenia.

### Strategia Skalowania

- **Aplikacja Frontendowa**: Skalowanie poziome za pomocą wielu instancji za load balancerem
- **API Gateway**: Skalowanie poziome za pomocą wielu instancji
- **Mikrousługi**: Niezależne skalowanie każdej mikrousługi w zależności od obciążenia
- **Baza Danych**: Początkowo pojedyncza instancja, w przyszłości możliwość replikacji read-only

## Monitoring i Logowanie

- **OpenTelemetry**: Zbieranie metryk i śladów z mikrousług
- **Prometheus**: Przechowywanie i analiza metryk
- **Grafana**: Wizualizacja metryk i tworzenie dashboardów
- **Centralizowane logowanie**: Agregacja logów ze wszystkich komponentów

## Bezpieczeństwo

- **Uwierzytelnianie**: Keycloak (OAuth 2.0 / OpenID Connect)
- **Autoryzacja**: Role i uprawnienia zarządzane przez Serwis Użytkowników
- **Komunikacja**: HTTPS dla wszystkich połączeń zewnętrznych
- **Zabezpieczenia**: Ochrona przed CSRF, XSS, SQL Injection

## Środowiska

- **Lokalne**: Dla deweloperów, możliwość uruchomienia całego systemu w kontenerach Docker lub podłączenia lokalnie rozwijanego komponentu do reszty systemu w kontenerach
- **Dev**: Środowisko w Azure do testowania i integracji
- **Prod**: Środowisko produkcyjne w Azure

## Technologie

- **Frontend**: Angular v19
- **Backend**: .NET (Serwis Użytkowników), Spring (pozostałe mikrousługi)
- **Baza Danych**: PostgreSQL
- **Konteneryzacja**: Docker
- **Orkiestracja**: W przyszłości potencjalnie Kubernetes
- **CI/CD**: GitHub Actions
- **Dokumentacja API**: OpenAPI / Swagger
