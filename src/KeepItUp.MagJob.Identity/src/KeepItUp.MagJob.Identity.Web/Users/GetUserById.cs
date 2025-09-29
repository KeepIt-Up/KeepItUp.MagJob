using KeepItUp.MagJob.Identity.Core.Interfaces;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries;
using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint to get a user by their identifier.
/// </summary>
/// <remarks>
/// Gets a user by their identifier.
/// </remarks>
public class GetUserById : BaseEndpoint<GetUserByIdRequest, UserDto>
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetUserById> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetUserById"/> class.
    /// </summary>
    /// <param name="mediator">Mediator.</param>
    /// <param name="logger">Logger.</param>
    public GetUserById(
        IMediator mediator,
        ILogger<GetUserById> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Configures the endpoint.
    /// </summary>
    protected override void ConfigureEndpoint()
    {
        Get(GetUserByIdRequest.Route);
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Gets a user";
            s.Description = "Gets a user by their identifier";
            s.ExampleRequest = new GetUserByIdRequest { Id = Guid.NewGuid() };
        });
    }

    /// <summary>
    /// Handles the GET /api/users/{id} request.
    /// </summary>
    /// <param name="req">Request.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Response containing the user data.</returns>
    protected override async Task<UserDto> HandleEndpointAsync(GetUserByIdRequest req, CancellationToken ct)
    {
        var query = new GetUserByIdQuery
        {
            Id = req.Id
        };

        return await _mediator.Send(query, ct);
    }
}
