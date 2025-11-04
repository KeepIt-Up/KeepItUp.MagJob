using Ardalis.Result;
using FastEndpoints;
using MediatR;
using Microsoft.Extensions.Logging;
using KeepItUp.MagJob.Identity.UseCases.Members.Queries.GetMemberEmail;

namespace KeepItUp.MagJob.Identity.Web.Members;

/// <summary>
/// Request do pobierania emaila członka.
/// </summary>
public class GetMemberEmailRequest
{
    public const string Route = "/api/members/{MemberId}/email";
    public static string BuildRoute(Guid memberId) => Route.Replace("{MemberId}", memberId.ToString());

    public Guid MemberId { get; set; }
}

/// <summary>
/// Response z emailem członka.
/// </summary>
public class GetMemberEmailResponse
{
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Endpoint do pobierania emaila członka organizacji.
/// </summary>
public class GetMemberEmail : Endpoint<GetMemberEmailRequest, GetMemberEmailResponse>
{
    private readonly IMediator _mediator;
    private readonly ILogger<GetMemberEmail> _logger;

    public GetMemberEmail(IMediator mediator, ILogger<GetMemberEmail> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public override void Configure()
    {
        Get(GetMemberEmailRequest.Route);
        AllowAnonymous(); // Adjust based on your security requirements
        Description(b => b
            .WithName("GetMemberEmail")
            .Produces<GetMemberEmailResponse>(200)
            .ProducesProblem(404)
            .ProducesProblem(500));
        Summary(s =>
        {
            s.Summary = "Pobiera email członka organizacji";
            s.Description = "Pobiera adres email użytkownika przypisanego do członka organizacji";
            s.ExampleRequest = new GetMemberEmailRequest { MemberId = Guid.NewGuid() };
        });
    }

    public override async Task HandleAsync(GetMemberEmailRequest req, CancellationToken ct)
    {
        var query = new GetMemberEmailQuery
        {
            MemberId = req.MemberId
        };

        var result = await _mediator.Send(query, ct);

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

        var response = new GetMemberEmailResponse
        {
            Email = result.Value
        };

        await SendOkAsync(response, ct);
    }
}