using KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;
using KeepItUp.MagJob.Identity.Web.Services;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint do aktualizacji użytkownika.
/// </summary>
/// <remarks>
/// Aktualizuje użytkownika o podanym identyfikatorze.
/// </remarks>
public class UpdateUser(IMediator mediator, ICurrentUserAccessor currentUserAccessor)
    : Endpoint<UpdateUserRequest, UpdateUserResponse>
{
    /// <summary>
    /// Konfiguruje endpoint.
    /// </summary>
    public override void Configure()
    {
        Put(UpdateUserRequest.Route);
        AllowAnonymous(); // Tymczasowo, do czasu naprawienia autoryzacji
        Description(b => b
            .WithName("UpdateUser")
            .Produces<UpdateUserResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(401)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Aktualizuje użytkownika";
            s.Description = "Aktualizuje użytkownika o podanym identyfikatorze";
            s.ExampleRequest = new UpdateUserRequest
            {
                Id = Guid.NewGuid(),
                FirstName = "Jan",
                LastName = "Kowalski"
            };
        });
    }

    /// <summary>
    /// Obsługuje żądanie PUT /api/users/{id}.
    /// </summary>
    /// <param name="req">Żądanie.</param>
    /// <param name="ct">Token anulowania.</param>
    /// <returns>Odpowiedź z danymi zaktualizowanego użytkownika.</returns>
    public override async Task HandleAsync(UpdateUserRequest req, CancellationToken ct)
    {
        // Pobierz identyfikator bieżącego użytkownika
        var currentUserId = currentUserAccessor.GetRequiredCurrentUserId();

        // Pobierz użytkownika, aby uzyskać aktualne dane
        var getUserQuery = new GetUserByIdQuery
        {
            Id = req.Id
        };

        var userResult = await mediator.Send(getUserQuery, ct);

        if (userResult.Status != ResultStatus.Ok)
        {
            if (userResult.Status == ResultStatus.NotFound)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            await SendErrorsAsync(500, ct);
            return;
        }

        var command = new UpdateUserCommand
        {
            Id = req.Id,
            FirstName = req.FirstName,
            LastName = req.LastName,
            // Zachowaj istniejące dane profilu
            PhoneNumber = req.PhoneNumber ?? userResult.Value.PhoneNumber(),
            Address = req.Address ?? userResult.Value.Address(),
            ProfileImageUrl = userResult.Value.ProfileImageUrl()
        };

        var result = await mediator.Send(command, ct);

        if (result.Status == ResultStatus.NotFound)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        if (result.Status == ResultStatus.Error)
        {
            await SendErrorsAsync(500, ct);
            return;
        }

        if (result.Status == ResultStatus.Invalid)
        {
            foreach (var error in result.ValidationErrors)
            {
                AddError(error.ErrorMessage);
            }
            await SendErrorsAsync(400, ct);
            return;
        }

        // Pobierz zaktualizowanego użytkownika
        var updatedUserQuery = new GetUserByIdQuery
        {
            Id = req.Id
        };

        var updatedUserResult = await mediator.Send(updatedUserQuery, ct);

        if (updatedUserResult.Status != ResultStatus.Ok)
        {
            await SendErrorsAsync(500, ct);
            return;
        }

        var response = new UpdateUserResponse
        {
            Id = updatedUserResult.Value.Id,
            ExternalId = updatedUserResult.Value.ExternalId,
            Email = updatedUserResult.Value.Email,
            FirstName = updatedUserResult.Value.FirstName,
            LastName = updatedUserResult.Value.LastName,
            IsActive = updatedUserResult.Value.IsActive,
            ProfileImageUrl = updatedUserResult.Value.ProfileImageUrl(),
            PhoneNumber = updatedUserResult.Value.PhoneNumber(),
            Address = updatedUserResult.Value.Address()
        };

        await SendOkAsync(response, ct);
    }
}
