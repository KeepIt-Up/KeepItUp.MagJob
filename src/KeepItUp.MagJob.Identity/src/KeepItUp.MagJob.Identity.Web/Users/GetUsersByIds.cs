using KeepItUp.MagJob.Identity.UseCases.Users.Queries.GetUserById;

namespace KeepItUp.MagJob.Identity.Web.Users;

/// <summary>
/// Endpoint do pobierania wielu użytkowników po listach identyfikatorów.
/// </summary>
public class GetUsersByIds : Endpoint<GetUsersByIdsRequest, GetUsersByIdsResponse>
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetUsersByIds> _logger;

    public GetUsersByIds(IMediator mediator, ILogger<GetUsersByIds> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override void Configure()
    {
        Post(GetUsersByIdsRequest.Route);
        AllowAnonymous();
        Description(b => b
            .WithName("GetUsersByIds")
            .Produces<GetUsersByIdsResponse>(200)
            .ProducesProblem(400)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Pobiera wielu użytkowników";
            s.Description = "Pobiera użytkowników na podstawie listy identyfikatorów";
        });
    }

    public override async Task HandleAsync(GetUsersByIdsRequest req, CancellationToken ct)
    {
        if (req.Ids == null || !req.Ids.Any())
        {
            await SendAsync(new GetUsersByIdsResponse { Users = new List<GetUsersByIdsResponse.UserDto>() }, cancellation: ct);
            return;
        }

        var users = new List<GetUsersByIdsResponse.UserDto>();

        foreach (var id in req.Ids)
        {
            try
            {
                var query = new GetUserByIdQuery { Id = id };
                var result = await _mediator.Send(query, ct);

                if (result.IsSuccess && result.Value != null)
                {
                    users.Add(new GetUsersByIdsResponse.UserDto
                    {
                        Id = result.Value.Id,
                        ExternalId = result.Value.ExternalId,
                        Email = result.Value.Email,
                        FirstName = result.Value.FirstName,
                        LastName = result.Value.LastName,
                        IsActive = result.Value.IsActive
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Nie udało się pobrać użytkownika o ID {UserId}", id);
            }
        }

        await SendAsync(new GetUsersByIdsResponse { Users = users }, cancellation: ct);
    }
}

