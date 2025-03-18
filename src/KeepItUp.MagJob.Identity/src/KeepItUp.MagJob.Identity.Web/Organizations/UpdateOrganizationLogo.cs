using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Commands.UpdateOrganizationLogo;
using KeepItUp.MagJob.Identity.UseCases.Organizations.Queries.GetOrganizationById;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Organizations;

/// <summary>
/// Endpoint do aktualizacji logo organizacji.
/// </summary>
public class UpdateOrganizationLogo : Endpoint<UpdateOrganizationLogoRequest, UpdateOrganizationLogoResponse>
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly ILogger<UpdateOrganizationLogo> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateOrganizationLogo"/>.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="fileStorageService">Serwis przechowywania plików.</param>
    /// <param name="currentUserAccessor">Akcesor bieżącego użytkownika.</param>
    /// <param name="logger">Logger.</param>
    public UpdateOrganizationLogo(
        IMediator mediator,
        IFileStorageService fileStorageService,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<UpdateOrganizationLogo> logger)
    {
        _mediator = mediator;
        _fileStorageService = fileStorageService;
        _currentUserAccessor = currentUserAccessor;
        _logger = logger;
    }

    /// <inheritdoc/>
    public override void Configure()
    {
        Put(UpdateOrganizationLogoRequest.Route);
        AllowFileUploads();
        AllowFormData();
        Permissions(OrganizationPermissions.UpdateOrganization);
        Description(d =>
        {
            d.WithName("UpdateOrganizationLogo");
            d.WithTags("Organizations");
            d.WithSummary("Aktualizuje logo organizacji");
            d.WithDescription("Aktualizuje logo organizacji.");
        });
    }

    /// <summary>
    /// Obsługuje żądanie PUT /api/organizations/{organizationId}/logo.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    public override async Task HandleAsync(UpdateOrganizationLogoRequest req, CancellationToken ct)
    {
        var currentUserId = _currentUserAccessor.GetCurrentUserId();

        if (!currentUserId.HasValue)
        {
            AddError("Użytkownik niezalogowany");
            await SendErrorsAsync(StatusCodes.Status401Unauthorized, ct);
            return;
        }

        if (req.LogoFile == null || req.LogoFile.Length == 0)
        {
            AddError("Nie przesłano pliku");
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        // Sprawdzenie typu pliku (tylko obrazy)
        var allowedTypes = new[] { "image/jpeg", "image/png", "image/gif" };
        if (!allowedTypes.Contains(req.LogoFile.ContentType.ToLower()))
        {
            AddError("Niedozwolony typ pliku. Dozwolone typy: JPEG, PNG, GIF");
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        // Sprawdzenie rozmiaru pliku (np. max 2MB)
        if (req.LogoFile.Length > 2 * 1024 * 1024)
        {
            AddError("Plik jest zbyt duży. Maksymalny rozmiar to 2MB");
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        // Pobierz organizację, aby sprawdzić, czy użytkownik ma uprawnienia
        var getOrganizationQuery = new GetOrganizationByIdQuery
        {
            OrganizationId = req.OrganizationId,
            UserId = currentUserId.Value
        };

        var organizationResult = await _mediator.Send(getOrganizationQuery, ct);

        if (!organizationResult.IsSuccess)
        {
            if (organizationResult.Status == ResultStatus.NotFound)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            if (organizationResult.Status == ResultStatus.Forbidden)
            {
                await SendForbiddenAsync(ct);
                return;
            }

            foreach (var error in organizationResult.Errors)
            {
                AddError(error);
            }

            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        // Pobierz poprzednie logo, aby je usunąć po aktualizacji
        string? oldLogoUrl = organizationResult.Value.LogoUrl;

        try
        {
            // Przesłanie pliku do usługi przechowywania
            string logoUrl;
            using (var stream = req.LogoFile.OpenReadStream())
            {
                // Zapisz logo w podkatalogu "logos"
                logoUrl = await _fileStorageService.UploadFileAsync(
                    stream,
                    req.LogoFile.FileName,
                    req.LogoFile.ContentType,
                    "logos"
                );
            }

            // Aktualizacja organizacji za pomocą komendy
            var command = new UpdateOrganizationLogoCommand
            {
                OrganizationId = req.OrganizationId,
                LogoUrl = logoUrl,
                UserId = currentUserId.Value
            };

            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess)
            {
                // Jeśli aktualizacja się powiodła, usuń stare logo
                if (!string.IsNullOrEmpty(oldLogoUrl) && oldLogoUrl != logoUrl)
                {
                    await _fileStorageService.DeleteFileAsync(oldLogoUrl);
                }

                await SendAsync(new UpdateOrganizationLogoResponse { LogoUrl = logoUrl }, StatusCodes.Status200OK, ct);
                return;
            }

            // Jeśli aktualizacja się nie powiodła, usuń nowo przesłane logo
            await _fileStorageService.DeleteFileAsync(logoUrl);

            foreach (var error in result.Errors)
            {
                AddError(error);
            }

            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji logo organizacji {OrganizationId}", req.OrganizationId);
            AddError("Wystąpił błąd podczas przetwarzania pliku");
            await SendErrorsAsync(StatusCodes.Status500InternalServerError, ct);
        }
    }
}
