# Walidacja danych wejściowych - zaimplementowane przykłady i kolejne kroki

## Naprawione błędy

W ramach prac nad ujednoliceniem podejścia do walidacji w projekcie naprawiono następujące błędy:

1. **Dodanie brakującej właściwości `Email` w klasie `UpdateUserCommand`**

   - W klasie `UpdateUserCommand` brakowało właściwości `Email`, która była używana w walidatorze
   - Dodano właściwość `Email` z odpowiednią dokumentacją XML
   - Zaktualizowano mapowanie w endpoincie `UpdateUser`, aby uwzględniało właściwość `Email` z żądania

2. **Naprawa błędu lintera w walidatorze `CreateRoleCommandValidator`**
   - Poprawiono implementację metody sprawdzającej członkostwo użytkownika w organizacji
   - Zastosowano podejście z bezpośrednią lambda funkcją zamiast referencji do metody, co rozwiązało problem typów argumentów

## Zaimplementowane zmiany

Zgodnie z rekomendacjami zawartymi w dokumencie `WalidacjaDanychWejsciowych.md`, zaimplementowano następujące usprawnienia:

1. **Dodanie metody `ExistsAsync` do interfejsu `IUserRepository`**

   - Umożliwia sprawdzenie istnienia użytkownika bez pobierania całego obiektu z bazy danych
   - Implementacja w klasie `UserRepository` wykorzystuje `AnyAsync` z Entity Framework Core

2. **Dodanie metod `ExistsAsync` i `ExistsByNameAsync` do interfejsu `IOrganizationRepository`**

   - Umożliwia sprawdzenie istnienia organizacji po ID lub nazwie bez pobierania całego obiektu
   - Implementacja w klasie `OrganizationRepository` wykorzystuje `AnyAsync` z Entity Framework Core

3. **Walidator dla `GetUserByIdRequest` w warstwie Web**

   - Podstawowa walidacja techniczna: sprawdzenie czy ID jest niepusty i różny od `Guid.Empty`
   - Implementacja przy użyciu klasy `Validator<T>` z FastEndpoints

4. **Rozszerzony walidator dla `GetUserByIdQuery` w warstwie UseCases**

   - Walidacja biznesowa: sprawdzenie istnienia użytkownika w bazie danych
   - Implementacja przy użyciu klasy `AbstractValidator<T>` z FluentValidation
   - Wstrzyknięcie zależności `IUserRepository` do walidatora

5. **Walidator dla `UpdateUserRequest` w warstwie Web**

   - Podstawowa walidacja techniczna: wymagane pola, długości pól, format adresu email
   - Implementacja przy użyciu klasy `Validator<T>` z FastEndpoints

6. **Rozszerzony walidator dla `UpdateUserCommand` w warstwie UseCases**

   - Walidacja biznesowa: istnienie użytkownika, unikalność adresu email
   - Właściwa obsługa walidacji pól opcjonalnych
   - Implementacja metod pomocniczych `UserExists` i `EmailNotTaken`

7. **Walidator dla `CreateOrganizationCommand` w warstwie UseCases**

   - Walidacja biznesowa: istnienie użytkownika, który ma być właścicielem organizacji
   - Wstrzyknięcie zależności `IUserRepository` do walidatora
   - Implementacja metody pomocniczej `UserExists`

8. **Walidator dla `CreateRoleCommand` w warstwie UseCases**

   - Walidacja biznesowa: istnienie organizacji, istnienie użytkownika, sprawdzanie członkostwa użytkownika w organizacji
   - Wstrzyknięcie zależności `IOrganizationRepository` i `IUserRepository` do walidatora
   - Implementacja metod pomocniczych `OrganizationExists`, `UserExists`
   - Zmiana sposobu implementacji walidacji członkostwa na lambda funkcję bezpośrednio w regule

9. **Walidator dla `UpdateOrganizationCommand` w warstwie UseCases**

   - Walidacja biznesowa: istnienie organizacji, unikalność nazwy, istnienie użytkownika, członkostwo użytkownika w organizacji
   - Wstrzyknięcie zależności `IOrganizationRepository` i `IUserRepository` do walidatora
   - Implementacja metod pomocniczych `OrganizationExists`, `UserExists`
   - Specjalna logika dla unikalności nazwy pozwalająca na zachowanie tej samej nazwy podczas aktualizacji

10. **Walidator dla `DeleteRoleCommand` w warstwie UseCases**

    - Walidacja biznesowa: istnienie organizacji, istnienie roli w tej organizacji, istnienie użytkownika, członkostwo użytkownika
    - Wstrzyknięcie zależności `IOrganizationRepository` i `IUserRepository` do walidatora
    - Złożona logika sprawdzająca, czy rola faktycznie należy do podanej organizacji

11. **Walidator dla `AssignRoleToMemberCommand` w warstwie UseCases**

    - Walidacja biznesowa: istnienie organizacji, roli, użytkowników oraz członkostwa użytkowników w organizacji
    - Wstrzyknięcie zależności `IOrganizationRepository` i `IUserRepository` do walidatora
    - Dokładne sprawdzenie, czy zarówno użytkownik wykonujący operację, jak i użytkownik, któremu ma być przypisana rola, są członkami organizacji

12. **Walidator dla `RevokeRoleFromMemberCommand` w warstwie UseCases**

    - Walidacja biznesowa: istnienie organizacji, roli, użytkowników, członkostwa użytkowników w organizacji oraz posiadania roli przez członka
    - Wstrzyknięcie zależności `IOrganizationRepository` i `IUserRepository` do walidatora
    - Dodatkowe sprawdzenie, czy użytkownik faktycznie posiada rolę, która ma zostać odebrana

13. **Walidator dla `RemoveMemberCommand` w warstwie UseCases**

    - Walidacja biznesowa: istnienie organizacji, użytkowników oraz członkostwa użytkowników w organizacji
    - Wstrzyknięcie zależności `IOrganizationRepository` i `IUserRepository` do walidatora
    - Dodatkowa reguła zabraniająca usuwania samego siebie z organizacji

14. **Walidator dla `UpdateRoleCommand` w warstwie UseCases**

    - Walidacja biznesowa: istnienie organizacji, roli, użytkownika, członkostwa użytkownika w organizacji oraz unikalności nazwy roli
    - Wstrzyknięcie zależności `IOrganizationRepository` i `IUserRepository` do walidatora
    - Specjalna logika dla unikalności nazwy pozwalająca na zachowanie tej samej nazwy podczas aktualizacji roli

15. **Konfiguracja walidacji w aplikacji**

    - Utworzenie klasy `ValidationConfig.cs` do rejestracji walidatorów
    - Konfiguracja walidatorów FastEndpoints (warstwa Web)
    - Rejestracja walidatorów FluentValidation (warstwa UseCases)
    - Aktualizacja `ServiceConfigs.cs`, aby uwzględnić konfigurację walidacji

16. **Dodatkowe walidatory w warstwie Web - Użytkownicy**

    - Walidator dla `GetUserOrganizationsRequest`: sprawdzenie identyfikatora użytkownika
    - Walidator dla `UpdateUserRequest`: walidacja identyfikatora, imienia, nazwiska i adresu email

17. **Dodatkowe walidatory w warstwie Web - Organizacje (podstawowe operacje)**

    - Walidator dla `CreateOrganizationRequest`: walidacja nazwy i opisu organizacji
    - Walidator dla `UpdateOrganizationRequest`: walidacja identyfikatora, nazwy i opisu organizacji
    - Walidator dla `DeleteOrganizationRequest`: walidacja identyfikatora organizacji
    - Walidator dla `GetOrganizationByIdRequest`: walidacja identyfikatora organizacji

18. **Dodatkowe walidatory w warstwie Web - Organizacje (role)**

    - Walidator dla `CreateRoleRequest`: walidacja identyfikatora organizacji, nazwy, opisu i koloru roli
    - Walidator dla `UpdateRoleRequest`: walidacja identyfikatorów organizacji i roli, nazwy, opisu i koloru roli
    - Walidator dla `DeleteRoleRequest`: walidacja identyfikatorów organizacji i roli
    - Walidator dla `GetOrganizationRolesRequest`: walidacja identyfikatora organizacji
    - Walidator dla `UpdateRolePermissionsRequest`: walidacja identyfikatorów organizacji i roli oraz listy uprawnień

19. **Dodatkowe walidatory w warstwie Web - Organizacje (członkowie i zaproszenia)**
    - Walidator dla `CreateInvitationRequest`: walidacja identyfikatora organizacji, adresu email i identyfikatora roli
    - Walidator dla `GetInvitationsRequest`: walidacja identyfikatora organizacji
    - Walidator dla `RejectInvitationRequest`: walidacja identyfikatora zaproszenia i tokenu
    - Walidator dla `AssignRoleToMemberRequest`: walidacja identyfikatorów organizacji, użytkownika i roli
    - Walidator dla `RevokeRoleFromMemberRequest`: walidacja identyfikatorów organizacji, użytkownika i roli
    - Walidator dla `RemoveMemberRequest`: walidacja identyfikatorów organizacji i użytkownika
    - Walidator dla `GetOrganizationMembersRequest`: walidacja identyfikatora organizacji

## Zmiany zaimplementowane ostatnio

1. **Naprawione błędy linterowe w walidatorach zaproszeń**:

   - Dodano brakującą metodę `GetByIdWithInvitationsAsync` do interfejsu `IOrganizationRepository`
   - Zaimplementowano tę metodę w klasie `OrganizationRepository`
   - Dzięki temu poprawiono walidatory: `CreateInvitationCommandValidator`, `AcceptInvitationCommandValidator` i `RejectInvitationCommandValidator`

2. **Usunięto zbędną walidację z handlerów komend**:

   - Z handlera `UpdateOrganizationLogoCommandHandler` usunięto redundantną logikę sprawdzania uprawnień, które teraz są weryfikowane przez walidator
   - Z handlera `UpdateOrganizationBannerCommandHandler` usunięto podobną logikę walidacyjną
   - Z handlerów endpointów (`UpdateOrganizationLogo` i `UpdateOrganizationBanner`) usunięto sprawdzanie poprawności plików, które teraz obsługiwane jest przez walidatory
   - Z handlerów `CreateInvitationCommandHandler`, `AcceptInvitationCommandHandler` i `RejectInvitationCommandHandler` usunięto logikę walidacji, która jest teraz obsługiwana przez odpowiednie walidatory
   - Z handlerów `RemoveMemberCommandHandler` i `RevokeRoleFromMemberCommandHandler` usunięto redundantną logikę walidacji, zachowując tylko sprawdzenie uprawnień użytkownika i reguły domenowe
   - Z handlera `DeactivateOrganizationCommandHandler` usunięto zbędne sprawdzenia, zachowując tylko kontrolę uprawnień właściciela
   - Z handlera `UpdateRolePermissionsCommandHandler` usunięto redundantne sprawdzenia istnienia organizacji i roli, pozostawiając tylko kontrolę uprawnień
   - Z handlera `UpdateRoleCommandHandler` usunięto zbędne sprawdzenia istnienia organizacji, roli i unikalności nazwy roli, zachowując logikę związaną z domyślnymi rolami systemowymi i kontrolę uprawnień

3. **Zaktualizowano wymagane zależności w handlerach zaproszeń**:
   - Z handlera `RejectInvitationCommandHandler` usunięto nieużywaną zależność `IUserRepository`
   - Z handlera `AcceptInvitationCommandHandler` usunięto nieużywaną zależność `IUserRepository`
   - Z handlera `CreateInvitationCommandHandler` usunięto nieużywaną zależność `IUserRepository`
   - Poprawiono użycie metod repozytorium w handlerach, aby korzystały z właściwych metod

## Podsumowanie ukończonych prac

W ramach prac nad implementacją spójnego podejścia do walidacji danych wejściowych w projekcie MagJob Identity, zrealizowano następujące zadania:

1. **Analiza istniejących mechanizmów walidacji**:

   - Przegląd walidacji w warstwach Web i UseCases
   - Identyfikacja różnic w podejściach do walidacji
   - Określenie docelowego modelu walidacji

2. **Implementacja walidacji technicznej (warstwa Web)**:

   - Implementacja walidatorów dla endpointów związanych z użytkownikami
   - Implementacja walidatorów dla endpointów związanych z organizacjami
   - Implementacja walidatorów dla endpointów związanych z rolami i uprawnieniami

3. **Implementacja walidacji biznesowej (warstwa UseCases)**:

   - Rozszerzenie repozytoriów o metody pomocnicze do walidacji
   - Implementacja walidatorów dla komend związanych z użytkownikami
   - Implementacja walidatorów dla komend związanych z organizacjami
   - Implementacja walidatorów dla komend związanych z rolami i członkostwem

4. **Naprawa błędów i uspójnienie podejścia**:
   - Naprawa błędów lintera w istniejących walidatorach
   - Dodanie brakujących właściwości w modelach komend
   - Uspójnienie komunikatów błędów walidacji

## Stan obecny

Wszystkie zidentyfikowane walidatory zostały zaimplementowane zgodnie z przyjętym podejściem:

- W warstwie Web znajdują się proste walidatory techniczne oparte o FastEndpoints
- W warstwie UseCases znajdują się rozszerzone walidatory biznesowe oparte o FluentValidation
- Walidatory biznesowe wykorzystują repozytoria do sprawdzania istnienia encji i relacji między nimi
- Komunikaty błędów są spójne i zawierają jasne informacje o problemach

## Plan na przyszłość

### 1. Testy jednostkowe dla walidatorów

Następnym etapem prac powinno być pokrycie walidatorów testami jednostkowymi, co zapewni ich poprawne działanie i odporność na zmiany w przyszłości:

- Implementacja testów dla walidatorów w warstwie Web (FastEndpoints)
- Implementacja testów dla walidatorów w warstwie UseCases (FluentValidation)
- Testy różnych scenariuszy walidacji, w tym przypadków granicznych

### 2. Udoskonalenia w obsłudze błędów walidacji

Warto rozważyć implementację centralnego mechanizmu obsługi błędów walidacji:

- Uspójnienie formatu komunikatów błędów
- Implementacja middleware do obsługi wyjątków walidacji
- Rozszerzenie logowania błędów walidacji

### 3. Dokumentacja

Ostatnim etapem powinno być uzupełnienie dokumentacji:

- Aktualizacja dokumentacji API w Swaggerze
- Dodanie przykładów korzystania z walidatorów w README projektu
- Utworzenie instrukcji dla deweloperów dotyczącej implementacji walidacji dla nowych endpointów

## Rekomendowane dalsze działania

1. **Priorytet wysoki**:

   - Implementacja podstawowych testów jednostkowych dla walidatorów

2. **Priorytet średni**:

   - Uspójnienie obsługi błędów walidacji
   - Rozszerzenie logowania błędów walidacji

3. **Priorytet niski**:
   - Uzupełnienie dokumentacji API
   - Refaktoryzacja wspólnych zasad walidacji do klas bazowych lub metod rozszerzających

Dzięki zrealizowanym pracom, system MagJob Identity posiada teraz spójne, kompletne i czytelne mechanizmy walidacji danych wejściowych, co przyczynia się do poprawy jakości kodu i zmniejszenia liczby potencjalnych błędów.

## Kolejne kroki do wykonania

Aby w pełni wdrożyć konsekwentne podejście do walidacji w projekcie, należy wykonać następujące kroki:

### 1. Analiza i uzupełnienie walidatorów dla istniejących endpointów

- [x] Przegląd wybranych endpointów w warstwie Web i sprawdzenie, które z nich wymagają walidatorów
- [x] Implementacja walidatorów dla wybranych żądań Web (FastEndpoints)
- [x] Implementacja walidatorów dla żądań związanych z użytkownikami
- [x] Implementacja walidatorów dla żądań związanych z organizacjami, rolami i członkami
- [x] Rozpoczęcie implementacji walidatorów biznesowych w warstwie UseCases
- [x] Dodanie pomocniczych metod do interfejsów repozytoriów
- [x] Rozszerzenie wybranych walidatorów w warstwie UseCases o walidację biznesową
- [ ] Pełny przegląd wszystkich komend i zapytań w warstwie UseCases
- [ ] Implementacja brakujących walidatorów dla komend i zapytań (FluentValidation)

### 2. Aktualizacja istniejących walidatorów

- [ ] Przegląd i aktualizacja istniejących walidatorów zgodnie z nowym podejściem
- [ ] Podział odpowiedzialności między walidacją techniczną a biznesową
- [ ] Dodanie odpowiednich zależności do walidatorów biznesowych

### 3. Uspójnienie obsługi błędów walidacji

- [ ] Przegląd obsługi błędów walidacji w endpointach
- [ ] Uspójnienie formatu komunikatów błędów
- [ ] Implementacja centralnego mechanizmu obsługi błędów walidacji

### 4. Testy jednostkowe dla walidatorów

- [ ] Implementacja testów jednostkowych dla walidatorów w warstwie Web
- [ ] Implementacja testów jednostkowych dla walidatorów w warstwie UseCases
- [ ] Pokrycie testami różnych scenariuszy walidacji

### 5. Dokumentacja

- [ ] Aktualizacja dokumentacji API w Swaggerze, aby uwzględniała wymagania walidacyjne
- [ ] Dodanie przykładów korzystania z walidatorów w README projektu
- [ ] Utworzenie instrukcji dla deweloperów dotyczącej implementacji walidacji dla nowych endpointów

## Priorytety

1. **Wysoki priorytet**:

   - ✅ Uzupełnienie walidatorów dla endpointów związanych z zarządzaniem użytkownikami
   - ✅ Uzupełnienie walidatorów dla endpointów związanych z zarządzaniem organizacjami
   - ✅ Rozszerzenie interfejsów repozytoriów o metody wspomagające walidację
   - ✅ Naprawa błędów lintera w istniejących walidatorach
   - ⚠️ Kontynuacja implementacji walidacji biznesowej w warstwie UseCases:
     - ✅ `UpdateUserCommand` - implementacja walidacji biznesowej
     - ✅ `CreateOrganizationCommand` - implementacja walidacji biznesowej
     - ✅ `UpdateOrganizationCommand` - implementacja walidacji biznesowej
     - ✅ `CreateRoleCommand` - implementacja walidacji biznesowej
     - ✅ `DeleteRoleCommand` - implementacja walidacji biznesowej
     - ✅ `AssignRoleToMemberCommand` - implementacja walidacji biznesowej
     - ✅ `RevokeRoleFromMemberCommand` - implementacja walidacji biznesowej
     - ✅ `RemoveMemberCommand` - implementacja walidacji biznesowej
     - ✅ `UpdateRoleCommand` - implementacja walidacji biznesowej
     - ✅ `DeactivateOrganizationCommand` - implementacja walidacji biznesowej
     - ✅ `CreateInvitationCommand` - implementacja walidacji biznesowej
     - ✅ `AcceptInvitationCommand` - implementacja walidacji biznesowej
     - ✅ `RejectInvitationCommand` - implementacja walidacji biznesowej
     - ✅ `UpdateOrganizationLogoCommand` - implementacja walidacji biznesowej
     - ✅ `UpdateOrganizationBannerCommand` - implementacja walidacji biznesowej
     - ✅ `UpdateRolePermissionsCommand` - implementacja walidacji biznesowej
   - ⚠️ Dodanie brakujących walidatorów w warstwie Web:
     - ✅ `UpdateOrganizationLogoRequest` - implementacja walidacji pliku i identyfikatora organizacji
     - ✅ `UpdateOrganizationBannerRequest` - implementacja walidacji pliku i identyfikatora organizacji

2. **Średni priorytet**:

   - Aktualizacja istniejących walidatorów
   - Uspójnienie obsługi błędów walidacji

3. **Niski priorytet**:
   - Testy jednostkowe
   - Dokumentacja

## Wzorce implementacji walidatorów

### Walidatory w warstwie Web (FastEndpoints)

```csharp
public class SampleRequestValidator : Validator<SampleRequest>
{
    public SampleRequestValidator()
    {
        // Walidacja identyfikatora
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator jest wymagany.")
            .Must(id => id != Guid.Empty).WithMessage("Identyfikator nie może być pusty (Guid.Empty).");

        // Walidacja pola tekstowego
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Nazwa jest wymagana.")
            .MaximumLength(100).WithMessage("Nazwa nie może przekraczać 100 znaków.");

        // Walidacja pola opcjonalnego
        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Opis nie może przekraczać 500 znaków.")
            .When(x => !string.IsNullOrEmpty(x.Description));
    }
}
```

### Walidatory w warstwie UseCases (FluentValidation)

```csharp
public class SampleCommandValidator : AbstractValidator<SampleCommand>
{
    private readonly ISampleRepository _sampleRepository;
    private readonly IOtherRepository _otherRepository;

    public SampleCommandValidator(ISampleRepository sampleRepository, IOtherRepository otherRepository)
    {
        _sampleRepository = sampleRepository ?? throw new ArgumentNullException(nameof(sampleRepository));
        _otherRepository = otherRepository ?? throw new ArgumentNullException(nameof(otherRepository));

        // Walidacja techniczna
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator jest wymagany.");

        // Walidacja biznesowa - istnienie encji
        RuleFor(x => x.Id)
            .MustAsync(EntityExists).WithMessage("Encja o podanym identyfikatorze nie istnieje.");

        // Walidacja biznesowa - unikalność nazwy
        RuleFor(x => x.Name)
            .MustAsync(async (command, name, context, cancellationToken) =>
            {
                var entity = await _sampleRepository.GetByNameAsync(name, cancellationToken);
                return entity == null || entity.Id == command.Id; // Nazwa może być taka sama jak obecna
            }).WithMessage("Nazwa jest już zajęta.");

        // Walidacja biznesowa - zależności między encjami
        RuleFor(x => x)
            .MustAsync(async (command, cancellationToken) =>
            {
                return await _otherRepository.HasRelationAsync(
                    command.Id,
                    command.OtherId,
                    cancellationToken);
            })
            .WithMessage("Powiązana encja nie istnieje lub nie ma relacji z główną encją.");
    }

    // Metody pomocnicze
    private async Task<bool> EntityExists(Guid id, CancellationToken cancellationToken)
    {
        return await _sampleRepository.ExistsAsync(id, cancellationToken);
    }
}
```

## Szacowany nakład pracy

- ✅ Analiza i uzupełnienie walidatorów w warstwie Web: ~8 godzin
- ⚠️ Implementacja walidatorów biznesowych w warstwie UseCases: ~8 godzin (w trakcie)
- Aktualizacja istniejących walidatorów: ~4 godziny
- Uspójnienie obsługi błędów: ~2 godziny
- Testy jednostkowe: ~6 godzin
- Dokumentacja: ~2 godziny

**Łącznie**: ~30 godzin

## Przykładowe testy jednostkowe

Zaimplementowano przykładowe testy jednostkowe dla walidatorów w warstwie UseCases:

### 1. Testy dla `UpdateRoleCommandValidator`

Utworzono zestaw testów jednostkowych dla walidatora `UpdateRoleCommandValidator`, który testuje:

- Poprawne działanie walidatora dla prawidłowych danych
- Walidację identyfikatora organizacji (pusty, nieistniejący)
- Walidację identyfikatora roli (pusty, nieistniejący w organizacji)
- Walidację nazwy roli (pusta, za długa, już istniejąca)
- Walidację koloru (nieprawidłowy format)
- Walidację istnienia użytkownika
- Walidację członkostwa użytkownika w organizacji

### 2. Testy dla `UpdateUserCommandValidator`

Zaimplementowano testy jednostkowe dla walidatora `UpdateUserCommandValidator`, sprawdzające:

- Walidację identyfikatora użytkownika (pusty, nieistniejący)
- Walidację imienia i nazwiska (puste, za długie)
- Walidację adresu email (pusty, nieprawidłowy format, już zajęty przez innego użytkownika)
- Walidację URL zdjęcia profilowego
- Obsługę pól opcjonalnych (telefon, adres)
- Obsługę zmiany danych bez zmiany adresu email

### 3. Testy dla `AssignRoleToMemberCommandValidator`

Dodano testy jednostkowe dla walidatora `AssignRoleToMemberCommandValidator`, testujące:

- Walidację identyfikatora organizacji (pusty, nieistniejący)
- Walidację identyfikatora roli (pusty, nieistniejący w organizacji)
- Walidację identyfikatora użytkownika docelowego (pusty, nieistniejący, nie jest członkiem organizacji)
- Walidację identyfikatora użytkownika wykonującego operację (pusty, nieistniejący, nie jest członkiem organizacji)
- Powiązania między różnymi encjami (organizacja, rola, użytkownik)

### 4. Struktura testów

Testy są zorganizowane według wzorca AAA (Arrange, Act, Assert):

- Arrange: przygotowanie danych testowych, w tym komend i zachowań mocków
- Act: wywołanie metody testowanej (`TestValidateAsync`)
- Assert: sprawdzenie wyników walidacji z użyciem metod `ShouldHaveValidationErrorFor` i `ShouldNotHaveAnyValidationErrors`

### 5. Obsługa prywatnych konstruktorów w testach

Ze względu na to, że klasy domenowe (np. `Organization`, `Role`, `User`) mają prywatne konstruktory i fabryki statyczne, w testach jednostkowych:

- Wykorzystano statyczne metody fabryczne (np. `Organization.Create`) do tworzenia obiektów
- Wykorzystano refleksję do ustawiania właściwości i modyfikowania pól prywatnych
- Zastosowano mocki repozytoriów dla symulowania różnych scenariuszy biznesowych

Testy te mogą służyć jako wzorzec dla implementacji testów dla pozostałych walidatorów w projekcie.

## Kolejne kroki w testowaniu

Aby w pełni pokryć testami jednostkowymi wszystkie walidatory w projekcie, należy wykonać następujące kroki:

1. **Zaimplementować testy dla pozostałych walidatorów w warstwie UseCases**:

   - Walidatory komend związanych z organizacjami
   - Walidatory komend związanych z zaproszeniami
   - Walidatory komend związanych z uprawnieniami

2. **Zaimplementować podstawowe testy dla walidatorów w warstwie Web**:
   - Testy walidatorów FastEndpoints
   - Weryfikacja walidacji technicznej
3. **Utworzyć bazowe klasy testowe**:

   - Wspólna logika konfiguracji mocków
   - Typowe scenariusze testowe
   - Pomocnicze metody do refleksji

4. **Dodać testy dla przypadków brzegowych**:
   - Obsługa wartości null
   - Obsługa niepoprawnych danych liczbowych
   - Obsługa znaków specjalnych w danych tekstowych
