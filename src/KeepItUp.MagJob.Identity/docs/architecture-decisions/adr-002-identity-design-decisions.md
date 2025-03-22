# ADR 002: Kluczowe Decyzje Projektowe dla Modułu Identity

## Status

Zaproponowany

## Kontekst

Moduł Identity jest odpowiedzialny za zarządzanie tożsamością użytkowników, organizacjami, rolami i uprawnieniami w systemie MagJob. Potrzebujemy podjąć kluczowe decyzje projektowe dotyczące architektury, modelu danych i integracji z innymi komponentami systemu.

## Decyzje

### 1. Architektura

Zdecydowaliśmy się na zastosowanie Clean Architecture z podziałem na następujące warstwy:
- **Core** - zawiera encje domenowe, agregaty, value objects i logikę biznesową
- **UseCases** - zawiera przypadki użycia, implementację CQRS
- **Infrastructure** - zawiera implementacje repozytoriów, dostęp do bazy danych, zewnętrzne usługi
- **Web** - zawiera endpointy API, konfigurację aplikacji

### 2. Wzorce Projektowe

Zdecydowaliśmy się na zastosowanie następujących wzorców:
- **Domain-Driven Design (DDD)** - dla modelowania domeny biznesowej
- **Command Query Responsibility Segregation (CQRS)** - dla rozdzielenia operacji odczytu i zapisu
- **Repository Pattern** - dla abstrakcji dostępu do danych
- **Mediator Pattern** - dla implementacji CQRS i komunikacji między komponentami
- **Result Pattern** - dla obsługi błędów biznesowych
- **FastEndpoints** - zamiast standardowych kontrolerów ASP.NET Core, dla bardziej zwięzłej i czytelnej implementacji API

### 3. Identyfikatory Encji

Zdecydowaliśmy się na użycie UUID (Guid) jako identyfikatorów encji z następujących powodów:
- Unikalność w skali globalnej
- Brak konieczności centralnej koordynacji przy generowaniu ID
- Łatwość integracji z innymi systemami
- Bezpieczeństwo (trudniejsze do zgadnięcia niż sekwencyjne ID)

### 4. Baza Danych

Zdecydowaliśmy się na użycie PostgreSQL z następującymi założeniami:
- Schemat `identity` dla wszystkich tabel modułu
- Migracje zarządzane przez Entity Framework Core
- Indeksy na często wyszukiwanych kolumnach (np. email użytkownika)
- Ograniczenia integralności (klucze obce, unikalne indeksy)

### 5. Uwierzytelnianie i Autoryzacja

Zdecydowaliśmy się na integrację z Keycloak jako zewnętrznym dostawcą tożsamości:
- Keycloak zarządza uwierzytelnianiem użytkowników i przechowuje dane wrażliwe
- Moduł Identity przechowuje dodatkowe dane użytkowników i zarządza organizacjami
- Dane o rolach i organizacjach są przekazywane do Keycloak, aby mogły być mapowane do tokenów JWT
- Autoryzacja oparta na rolach i uprawnieniach zdefiniowanych w module Identity
- JWT jako mechanizm uwierzytelniania między usługami
- Logowanie i rejestracja realizowane przez Keycloak (przekierowanie użytkownika do Keycloak)

### 6. Model Organizacji

Zdecydowaliśmy się na płaską strukturę organizacji:
- Brak hierarchii organizacyjnej (oddziały, departamenty)
- Złożoność organizacyjna realizowana przez system ról i uprawnień
- Każda organizacja może mieć wielu członków z różnymi rolami

### 7. Role i Uprawnienia

Zdecydowaliśmy się na następujący model ról i uprawnień:
- Możliwość definiowania własnych ról przez administratorów organizacji
- Jedna predefiniowana rola "everyone" dla wszystkich członków organizacji
- Uprawnienia definiowane według wykonywanych zadań (np. usuwanie członków, usuwanie organizacji, zarządzanie grafikami)
- Granularne uprawnienia pozwalające na precyzyjne kontrolowanie dostępu do funkcji systemu
- Uprawnienia przyznawane na podstawie kontekstu organizacji oraz roli użytkownika

### 8. Komunikacja Asynchroniczna

Zdecydowaliśmy się na publikowanie zdarzeń domenowych do kolejki wiadomości:
- Zdarzenia takie jak UserCreated, OrganizationCreated, UserAddedToOrganization
- Inne mikrousługi mogą subskrybować te zdarzenia i reagować na nie
- Zapewnia luźne powiązanie między mikrousługami

### 9. Walidacja

Zdecydowaliśmy się na wielopoziomową walidację:
- Walidacja na poziomie API za pomocą FluentValidation
- Walidacja na poziomie domeny za pomocą Guard Clauses
- Walidacja na poziomie bazy danych za pomocą ograniczeń

### 10. Obsługa Błędów

Zdecydowaliśmy się na następujące podejście do obsługi błędów:
- Używanie Result Pattern zamiast wyjątków do obsługi błędów biznesowych
- Globalna obsługa wyjątków na poziomie API
- Szczegółowe komunikaty błędów dla deweloperów, ogólne dla użytkowników
- Logowanie wszystkich błędów

### 11. Audyt

Zdecydowaliśmy się na śledzenie wszystkich operacji w systemie:
- Rejestrowanie wszystkich zmian w encjach (kto, kiedy, co zmienił)
- Przechowywanie historii zmian dla celów audytowych
- Możliwość generowania raportów audytowych

### 12. Wydajność i Skalowalność

System powinien obsługiwać:
- Około 50 000 użytkowników miesięcznie
- Około 5 000 organizacji
- Optymalizacja zapytań do bazy danych dla najczęściej wykonywanych operacji
- Możliwość skalowania poziomego w miarę wzrostu obciążenia

## Konsekwencje

### Pozytywne

- Czysta, modułowa architektura ułatwiająca testowanie i rozwój
- Jasne granice między warstwami aplikacji
- Skalowalność i elastyczność dzięki luźnemu powiązaniu komponentów
- Łatwość integracji z innymi mikrousługami
- Bezpieczeństwo dzięki wielopoziomowej walidacji i autoryzacji
- Pełna historia zmian dla celów audytowych
- Standardowy i bezpieczny przepływ uwierzytelniania dzięki Keycloak
- Bardziej zwięzła i czytelna implementacja API dzięki FastEndpoints

### Negatywne

- Większa złożoność początkowa
- Więcej kodu boilerplate
- Potencjalne wyzwania z synchronizacją danych między Keycloak a modułem Identity
- Konieczność zarządzania spójnością danych w modelu asynchronicznym
- Dodatkowe obciążenie związane z audytem wszystkich operacji
- Mniejsza kontrola nad interfejsem użytkownika logowania/rejestracji (Keycloak)

## Pytania Otwarte

1. Jak będziemy zarządzać migracjami użytkowników z istniejących systemów?
2. Jak będziemy obsługiwać scenariusze odzyskiwania hasła i weryfikacji email?
3. Jak będziemy zarządzać sesjami użytkowników w aplikacji frontendowej?
4. Jakie dokładnie będą wymagania dotyczące bezpieczeństwa (szyfrowanie danych, ochrona przed atakami)? 