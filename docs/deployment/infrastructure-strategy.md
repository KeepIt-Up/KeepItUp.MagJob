# Strategia Infrastruktury - MagJob

Ten dokument opisuje strategię infrastruktury dla projektu MagJob, w tym środowiska, konteneryzację, monitoring i zarządzanie konfiguracją.

## Środowiska

### Środowisko Lokalne (Development)

- **Cel**: Umożliwienie deweloperom lokalnego rozwoju i testowania komponentów
- **Konfiguracja**:
  - Docker Compose do uruchamiania wszystkich komponentów systemu
  - Możliwość uruchomienia pojedynczego komponentu lokalnie i podłączenia go do reszty systemu w kontenerach
  - Lokalna baza danych PostgreSQL w kontenerze
  - Lokalne instancje mikrousług
  - Lokalny Keycloak
- **Wymagania**:
  - Docker Desktop
  - Odpowiednie SDK (.NET, Java, Node.js)
  - IDE (Visual Studio, IntelliJ, VS Code)

### Środowisko Deweloperskie (Dev)

- **Cel**: Integracja i testowanie komponentów
- **Lokalizacja**: Azure
- **Konfiguracja**:
  - Kontenery Docker uruchamiane w Azure Container Instances lub App Service
  - Baza danych PostgreSQL w Azure Database for PostgreSQL
  - Keycloak w kontenerze
  - Monitoring i logowanie
- **Dostęp**: Ograniczony do zespołu deweloperskiego i testowego

### Środowisko Produkcyjne (Prod)

- **Cel**: Środowisko dla użytkowników końcowych
- **Lokalizacja**: Azure
- **Konfiguracja**:
  - Kontenery Docker uruchamiane w Azure Container Instances lub App Service
  - Baza danych PostgreSQL w Azure Database for PostgreSQL z konfiguracją wysokiej dostępności
  - Keycloak w kontenerze z konfiguracją wysokiej dostępności
  - Pełny monitoring i logowanie
- **Dostęp**: Publiczny dla użytkowników końcowych, ograniczony dostęp administracyjny

## Konteneryzacja

### Docker

- Wszystkie komponenty systemu będą konteneryzowane za pomocą Docker
- Każda mikrousługa będzie miała własny Dockerfile
- Obrazy będą budowane i publikowane w ramach procesu CI/CD
- Wersjonowanie obrazów będzie zgodne z wersjonowaniem aplikacji

### Docker Compose

- Docker Compose będzie używany do lokalnego uruchamiania całego systemu
- Plik docker-compose.yml będzie zawierał definicje wszystkich usług
- Plik docker-compose.override.yml będzie zawierał konfiguracje specyficzne dla środowiska lokalnego

### Przyszła Orkiestracja (Kubernetes)

- W przyszłości, gdy skala projektu wzrośnie, rozważymy migrację do Kubernetes
- Azure Kubernetes Service (AKS) będzie preferowanym rozwiązaniem
- Helm Charts będą używane do definiowania i wdrażania aplikacji w Kubernetes

## Usługi Azure

### Azure Container Instances (ACI)

- Używane do uruchamiania kontenerów Docker w środowisku deweloperskim
- Łatwe wdrażanie i zarządzanie
- Integracja z Azure Monitor

### Azure App Service

- Alternatywa dla ACI, oferująca dodatkowe funkcje takie jak automatyczne skalowanie
- Możliwość uruchamiania kontenerów Docker
- Wbudowane wsparcie dla SSL, domen niestandardowych i zarządzania certyfikatami

### Azure Database for PostgreSQL

- Zarządzana usługa bazy danych PostgreSQL
- Automatyczne kopie zapasowe i wysoka dostępność
- Skalowanie w zależności od potrzeb

### Azure Monitor

- Monitorowanie wszystkich komponentów systemu
- Alerty i powiadomienia
- Integracja z OpenTelemetry

### Azure Key Vault

- Przechowywanie sekretów, kluczy i certyfikatów
- Bezpieczny dostęp do sekretów z aplikacji
- Rotacja kluczy i zarządzanie cyklem życia sekretów

## Strategia Zarządzania Konfiguracją

### Konfiguracja Aplikacji

- Konfiguracja specyficzna dla środowiska będzie przechowywana w zmiennych środowiskowych
- Sekrety będą przechowywane w Azure Key Vault
- Konfiguracja domyślna będzie przechowywana w plikach konfiguracyjnych w repozytorium

### Zarządzanie Zmiennymi Środowiskowymi

- Zmienne środowiskowe będą definiowane w plikach .env dla środowiska lokalnego
- W Azure, zmienne środowiskowe będą konfigurowane na poziomie usługi (App Service, Container Instances)
- Sekrety będą pobierane z Azure Key Vault podczas uruchamiania aplikacji

## Monitoring i Logowanie

### OpenTelemetry

- Zbieranie metryk i śladów z mikrousług
- Instrumentacja kodu dla lepszej widoczności
- Eksport danych do Prometheus i innych systemów

### Prometheus

- Przechowywanie i analiza metryk
- Definiowanie alertów
- Integracja z Grafana

### Grafana

- Wizualizacja metryk i tworzenie dashboardów
- Monitorowanie stanu systemu
- Alerty i powiadomienia

### Centralizowane Logowanie

- Agregacja logów ze wszystkich komponentów
- Strukturyzowane logowanie w formacie JSON
- Przechowywanie logów w Azure Log Analytics

## Strategia Backupu i Odtwarzania

### Backup Bazy Danych

- Automatyczne kopie zapasowe bazy danych PostgreSQL w Azure
- Regularne testy odtwarzania
- Strategia retencji kopii zapasowych

### Backup Konfiguracji

- Konfiguracja infrastruktury jako kod (w przyszłości)
- Wersjonowanie konfiguracji w repozytorium
- Dokumentacja procedur odtwarzania

## Bezpieczeństwo Infrastruktury

### Sieć

- Wszystkie połączenia zewnętrzne przez HTTPS
- Wewnętrzna komunikacja między usługami zabezpieczona
- Ograniczony dostęp do zasobów infrastruktury

### Zarządzanie Tożsamością i Dostępem

- Zasada najmniejszych uprawnień
- Uwierzytelnianie wieloskładnikowe dla dostępu administracyjnego
- Regularne przeglądy uprawnień

### Skanowanie Podatności

- Regularne skanowanie obrazów Docker pod kątem podatności
- Automatyczne aktualizacje zależności
- Monitorowanie alertów bezpieczeństwa

## Strategia Wdrażania

### Continuous Integration (CI)

- Automatyczne budowanie i testowanie kodu przy każdym push do repozytorium
- Budowanie obrazów Docker
- Uruchamianie testów jednostkowych i integracyjnych

### Continuous Deployment (CD)

- Automatyczne wdrażanie do środowiska deweloperskiego po pomyślnym CI
- Manualne zatwierdzanie wdrożeń do środowiska produkcyjnego
- Strategia rolling update dla minimalizacji przestojów

### Strategia Rollback

- Możliwość szybkiego powrotu do poprzedniej wersji w przypadku problemów
- Automatyczne rollbacki w przypadku nieudanych health checks
- Dokumentacja procedur rollback 
