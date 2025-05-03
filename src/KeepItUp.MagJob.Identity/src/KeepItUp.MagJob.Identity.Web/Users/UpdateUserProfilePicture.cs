using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint do aktualizacji zdjęcia profilowego użytkownika.
/// </summary>
public class UpdateUserProfilePicture : Endpoint<UpdateUserProfilePictureRequest, UpdateUserProfilePictureResponse>
{
    private readonly IMediator _mediator;
    private readonly IFileStorageService _fileStorageService;
    private readonly ICurrentUserAccessor _currentUserAccessor;
    private readonly ILogger<UpdateUserProfilePicture> _logger;

    /// <summary>
    /// Inicjalizuje nową instancję klasy <see cref="UpdateUserProfilePicture"/>.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="fileStorageService">Serwis przechowywania plików.</param>
    /// <param name="currentUserAccessor">Akcesor bieżącego użytkownika.</param>
    /// <param name="logger">Logger.</param>
    public UpdateUserProfilePicture(
        IMediator mediator,
        IFileStorageService fileStorageService,
        ICurrentUserAccessor currentUserAccessor,
        ILogger<UpdateUserProfilePicture> logger)
    {
        _mediator = mediator;
        _fileStorageService = fileStorageService;
        _currentUserAccessor = currentUserAccessor;
        _logger = logger;
    }

    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Put(UpdateUserProfilePictureRequest.Route);
        AllowFileUploads();
        AllowFormData();
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("UpdateUserProfilePicture")
            .WithTags("Users")
            .Produces<UpdateUserProfilePictureResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Aktualizuje zdjęcie profilowe użytkownika";
            s.Description = "Aktualizuje zdjęcie profilowe użytkownika o podanym identyfikatorze";
        });
    }

    /// <summary>
    /// Obsługuje żądanie PUT /api/users/{id}/profile-picture.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    public override async Task HandleAsync(UpdateUserProfilePictureRequest req, CancellationToken ct)
    {
        var currentUserId = _currentUserAccessor.GetCurrentUserId();

        if (!currentUserId.HasValue)
        {
            AddError("Użytkownik niezalogowany");
            await SendErrorsAsync(StatusCodes.Status401Unauthorized, ct);
            return;
        }

        // Sprawdź, czy użytkownik istnieje
        var getUserQuery = new GetUserByIdQuery
        {
            Id = req.UserId
        };

        var userResult = await _mediator.Send(getUserQuery, ct);

        if (!userResult.IsSuccess)
        {
            if (userResult.Status == ResultStatus.NotFound)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            foreach (var error in userResult.Errors)
            {
                AddError(error);
            }

            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        // Pobierz poprzednie zdjęcie profilowe, aby je usunąć po aktualizacji
        string? oldProfileImageUrl = userResult.Value.ProfileImageUrl();

        try
        {
            // Sprawdź, czy przesłano plik
            if (req.ProfilePictureFile == null || req.ProfilePictureFile.Length == 0)
            {
                AddError("Nie przesłano pliku ze zdjęciem profilowym");
                await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
                return;
            }

            // Sprawdź typ pliku (akceptuj tylko obrazy)
            string contentType = req.ProfilePictureFile.ContentType;
            if (!(contentType.StartsWith("image/jpeg") || contentType.StartsWith("image/png") || contentType.StartsWith("image/gif")))
            {
                AddError("Dozwolone są tylko obrazy w formatach JPEG, PNG lub GIF");
                await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
                return;
            }

            // Przesłanie pliku do usługi przechowywania
            string profileImageUrl;
            using (var stream = req.ProfilePictureFile.OpenReadStream())
            {
                // Zapisz zdjęcie profilowe w podkatalogu "profile-pictures"
                profileImageUrl = await _fileStorageService.UploadFileAsync(
                    stream,
                    req.ProfilePictureFile.FileName,
                    req.ProfilePictureFile.ContentType,
                    "profile-pictures"
                );
            }

            // Aktualizacja profilu użytkownika
            var command = new UpdateUserCommand
            {
                Id = req.UserId,
                FirstName = userResult.Value.FirstName,
                LastName = userResult.Value.LastName,
                PhoneNumber = userResult.Value.PhoneNumber(),
                Address = userResult.Value.Address(),
                ProfileImageUrl = profileImageUrl
            };

            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess)
            {
                // Jeśli aktualizacja się powiodła, usuń stare zdjęcie profilowe (tylko jeśli zostało uploaded lokalnie)
                if (!string.IsNullOrEmpty(oldProfileImageUrl)
                    && oldProfileImageUrl != profileImageUrl
                    && await _fileStorageService.FileExistsAsync(oldProfileImageUrl))
                {
                    await _fileStorageService.DeleteFileAsync(oldProfileImageUrl);
                }

                await SendAsync(new UpdateUserProfilePictureResponse { ProfileImageUrl = profileImageUrl }, StatusCodes.Status200OK, ct);
                return;
            }

            // Jeśli aktualizacja się nie powiodła, usuń nowo przesłane zdjęcie
            await _fileStorageService.DeleteFileAsync(profileImageUrl);

            foreach (var error in result.Errors)
            {
                AddError(error);
            }

            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Błąd podczas aktualizacji zdjęcia profilowego użytkownika {UserId}", req.UserId);
            AddError("Wystąpił błąd podczas przetwarzania pliku");
            await SendErrorsAsync(StatusCodes.Status500InternalServerError, ct);
        }
    }
}
