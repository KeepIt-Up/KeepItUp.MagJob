using KeepItUp.MagJob.Identity.Core.ContributorAggregate;
using KeepItUp.MagJob.Identity.UseCases.Common;

namespace KeepItUp.MagJob.Identity.UseCases.Contributors.Queries.ListContributors;

public record ListContributorsQuery(PaginationOptions Options) : IQuery<Result<IPaginatedResponse<Contributor, ContributorDTO>>>;
