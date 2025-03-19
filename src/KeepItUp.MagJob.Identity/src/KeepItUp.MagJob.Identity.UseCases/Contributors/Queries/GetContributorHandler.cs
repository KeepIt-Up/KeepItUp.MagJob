using KeepItUp.MagJob.Identity.Core.ContributorAggregate.Repositories;

namespace KeepItUp.MagJob.Identity.UseCases.Contributors.Queries.GetContributor;

/// <summary>
/// Queries don't necessarily need to use repository methods, but they can if it's convenient
/// </summary>
public class GetContributorHandler(IContributorRepository _repository)
  : IQueryHandler<GetContributorQuery, Result<ContributorDTO>>
{
    public async Task<Result<ContributorDTO>> Handle(GetContributorQuery request, CancellationToken cancellationToken)
    {
        var entity = await _repository.GetByIdAsync(request.ContributorId, cancellationToken);
        if (entity == null) return Result.NotFound();

        return new ContributorDTO(entity.Id, entity.Name, entity.PhoneNumber?.Number ?? "");
    }
}
