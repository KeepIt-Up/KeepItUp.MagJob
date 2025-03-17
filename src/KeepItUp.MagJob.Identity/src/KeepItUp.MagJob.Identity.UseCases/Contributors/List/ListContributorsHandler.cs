using KeepItUp.MagJob.Identity.Core.ContributorAggregate;
using KeepItUp.MagJob.Identity.UseCases.Common;

namespace KeepItUp.MagJob.Identity.UseCases.Contributors.List;

public class ListContributorsHandler(IEfRepository<Contributor> _repository)
  : IQueryHandler<ListContributorsQuery, Result<IPaginatedResponse<Contributor, ContributorDTO>>>
{
  public async Task<Result<IPaginatedResponse<Contributor, ContributorDTO>>> Handle(ListContributorsQuery request, CancellationToken cancellationToken)
  {
    var result = await _repository.ToPaginatedResponse<ContributorDTO>(request.Options);

    return Result.Success(result);
  }
}
