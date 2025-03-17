using KeepItUp.MagJob.Identity.Core.ContributorAggregate;
using KeepItUp.MagJob.Identity.UseCases.Common;

namespace KeepItUp.MagJob.Identity.UseCases.Contributors.List;

public record ListContributorsQuery(PaginationOptions Options) : IQuery<Result<IPaginatedResponse<Contributor, ContributorDTO>>>;
