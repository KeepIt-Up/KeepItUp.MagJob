# Strategia Bezpieczeństwa - MagJob

Ten dokument opisuje strategię bezpieczeństwa dla projektu MagJob, w tym uwierzytelnianie, autoryzację, ochronę danych i zabezpieczenia infrastruktury.

## Uwierzytelnianie

### Keycloak jako Identity Provider

- **Centralne zarządzanie tożsamością**: Keycloak będzie używany jako centralny system zarządzania tożsamością
- **Protokoły**: OAuth 2.0 i OpenID Connect
- **Funkcje**:
  - Rejestracja i logowanie użytkowników
  - Zarządzanie sesjami
  - Odzyskiwanie hasła
  - Uwierzytelnianie wieloskładnikowe (MFA)
  - Integracja z zewnętrznymi dostawcami tożsamości (opcjonalnie)

### Przepływ uwierzytelniania

1. Użytkownik próbuje uzyskać dostęp do aplikacji frontendowej
2. Aplikacja frontendowa przekierowuje użytkownika do Keycloak
3. Użytkownik uwierzytelnia się w Keycloak
4. Keycloak generuje token JWT i przekazuje go do aplikacji frontendowej
5. Aplikacja frontendowa przechowuje token i używa go do uwierzytelniania żądań do API Gateway
6. API Gateway weryfikuje token JWT i przekazuje żądanie do odpowiedniej mikrousługi

### Zarządzanie tokenami

- **Typ tokenów**: JWT (JSON Web Tokens)
- **Zawartość tokenów**:
  - Identyfikator użytkownika
  - Role i uprawnienia
  - Czas wygaśnięcia
  - Informacje o organizacji
- **Czas życia tokenów**:
  - Access Token: 15 minut
  - Refresh Token: 24 godziny
- **Odnawianie tokenów**: Automatyczne odnawianie za pomocą Refresh Tokena

## Autoryzacja

### Model uprawnień

- **Role w organizacji**: Każdy użytkownik ma przypisaną rolę w kontekście organizacji
- **Uprawnienia**: Każda rola ma przypisany zestaw uprawnień
- **Hierarchia ról**: Możliwość definiowania hierarchii ról w organizacji

### Domyślne role

- **Administrator**: Pełny dostęp do wszystkich funkcji organizacji
- **Manager**: Zarządzanie grafikami, zatwierdzanie czasu pracy, zarządzanie użytkownikami
- **Pracownik**: Podstawowe funkcje, takie jak przeglądanie grafików, zgłaszanie dyspozycyjności, rejestracja czasu pracy
- **Gość**: Ograniczony dostęp tylko do odczytu

### Kontrola dostępu

- **Kontrola dostępu na poziomie API Gateway**: Weryfikacja tokenów JWT i podstawowa autoryzacja
- **Kontrola dostępu na poziomie mikrousług**: Szczegółowa autoryzacja na podstawie uprawnień użytkownika
- **Kontrola dostępu na poziomie bazy danych**: Filtrowanie danych na podstawie przynależności do organizacji

## Ochrona danych

### Szyfrowanie danych

- **Dane w spoczynku**: Szyfrowanie bazy danych PostgreSQL
- **Dane w tranzycie**: HTTPS dla wszystkich połączeń zewnętrznych
- **Dane wrażliwe**: Dodatkowe szyfrowanie danych wrażliwych w bazie danych

### Zarządzanie sekretami

- **Azure Key Vault**: Przechowywanie kluczy, certyfikatów i innych sekretów
- **Zmienne środowiskowe**: Bezpieczne przekazywanie sekretów do aplikacji
- **Rotacja kluczy**: Regularna rotacja kluczy i sekretów

### Ochrona prywatności

- **Minimalizacja danych**: Zbieranie tylko niezbędnych danych osobowych
- **Anonimizacja**: Możliwość anonimizacji danych do celów analitycznych
- **Zgodność z RODO**: Implementacja mechanizmów wymaganych przez RODO
  - Prawo do bycia zapomnianym
  - Eksport danych użytkownika
  - Zgody na przetwarzanie danych

## Zabezpieczenia aplikacji

### Ochrona przed typowymi zagrożeniami

- **Cross-Site Scripting (XSS)**: 
  - Sanityzacja danych wejściowych i wyjściowych
  - Content Security Policy (CSP)
  - Używanie bezpiecznych frameworków (Angular)
  
- **Cross-Site Request Forgery (CSRF)**:
  - Tokeny anty-CSRF
  - Same-origin policy
  
- **SQL Injection**:
  - Parametryzowane zapytania
  - ORM (Entity Framework, Hibernate)
  - Walidacja danych wejściowych
  
- **Broken Authentication**:
  - Bezpieczne zarządzanie sesjami
  - Limity prób logowania
  - Silne polityki haseł
  
- **Insecure Direct Object References (IDOR)**:
  - Weryfikacja dostępu do zasobów
  - Używanie UUID zamiast inkrementalnych ID

### Walidacja danych

- **Walidacja po stronie klienta**: Podstawowa walidacja w aplikacji frontendowej
- **Walidacja po stronie serwera**: Dokładna walidacja wszystkich danych wejściowych
- **Sanityzacja danych**: Oczyszczanie danych wejściowych z potencjalnie niebezpiecznych elementów

## Bezpieczeństwo infrastruktury

### Sieć

- **Segmentacja sieci**: Oddzielenie różnych komponentów systemu
- **Firewall**: Ograniczenie dostępu do niezbędnych portów i usług
- **WAF (Web Application Firewall)**: Ochrona przed atakami na aplikację webową

### Monitorowanie bezpieczeństwa

- **Logowanie zdarzeń bezpieczeństwa**: Centralne zbieranie i analiza logów
- **Alerty bezpieczeństwa**: Powiadomienia o podejrzanych działaniach
- **Audyt dostępu**: Rejestrowanie wszystkich działań administracyjnych

### Aktualizacje i patche

- **Regularne aktualizacje**: Systematyczne aktualizowanie wszystkich komponentów
- **Zarządzanie podatnościami**: Skanowanie i naprawianie podatności
- **Dependency scanning**: Monitorowanie bezpieczeństwa zależności

## Procedury bezpieczeństwa

### Reagowanie na incydenty

- **Plan reagowania**: Zdefiniowany proces obsługi incydentów bezpieczeństwa
- **Zespół reagowania**: Określone role i odpowiedzialności
- **Komunikacja**: Procedury powiadamiania o incydentach

### Testy bezpieczeństwa

- **Regularne testy penetracyjne**: Okresowe testowanie zabezpieczeń
- **Skanowanie podatności**: Automatyczne skanowanie kodu i infrastruktury
- **Code review**: Przegląd kodu pod kątem bezpieczeństwa

### Szkolenia i świadomość

- **Szkolenia dla deweloperów**: Regularne szkolenia z bezpiecznego programowania
- **Dokumentacja**: Aktualna dokumentacja dotycząca bezpieczeństwa
- **Najlepsze praktyki**: Stosowanie uznanych standardów i najlepszych praktyk

## Zgodność i regulacje

### RODO/GDPR

- **Inwentaryzacja danych osobowych**: Identyfikacja wszystkich miejsc przetwarzania danych osobowych
- **Podstawy prawne**: Określenie podstaw prawnych przetwarzania
- **Prawa podmiotów danych**: Implementacja mechanizmów realizujących prawa osób, których dane dotyczą

### Audyt i certyfikacja

- **Regularne audyty**: Okresowe sprawdzanie zgodności z politykami bezpieczeństwa
- **Dokumentacja zgodności**: Utrzymywanie dokumentacji wymaganej przez regulacje
- **Certyfikacja**: Rozważenie certyfikacji bezpieczeństwa w przyszłości 
