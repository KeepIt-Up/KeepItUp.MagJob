# Pipeline CI/CD - MagJob

Ten dokument opisuje pipeline Continuous Integration i Continuous Deployment dla projektu MagJob, wykorzystujący GitHub Actions.

## Przegląd

Pipeline CI/CD dla projektu MagJob jest zorganizowany wokół monorepo, które zawiera kod wszystkich komponentów systemu. Pipeline jest zaimplementowany przy użyciu GitHub Actions i obejmuje następujące etapy:

1. **Budowanie** - kompilacja kodu i budowanie obrazów Docker
2. **Testowanie** - uruchamianie testów jednostkowych i integracyjnych
3. **Analiza kodu** - statyczna analiza kodu i skanowanie bezpieczeństwa
4. **Publikowanie** - publikowanie obrazów Docker w rejestrze
5. **Wdrażanie** - wdrażanie aplikacji w środowiskach docelowych

## Struktura repozytorium

```
/
├── .github/
│   └── workflows/           # Definicje workflow GitHub Actions
├── src/
│   ├── KeepItUp.MagJob.Gateway/          # Mikroserwis Gateway
│   │   ├── KeepItUp.MagJob.Gateway.Web/  # Aplikacja Gateway
│   │   └── KeepItUp.MagJob.Gateway.Tests/# Testy dla Gateway
│   │
│   ├── KeepItUp.MagJob.Client/              # Aplikacja frontendowa
│   │
│   ├── KeepItUp.MagJob.Identity/src       # Mikroserwis zarządzania organizacjami i użytkownikami
│   │   ├── KeepItUp.MagJob.Identity.Web/         # API mikroserwisu
│   │   ├── KeepItUp.MagJob.Identity.UseCases/      # Warstwa domeny
│   │   ├── KeepItUp.MagJob.Identity.Infrastructure/ # Warstwa infrastruktury
│   │   ├── KeepItUp.MagJob.Identity.Core/ # Warstwa aplikacji
│   │
│   ├── KeepItUp.MagJob.Schedules/           # Mikroserwis zarządzania dyspozycyjnością i grafikami
│   │   ├── KeepItUp.MagJob.Schedules.Web/   # API mikroserwisu
│   │   ├── KeepItUp.MagJob.Schedules.UseCases/ # Warstwa use cases
│   │   ├── KeepItUp.MagJob.Schedules.Infrastructure/ # Warstwa infrastruktury
│   │   ├── KeepItUp.MagJob.Schedules.Core/ # Warstwa core
│   │
│   ├── KeepItUp.MagJob.WorkEvidence/        # Mikroserwis ewidencji czasu pracy
│   │   ├── KeepItUp.MagJob.WorkEvidence.Web/# API mikroserwisu
│   │   ├── KeepItUp.MagJob.WorkEvidence.UseCases/ # Warstwa use cases
│   │   ├── KeepItUp.MagJob.WorkEvidence.Infrastructure/ # Warstwa infrastruktury
│   │   ├── KeepItUp.MagJob.WorkEvidence.Core/ # Warstwa core
│   │
│   └── Keycloak/            # Konfiguracja Keycloak
│
├── docker/                  # Pliki Dockerfile dla poszczególnych komponentów
├── docs/                    # Dokumentacja projektu
├── scripts/                 # Skrypty pomocnicze
└── docker-compose.yml       # Konfiguracja Docker Compose
```

## Workflow GitHub Actions

### Workflow dla CI

Plik: `.github/workflows/ci.yml`

```yaml
name: Continuous Integration

on:
  push:
    branches: [ main, develop ]
  pull_request:
    branches: [ main, develop ]

jobs:
  build-and-test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v2
      
      # Konfiguracja .NET
      - name: Setup .NET
        uses: actions/setup-dotnet@v1
        with:
          dotnet-version: 8.0.x
      
      # Konfiguracja Node.js
      - name: Setup Node.js
        uses: actions/setup-node@v2
        with:
          node-version: '20'
      
      # Konfiguracja Java
      - name: Setup Java
        uses: actions/setup-java@v2
        with:
          distribution: 'adopt'
          java-version: '17'
      
      # Budowanie i testowanie .NET
      - name: Build .NET Projects
        run: |
          dotnet build src/APIGateway/APIGateway.Web/APIGateway.Web.csproj
          dotnet build src/KeepItUp.MagJob.Identity/src/KeepItUp.MagJob.Identity.Web/KeepItUp.MagJob.Identity.Web.csproj
      
      - name: Test .NET Projects
        run: |
          dotnet test src/APIGateway/APIGateway.Tests/APIGateway.Tests.csproj
          dotnet test src/KeepItUp.MagJob.Identity/tests/KeepItUp.MagJob.Identity.Tests/KeepItUp.MagJob.Identity.Tests.csproj
      
      # Budowanie i testowanie Angular
      - name: Install Angular dependencies
        run: cd src/KeepItUp.MagJob.Client && npm install
      
      - name: Build Angular
        run: cd src/KeepItUp.MagJob.Client && npm run build
      
      - name: Test Angular
        run: cd src/KeepItUp.MagJob.Client && npm run test
      
      # Budowanie i testowanie Spring
      - name: Build Spring Schedules
        run: cd src/Schedules/Schedules.API && ./mvnw clean package
      
      - name: Test Spring Schedules
        run: cd src/Schedules/Schedules.API && ./mvnw test
        
      - name: Build Spring WorkEvidence
        run: cd src/WorkEvidence/WorkEvidence.API && ./mvnw clean package
      
      - name: Test Spring WorkEvidence
        run: cd src/WorkEvidence/WorkEvidence.API && ./mvnw test
      
      # Statyczna analiza kodu
      - name: Run ESLint
        run: cd src/KeepItUp.MagJob.Client && npm run lint
      
      - name: Run SonarQube Scan
        uses: SonarSource/sonarcloud-github-action@master
        env:
          GITHUB_TOKEN: ${{ secrets.GITHUB_TOKEN }}
          SONAR_TOKEN: ${{ secrets.SONAR_TOKEN }}
      
      # Skanowanie bezpieczeństwa
      - name: Run OWASP Dependency Check
        uses: dependency-check/Dependency-Check_Action@main
        with:
          project: 'MagJob'
          path: '.'
          format: 'HTML'
          out: 'reports'
```

### Workflow dla CD - Środowisko Dev

Plik: `.github/workflows/cd-dev.yml`

```yaml
name: Deploy to Dev

on:
  push:
    branches: [ develop ]

jobs:
  deploy-to-dev:
    runs-on: ubuntu-latest
    needs: build-and-test
    steps:
      - uses: actions/checkout@v2
      
      # Logowanie do Azure
      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
      
      # Budowanie i publikowanie obrazów Docker
      - name: Build and push Docker images
        uses: docker/build-push-action@v2
        with:
          context: .
          push: true
          tags: |
            magjob/client-web:${{ github.sha }}
            magjob/api-gateway:${{ github.sha }}
            magjob/identity-api:${{ github.sha }}
      
      # Wdrażanie do Azure
      - name: Deploy to Azure
        uses: azure/webapps-deploy@v2
        with:
          app-name: 'magjob-dev'
          images: |
            magjob/api-gateway:${{ github.sha }}
            magjob/client-web:${{ github.sha }}
            magjob/identity-api:${{ github.sha }}
      
      # Powiadomienie o wdrożeniu
      - name: Notify Deployment
        uses: rtCamp/action-slack-notify@v2
        env:
          SLACK_WEBHOOK: ${{ secrets.SLACK_WEBHOOK }}
          SLACK_TITLE: 'Deployment to Dev'
          SLACK_MESSAGE: 'Successfully deployed to Dev environment'
```

### Workflow dla CD - Środowisko Prod

Plik: `.github/workflows/cd-prod.yml`

```yaml
name: Deploy to Production

on:
  push:
    branches: [ main ]

jobs:
  deploy-to-prod:
    runs-on: ubuntu-latest
    needs: build-and-test
    steps:
      - uses: actions/checkout@v2
      
      # Logowanie do Azure
      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
      
      # Budowanie i publikowanie obrazów Docker
      - name: Build and push Docker images
        uses: docker/build-push-action@v2
        with:
          context: .
          push: true
          tags: |
            magjob/client-web:${{ github.sha }}
            magjob/api-gateway:${{ github.sha }}
            magjob/identity-api:${{ github.sha }}
      
      # Wdrażanie do Azure
      - name: Deploy to Azure Kubernetes Service
        uses: azure/k8s-deploy@v1
        with:
          manifests: |
            kubernetes/client-web-deployment.yaml
            kubernetes/api-gateway-deployment.yaml
            kubernetes/identity-api-deployment.yaml
      
      # Powiadomienie o wdrożeniu
      - name: Notify Deployment
        uses: rtCamp/action-slack-notify@v2
        env:
          SLACK_WEBHOOK: ${{ secrets.SLACK_WEBHOOK }}
          SLACK_TITLE: 'Deployment to Production'
          SLACK_MESSAGE: 'Successfully deployed to Production environment'
```

## Strategia branchy

Projekt MagJob wykorzystuje następującą strategię branchy:

- **main** - branch produkcyjny, zawiera kod wdrożony na środowisku produkcyjnym
- **develop** - branch deweloperski, zawiera kod gotowy do wdrożenia na środowisku deweloperskim
- **feature/[nazwa-zadania]** - branche dla nowych funkcjonalności, tworzone na podstawie nazwy zadania w Jira
- **bugfix/[nazwa-zadania]** - branche dla poprawek błędów, tworzone na podstawie nazwy zadania w Jira
- **hotfix/[nazwa-zadania]** - branche dla pilnych poprawek na produkcji, tworzone na podstawie nazwy zadania w Jira

## Proces CI/CD

### Continuous Integration

1. Deweloper tworzy branch feature/bugfix/hotfix na podstawie nazwy zadania w Jira
2. Deweloper implementuje zmiany i tworzy Pull Request do brancha develop
3. GitHub Actions uruchamia workflow CI, który:
   - Buduje kod
   - Uruchamia testy jednostkowe i integracyjne
   - Przeprowadza statyczną analizę kodu
   - Skanuje kod pod kątem bezpieczeństwa
4. Jeśli wszystkie testy przejdą pomyślnie, Pull Request może zostać zaakceptowany
5. Po zaakceptowaniu Pull Request, zmiany są mergowane do brancha develop

### Continuous Deployment - Dev

1. Po zmergowaniu zmian do brancha develop, GitHub Actions uruchamia workflow CD-Dev, który:
   - Buduje obrazy Docker dla wszystkich komponentów
   - Publikuje obrazy Docker w rejestrze
   - Wdraża aplikację na środowisku deweloperskim w Azure
   - Wysyła powiadomienie o wdrożeniu

### Continuous Deployment - Prod

1. Po przetestowaniu zmian na środowisku deweloperskim, tworzony jest Pull Request z brancha develop do main
2. Po zaakceptowaniu Pull Request, zmiany są mergowane do brancha main
3. GitHub Actions uruchamia workflow CD-Prod, który:
   - Buduje obrazy Docker dla wszystkich komponentów
   - Publikuje obrazy Docker w rejestrze
   - Wdraża aplikację na środowisku produkcyjnym w Azure
   - Wysyła powiadomienie o wdrożeniu

## Monitorowanie i logowanie

- **Monitorowanie wdrożeń**: Azure Monitor
- **Logowanie**: Centralizowane logowanie z wykorzystaniem Azure Log Analytics
- **Alerty**: Powiadomienia o nieudanych wdrożeniach i problemach z aplikacją

## Strategia rollback

W przypadku problemów z wdrożeniem, dostępne są następujące opcje rollback:

1. **Automatyczny rollback**: W przypadku nieudanego wdrożenia, system automatycznie przywraca poprzednią wersję
2. **Manualny rollback**: Możliwość manualnego przywrócenia poprzedniej wersji poprzez ponowne wdrożenie starszego obrazu Docker
3. **Rollback bazy danych**: Procedury przywracania bazy danych do stanu sprzed wdrożenia (w przypadku zmian schematu)

## Sekrety i zmienne środowiskowe

Sekrety i zmienne środowiskowe są zarządzane w następujący sposób:

1. **GitHub Secrets**: Przechowywanie sekretów używanych w pipeline CI/CD
2. **Azure Key Vault**: Przechowywanie sekretów używanych przez aplikację
3. **Zmienne środowiskowe**: Konfiguracja specyficzna dla środowiska przekazywana do kontenerów Docker 

## Aktualizacje w pliku

- **Update Kubernetes manifests**:
  - Zastąpione odwołania do "Organizations.API" przez "identity-api"
  - Zastąpione odwołania do "organizations-api" przez "identity-api"
- **Update Docker Compose**:
  - Zastąpione odwołania do "Organizations.API" przez "identity-api"
  - Zastąpione odwołania do "organizations-api" przez "identity-api"
