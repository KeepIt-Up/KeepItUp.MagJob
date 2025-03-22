# Guide: Tworzenie Endpointów w FastEndpoints

## Spis treści
1. [Struktura i organizacja endpointów](#struktura-i-organizacja-endpointów)
2. [Podstawowy szablon endpointu](#podstawowy-szablon-endpointu)
3. [Autoryzacja](#autoryzacja)
4. [Walidacja](#walidacja)
5. [Obsługa użytkownika z CurrentUserAccessor](#obsługa-użytkownika-z-currentuseraccessor)
6. [Obsługa błędów](#obsługa-błędów)
7. [Odpowiedzi z endpointów](#odpowiedzi-z-endpointów)
8. [Dokumentacja endpointów](#dokumentacja-endpointów)
9. [Konwencje nazewnictwa](#konwencje-nazewnictwa)
10. [Przykłady](#przykłady)

## Struktura i organizacja endpointów

### Organizacja folderów
Endpointy powinny być organizowane w folderach według funkcjonalności, np.:
```
/Endpoints
  /Organizations
    CreateOrganizationEndpoint.cs
    GetOrganizationEndpoint.cs
    UpdateOrganizationEndpoint.cs
    DeleteOrganizationEndpoint.cs
  /Users
    GetUserEndpoint.cs
    UpdateUserEndpoint.cs
    ...
```

### Struktura pliku
Każdy endpoint powinien być zdefiniowany w osobnym pliku, z nazwą odzwierciedlającą jego funkcję.

## Podstawowy szablon endpointu

```csharp
using FastEndpoints;
using KeepItUp.MagJob.Identity.Web.Services;
using MediatR;

namespace KeepItUp.MagJob.Identity.Web.Endpoints.NazwaFunkcjonalności;

/// <summary>
/// Opis endpointu
/// </summary>
/// <remarks>
/// Dodatkowe informacje o endpoincie
/// </remarks>
public class NazwaEndpointu(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<RequestDto, ResponseDto>
{
    public override void Configure()
    {
        // Konfiguracja metody HTTP i ścieżki
        Verb("ścieżka/{parametr}");
        AllowAnonymous(); // Tymczasowo, docelowo RequireAuthorization()
        
        // Konfiguracja dokumentacji
        Description(b => b
            .WithName("NazwaEndpointu")
            .Produces<ResponseDto>(200)
            .ProducesProblem(400)
            // Inne kody odpowiedzi
        );
        
        Summary(s => {
            s.Summary = "Krótki opis";
            s.Description = "Dłuższy opis";
            s.ExampleRequest = new RequestDto { /* przykładowe dane */ };
            s.ResponseExamples[200] = new ResponseDto { /* przykładowe dane */ };
        });
    }

    public override async Task HandleAsync(RequestDto req, CancellationToken ct)
    {
        // Pobieranie ID użytkownika
        var userId = currentUserAccessor.GetRequiredCurrentUserId();
        
        // Tworzenie komendy/zapytania
        var command = new Command
        {
            // Mapowanie z req do command
            UserId = userId
        };
        
        // Wysłanie komendy/zapytania
        var result = await mediator.Send(command, ct);
        
        // Obsługa wyniku
        if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        // Inne obsługi statusów
        
        // Przygotowanie odpowiedzi
        Response = new ResponseDto
        {
            // Mapowanie z result do Response
        };
        
        // Wysłanie odpowiedzi
        await SendOkAsync(Response, ct);
    }
}
```

## Autoryzacja

Obecnie wszystkie endpointy powinny używać `AllowAnonymous()` do czasu pełnej implementacji autoryzacji. Docelowo będziemy używać `RequireAuthorization()` z odpowiednimi politykami.

```csharp
public override void Configure()
{
    Verb("ścieżka");
    AllowAnonymous(); // Tymczasowo
    // Docelowo: RequireAuthorization("NazwaPolityki");
}
```

## Walidacja

Walidacja powinna być realizowana zgodnie z dokumentacją FastEndpoints, używając FluentValidation. Walidatory powinny być zdefiniowane jako osobne klasy.

### Walidator
```csharp
public class RequestDtoValidator : Validator<RequestDto>
{
    public RequestDtoValidator()
    {
        RuleFor(x => x.Property)
            .NotEmpty()
            .WithMessage("Property jest wymagane");
            
        // Inne reguły walidacji
    }
}
```

### Obsługa błędów walidacji w endpoincie
```csharp
public override async Task HandleAsync(RequestDto req, CancellationToken ct)
{
    // Walidacja biznesowa
    if (warunekBiznesowy)
    {
        AddError(nameof(req.Property), "Komunikat błędu");
        ThrowIfAnyErrors();
    }
    
    // Reszta logiki
}
```

## Obsługa użytkownika z CurrentUserAccessor

Do pobierania informacji o bieżącym użytkowniku należy używać `ICurrentUserAccessor`. Obsługa wyjątków związanych z brakiem użytkownika powinna być realizowana przez globalny handler wyjątków.

```csharp
// Pobieranie ID użytkownika
var userId = currentUserAccessor.GetRequiredCurrentUserId();
```

## Obsługa błędów

Obsługa błędów powinna być realizowana przez globalny handler wyjątków, aby uniknąć duplikacji kodu. W endpointach należy używać metod `AddError()`, `ThrowIfAnyErrors()` i `ThrowError()`.

```csharp
// Dodanie błędu
AddError(nameof(req.Property), "Komunikat błędu");

// Sprawdzenie, czy są błędy i przerwanie wykonania
ThrowIfAnyErrors();

// Natychmiastowe przerwanie wykonania z błędem
ThrowError("Ogólny komunikat błędu");
```

## Odpowiedzi z endpointów

Do przygotowania odpowiedzi należy używać właściwości `Response`, a nie lokalnych zmiennych.

```csharp
// Przygotowanie odpowiedzi
Response = new ResponseDto
{
    Id = result.Value.Id,
    Name = result.Value.Name
};

// Wysłanie odpowiedzi
await SendOkAsync(Response, ct);
```

### Kody statusów HTTP

- 200 OK - Sukces dla operacji GET, PUT, PATCH
- 201 Created - Sukces dla operacji POST
- 204 No Content - Sukces dla operacji DELETE
- 400 Bad Request - Błędy walidacji
- 401 Unauthorized - Brak autoryzacji
- 403 Forbidden - Brak uprawnień
- 404 Not Found - Zasób nie znaleziony
- 500 Internal Server Error - Błąd serwera

## Dokumentacja endpointów

Dokumentacja endpointów powinna być realizowana przez XML comments oraz metody `Description()` i `Summary()`.

```csharp
/// <summary>
/// Opis endpointu
/// </summary>
/// <remarks>
/// Dodatkowe informacje o endpoincie
/// </remarks>
public class NazwaEndpointu(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<RequestDto, ResponseDto>
{
    public override void Configure()
    {
        // ...
        Description(b => b
            .WithName("NazwaEndpointu")
            .Produces<ResponseDto>(200)
            .ProducesProblem(400)
            // Inne kody odpowiedzi
        );
        
        Summary(s => {
            s.Summary = "Krótki opis";
            s.Description = "Dłuższy opis";
            s.ExampleRequest = new RequestDto { /* przykładowe dane */ };
            s.ResponseExamples[200] = new ResponseDto { /* przykładowe dane */ };
        });
    }
}
```

## Konwencje nazewnictwa

Należy stosować standardowe konwencje nazewnictwa zgodne z Ardalis.CleanArchitecture:

- **Endpointy**: `NazwaOperacjiNazwaZasobuEndpoint` (np. `CreateOrganizationEndpoint`)
- **DTO**: `NazwaOperacjiNazwaZasobuRequest/Response` (np. `CreateOrganizationRequest`, `CreateOrganizationResponse`)
- **Walidatory**: `NazwaOperacjiNazwaZasobuRequestValidator` (np. `CreateOrganizationRequestValidator`)

## Przykłady

### Przykład endpointu GET

```csharp
public class GetOrganizationEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<GetOrganizationRequest, GetOrganizationResponse>
{
    public override void Configure()
    {
        Get("api/organizations/{id}");
        AllowAnonymous();
        Description(b => b
            .WithName("GetOrganization")
            .Produces<GetOrganizationResponse>(200)
            .ProducesProblem(401)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Pobiera organizację";
            s.Description = "Pobiera organizację o podanym identyfikatorze";
        });
    }

    public override async Task HandleAsync(GetOrganizationRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var query = new GetOrganizationByIdQuery
        {
            OrganizationId = req.Id,
            UserId = userId
        };

        var result = await mediator.Send(query, ct);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (result.Status == ResultStatus.Forbidden)
        {
            await SendForbiddenAsync(ct);
            return;
        }

        Response = new GetOrganizationResponse
        {
            Id = result.Value.Id,
            Name = result.Value.Name,
            Description = result.Value.Description,
            OwnerId = result.Value.OwnerId,
            IsOwner = result.Value.OwnerId == userId
        };

        await SendOkAsync(Response, ct);
    }
}
```

### Przykład endpointu POST

```csharp
public class CreateOrganizationEndpoint(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<CreateOrganizationRequest, CreateOrganizationResponse>
{
    public override void Configure()
    {
        Post("api/organizations");
        AllowAnonymous();
        Description(b => b
            .WithName("CreateOrganization")
            .Produces<CreateOrganizationResponse>(201)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(500));
        Summary(s => {
            s.Summary = "Tworzy nową organizację";
            s.Description = "Tworzy nową organizację z podanymi danymi";
            s.ExampleRequest = new CreateOrganizationRequest { Name = "Nazwa organizacji", Description = "Opis organizacji" };
            s.ResponseExamples[201] = new CreateOrganizationResponse { Id = Guid.NewGuid(), Name = "Nazwa organizacji", Description = "Opis organizacji", OwnerId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(CreateOrganizationRequest req, CancellationToken ct)
    {
        var userId = currentUserAccessor.GetRequiredCurrentUserId();

        var command = new CreateOrganizationCommand
        {
            Name = req.Name,
            Description = req.Description,
            OwnerId = userId
        };

        var result = await mediator.Send(command, ct);

        if (result.Status == ResultStatus.Invalid)
        {
            foreach (var error in result.ValidationErrors)
            {
                AddError(error.ErrorMessage);
            }
            await SendErrorsAsync(400, ct);
            return;
        }

        Response = new CreateOrganizationResponse
        {
            Id = result.Value,
            Name = req.Name,
            Description = req.Description,
            OwnerId = userId
        };

        await SendCreatedAtAsync<GetOrganizationEndpoint>(
            new { id = Response.Id },
            Response,
            generateAbsoluteUrl: true,
            cancellation: ct);
    }
} 