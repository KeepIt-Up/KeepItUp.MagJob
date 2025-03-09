# Standardy Kodowania - MagJob

Ten dokument opisuje standardy kodowania dla projektu MagJob, które powinny być przestrzegane przez wszystkich deweloperów pracujących nad projektem.

## Ogólne Zasady

### Nazewnictwo

- Używaj znaczących, opisowych nazw dla wszystkich identyfikatorów (zmiennych, metod, klas, itp.)
- Unikaj skrótów, chyba że są powszechnie znane
- Używaj angielskiego języka dla wszystkich identyfikatorów i komentarzy
- Przestrzegaj konwencji nazewnictwa specyficznych dla danego języka/frameworka

### Formatowanie

- Używaj 4 spacji do wcięć (nie tabulatorów)
- Maksymalna długość linii: 120 znaków
- Używaj pustych linii do logicznego grupowania kodu
- Konsekwentnie stosuj nawiasy klamrowe
- Używaj narzędzi do automatycznego formatowania kodu

### Komentarze

- Kod powinien być samodokumentujący się poprzez dobre nazewnictwo i strukturę
- Używaj komentarzy do wyjaśnienia skomplikowanej logiki lub decyzji projektowych
- Unikaj oczywistych komentarzy, które powtarzają to, co robi kod
- Dokumentuj publiczne API za pomocą komentarzy dokumentacyjnych (XML Doc, JSDoc, Javadoc)

### Obsługa Błędów

- Zawsze obsługuj potencjalne błędy
- Używaj wyjątków do sygnalizowania błędów, nie do kontroli przepływu
- Loguj błędy z odpowiednim poziomem szczegółowości
- Nie połykaj wyjątków bez logowania

### Testowanie

- Pisz testy jednostkowe dla wszystkich nowych funkcjonalności
- Utrzymuj wysokie pokrycie kodu testami
- Testy powinny być niezależne od siebie
- Używaj mocków i stubów do izolowania testowanych komponentów

## Standardy dla .NET (C#)

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
- **Przestrzenie nazw**: PascalCase, zgodne z hierarchią katalogów
  ```csharp
  namespace KeepItUp.MagJob.Identity.Application.Services
  ```

### Organizacja Kodu

- Używaj przestrzeni nazw do organizacji kodu
- Jeden plik powinien zawierać jedną klasę publiczną
- Organizuj kod w warstwy zgodnie z architekturą Clean Architecture
- Używaj modyfikatorów dostępu (public, private, protected, internal) świadomie

### Dobre Praktyki

- Preferuj typy immutable
- Używaj async/await do operacji asynchronicznych
- Używaj LINQ do operacji na kolekcjach
- Używaj dependency injection
- Unikaj magicznych liczb i stringów
- Używaj nullable reference types
- Używaj record types dla obiektów DTO

### Przykład

```csharp
using System;
using System.Threading.Tasks;

namespace KeepItUp.MagJob.Identity.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        
        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        }
        
        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            
            if (user == null)
            {
                throw new UserNotFoundException(userId);
            }
            
            return new UserDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };
        }
    }
}
```

## Standardy dla Spring (Java)

### Konwencje Nazewnictwa

- **Klasy, Interfejsy, Enumy**: PascalCase
  ```java
  public class UserService
  public interface UserRepository
  public enum UserStatus
  ```
- **Metody, zmienne, parametry**: camelCase
  ```java
  public void processRequest()
  private String firstName;
  public void processUser(Long userId)
  ```
- **Stałe**: UPPER_CASE_WITH_UNDERSCORES
  ```java
  public static final String DEFAULT_ROLE = "User";
  ```
- **Pakiety**: lowercase.with.dots
  ```java
  package com.magjob.schedules.service;
  ```

### Organizacja Kodu

- Używaj pakietów do organizacji kodu
- Jeden plik powinien zawierać jedną klasę publiczną
- Organizuj kod zgodnie z architekturą warstwową (controller, service, repository, model)
- Używaj adnotacji Spring do definiowania komponentów

### Dobre Praktyki

- Używaj Lombok do redukcji boilerplate kodu
- Używaj Optional dla wartości, które mogą być null
- Używaj Stream API do operacji na kolekcjach
- Używaj dependency injection przez konstruktor
- Używaj adnotacji @Transactional dla metod modyfikujących dane
- Używaj ResponseEntity do zwracania odpowiedzi HTTP

### Przykład

```java
package com.magjob.schedules.service;

import com.magjob.schedules.dto.ScheduleDto;
import com.magjob.schedules.exception.ScheduleNotFoundException;
import com.magjob.schedules.repository.ScheduleRepository;
import lombok.RequiredArgsConstructor;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import java.util.UUID;

@Service
@RequiredArgsConstructor
public class ScheduleService {
    
    private final ScheduleRepository scheduleRepository;
    
    @Transactional(readOnly = true)
    public ScheduleDto getScheduleById(UUID scheduleId) {
        return scheduleRepository.findById(scheduleId)
                .map(schedule -> new ScheduleDto(
                        schedule.getId(),
                        schedule.getName(),
                        schedule.getDescription(),
                        schedule.getStartDate(),
                        schedule.getEndDate(),
                        schedule.getStatus()
                ))
                .orElseThrow(() -> new ScheduleNotFoundException(scheduleId));
    }
}
```

## Standardy dla Angular (TypeScript)

### Konwencje Nazewnictwa

- **Klasy, Interfejsy, Typy, Enumy, Dekoratory**: PascalCase
  ```typescript
  export class UserComponent
  export interface User
  export type UserResponse
  export enum UserStatus
  @Component()
  ```
- **Zmienne, funkcje, metody, properties**: camelCase
  ```typescript
  const firstName: string;
  function getUserById()
  public getUsers()
  ```
- **Stałe**: UPPER_CASE_WITH_UNDERSCORES lub PascalCase
  ```typescript
  const API_URL = 'https://api.example.com';
  const DefaultRole = 'User';
  ```
- **Selektory komponentów**: kebab-case
  ```typescript
  selector: 'app-user-list'
  ```
- **Pliki**: kebab-case.type.ts
  ```
  user.component.ts
  user.service.ts
  user.model.ts
  ```

### Organizacja Kodu

- Organizuj kod według funkcjonalności (feature modules)
- Używaj lazy loading dla modułów
- Przestrzegaj zasady Single Responsibility Principle
- Używaj barrel files (index.ts) do eksportowania publicznego API modułu

### Dobre Praktyki

- Używaj TypeScript strict mode
- Używaj interfejsów do definiowania typów
- Używaj RxJS dla operacji asynchronicznych
- Używaj Angular CLI do generowania komponentów, serwisów, itp.
- Używaj OnPush strategy dla detekcji zmian
- Używaj reactive forms zamiast template-driven forms
- Używaj NgRx dla zarządzania stanem aplikacji (dla większych aplikacji)

### Przykład

```typescript
import { Component, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Observable } from 'rxjs';
import { UserService } from '../../services/user.service';
import { User } from '../../models/user.model';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class UserListComponent implements OnInit {
  users$: Observable<User[]>;
  
  constructor(private userService: UserService) {}
  
  ngOnInit(): void {
    this.users$ = this.userService.getUsers();
  }
  
  deleteUser(userId: string): void {
    this.userService.deleteUser(userId).subscribe(() => {
      this.users$ = this.userService.getUsers();
    });
  }
}
```

## Standardy dla SQL

### Konwencje Nazewnictwa

- **Tabele**: snake_case, liczba mnoga
  ```sql
  CREATE TABLE users
  CREATE TABLE schedule_entries
  ```
- **Kolumny**: snake_case
  ```sql
  user_id
  first_name
  created_at
  ```
- **Klucze obce**: nazwa_tabeli_nazwa_kolumny_fk
  ```sql
  users_organization_id_fk
  ```
- **Indeksy**: nazwa_tabeli_nazwa_kolumny_idx
  ```sql
  users_email_idx
  ```
- **Schematy**: snake_case
  ```sql
  CREATE SCHEMA identity
  ```

### Dobre Praktyki

- Używaj migracji do zarządzania schematem bazy danych
- Definiuj ograniczenia integralności (klucze obce, unikalne indeksy)
- Używaj indeksów dla często wyszukiwanych kolumn
- Używaj transakcji dla operacji modyfikujących dane
- Unikaj złożonych zapytań w kodzie aplikacji, używaj procedur składowanych lub widoków
- Używaj parametryzowanych zapytań, nigdy nie konkatenuj stringów SQL

### Przykład

```sql
CREATE TABLE identity.organizations (
    id UUID PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    owner_id UUID NOT NULL,
    CONSTRAINT organizations_owner_id_fk FOREIGN KEY (owner_id) REFERENCES identity.users(id)
);

CREATE INDEX organizations_name_idx ON identity.organizations(name);
```

## Proces Code Review

### Kryteria Akceptacji

- Kod jest zgodny ze standardami kodowania
- Kod jest pokryty testami
- Kod jest zoptymalizowany pod kątem wydajności
- Kod jest bezpieczny
- Kod jest czytelny i łatwy do utrzymania
- Kod realizuje wymagania biznesowe

### Proces

1. Deweloper tworzy Pull Request
2. Automatyczne testy CI są uruchamiane
3. Co najmniej jeden inny deweloper przegląda kod
4. Deweloper wprowadza poprawki na podstawie uwag
5. Po zaakceptowaniu, kod jest mergowany do docelowego brancha

### Wskazówki dla Recenzentów

- Bądź konstruktywny i uprzejmy
- Skup się na istotnych problemach, nie na preferencjach stylistycznych
- Wyjaśniaj, dlaczego coś powinno być zmienione
- Sugeruj alternatywne rozwiązania
- Doceniaj dobre rozwiązania

## Narzędzia do Egzekwowania Standardów

- **C#**: StyleCop, .editorconfig, ReSharper/Rider
- **Java**: Checkstyle, PMD, SonarQube
- **TypeScript/Angular**: ESLint, Prettier, Angular ESLint
- **SQL**: SQL Formatter, SQL Lint
- **Ogólne**: SonarQube, GitHub Actions, Husky (pre-commit hooks)

## Wdrażanie Standardów

- Standardy kodowania są częścią procesu onboardingu nowych deweloperów
- Regularne przeglądy i aktualizacje standardów
- Automatyczne egzekwowanie standardów w pipeline CI/CD
- Code review jako narzędzie edukacyjne 
