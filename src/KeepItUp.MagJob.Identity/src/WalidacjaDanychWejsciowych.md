# Standaryzacja walidacji danych wejściowych w projekcie

## Problem

W projekcie występuje niekonsekwentne podejście do walidacji danych wejściowych:

- W warstwie Web (FastEndpoints) - używane są klasy `Validator<T>` z FastEndpoints
- W warstwie UseCases - używane są klasy `AbstractValidator<T>` z biblioteki FluentValidation
- Nie wszystkie endpointy/przypadki użycia mają zaimplementowaną walidację
- Brak jawnej rejestracji walidatorów w konfiguracji usług

## Analiza warstw walidacji

### 1. Walidacja w warstwie Web (FastEndpoints)

**Zalety:**

- Szybkie odrzucanie nieprawidłowych żądań bez angażowania logiki biznesowej
- Jednolity sposób obsługi błędów walidacji dla klientów API
- Automatyczne mapowanie błędów walidacji na odpowiedzi HTTP 400 Bad Request
- Logika walidacji jest blisko endpointów, co ułatwia jej utrzymanie

**Wady:**

- Walidacja jest duplikowana, jeśli te same dane są używane w różnych endpointach
- Oddzielenie walidacji od logiki biznesowej może prowadzić do niespójności

### 2. Walidacja w warstwie UseCases (Application)

**Zalety:**

- Walidacja jest częścią logiki biznesowej i jest zawsze wykonywana
- Zapobiega sytuacjom, w których nieprawidłowe dane mogłyby zostać przekazane do warstwy domenowej
- Walidatory mogą być łatwo reużywane przez różne komponenty aplikacji

**Wady:**

- Błędy walidacji wymagają dodatkowej obsługi w warstwie Web
- Możliwe opóźnienie w wykrywaniu błędów (dopiero po przekazaniu danych do warstwy aplikacji)

## Rekomendowane podejście: Walidacja w obu warstwach z jasnym podziałem odpowiedzialności

Zalecamy zastosowanie walidacji w obu warstwach, ale z jasnym podziałem odpowiedzialności:

### Warstwa Web (FastEndpoints):

- **Podstawowa walidacja techniczna**: format danych, zgodność typów, wymagane pola, limity długości, etc.
- Walidacja specyficzna dla kontekstu HTTP: parametry zapytania, nagłówki, formaty URL, etc.
- Celem jest szybkie odrzucenie oczywiście nieprawidłowych żądań

### Warstwa UseCases (Application):

- **Walidacja biznesowa**: reguły biznesowe, spójność danych, relacje między obiektami
- Walidacja wymagająca dostępu do bazy danych lub innych zewnętrznych zasobów
- Celem jest zapewnienie poprawności danych z perspektywy domenowej

## Implementacja

### 1. Standaryzacja rejestracji walidatorów

Dodać jawną rejestrację walidatorów w konfiguracji aplikacji:

```csharp
// Dla warstwy Web (FastEndpoints)
services.AddFastEndpoints(c => {
    c.DisableAutoDiscovery = false;
});

// Dla warstwy UseCases (FluentValidation)
services.AddValidatorsFromAssemblyContaining<UpdateUserCommandValidator>();
```

### 2. Implementacja brakujących walidatorów

Przejrzeć wszystkie endpointy i przypadki użycia, gdzie brakuje walidacji i dodać odpowiednie klasy walidatorów.

### 3. Rozdzielenie odpowiedzialności walidacji

Ujednolicić podejście do walidacji, określając jasno, które reguły powinny być implementowane w warstwie Web, a które w warstwie UseCases.

## Przykłady implementacji

### Przykład walidacji w warstwie Web (FastEndpoints)

```csharp
public class UpdateUserRequestValidator : Validator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Identyfikator użytkownika jest wymagany.");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Imię jest wymagane.")
            .MaximumLength(100).WithMessage("Imię nie może być dłuższe niż 100 znaków.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Nazwisko jest wymagane.")
            .MaximumLength(100).WithMessage("Nazwisko nie może być dłuższe niż 100 znaków.");
    }
}
```

### Przykład walidacji w warstwie UseCases (FluentValidation)

```csharp
public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(x => x.Id)
            .MustAsync(UserExists).WithMessage("Użytkownik o podanym identyfikatorze nie istnieje.");

        RuleFor(x => x.Email)
            .EmailAddress().WithMessage("Podany adres email jest nieprawidłowy.")
            .MustAsync(EmailNotTaken).WithMessage("Podany adres email jest już zajęty.");
    }

    private async Task<bool> UserExists(Guid userId, CancellationToken cancellationToken)
    {
        return await _userRepository.ExistsAsync(userId, cancellationToken);
    }

    private async Task<bool> EmailNotTaken(UpdateUserCommand command, string email, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(email, cancellationToken);
        return existingUser == null || existingUser.Id == command.Id;
    }
}
```

## Korzyści

1. **Spójność**: Jednolite podejście do walidacji we wszystkich endpointach i przypadkach użycia
2. **Niezawodność**: Mniejsze ryzyko wprowadzenia nieprawidłowych danych do systemu
3. **Lepsza obsługa błędów**: Jasne komunikaty błędów dla klientów API
4. **Separacja odpowiedzialności**: Wyraźny podział między walidacją techniczną a biznesową

## Dodatkowe zalecenia

1. **Testy jednostkowe dla walidatorów**: Zapewniają, że reguły walidacji działają poprawnie
2. **Centralizacja komunikatów błędów**: Ułatwia lokalizację i utrzymanie spójności komunikatów
3. **Dokumentacja walidacji w Swaggerze**: Informuje klientów API o oczekiwanych formatach danych
