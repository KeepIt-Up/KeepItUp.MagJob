using KeepItUp.MagJob.Identity.UseCases.Users.Commands.UpdateUser;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint to update a user.
/// </summary>
/// <remarks>
/// Updates a user with the specified identifier.
/// </remarks>
public class UpdateUser(IMediator mediator)
    : BaseEndpoint<UpdateUserRequest, EmptyResponse>
{
    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Put(UpdateUserRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Updates a user";
            s.Description = "Updates a user with the specified identifier";
            s.ExampleRequest = new UpdateUserRequest
            {
                Id = Guid.NewGuid(),
                FirstName = "Jan",
                LastName = "Kowalski"
            };
        });
    }

    /// <summary>
    /// Handles the PUT /api/users/{id} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the updated user data.</returns>
    protected override async Task<EmptyResponse> HandleEndpointAsync(UpdateUserRequest req, CancellationToken ct)
    {

        var command = new UpdateUserCommand
        {
            Id = req.Id,
            FirstName = req.FirstName,
            LastName = req.LastName,
            PhoneNumber = req.PhoneNumber,
            Address = req.Address,
        };

        return await mediator.Send(command, ct);
    }
}
