using KeepItUp.MagJob.Identity.Core.ContributorAggregate;
using KeepItUp.MagJob.Identity.UseCases.Common;
using KeepItUp.MagJob.Identity.UseCases.Contributors;
using KeepItUp.MagJob.Identity.UseCases.Contributors.List;
using KeepItUp.MagJob.Identity.Web.Common;

namespace KeepItUp.MagJob.Identity.Web.Contributors;

/// <summary>
/// List all Contributors
/// </summary>
/// <remarks>
/// List all contributors - returns a ContributorListResponse containing the Contributors.
/// </remarks>
public class List(IMediator _mediator) : Endpoint<RequestWithPagination, IPaginatedResponse<Contributor, ContributorDTO>>
{
    public override void Configure()
    {
        Get("/Contributors");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RequestWithPagination request, CancellationToken cancellationToken)
    {
        Result<IPaginatedResponse<Contributor, ContributorDTO>> result = await _mediator.Send(new ListContributorsQuery(request.Options), cancellationToken);

        if (result.IsSuccess)
        {
            Response = result.Value;
        }
    }
}
