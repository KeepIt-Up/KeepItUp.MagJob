using Ardalis.Specification;
using KeepItUp.MagJob.Identity.Core.SharedKernel;
using KeepItUp.MagJob.Identity.UseCases.Common;

namespace KeepItUp.MagJob.Identity.Infrastructure.Data;

// inherit from Ardalis.Specification type
public class EfRepository<T>(AppDbContext dbContext) :
  RepositoryBase<T>(dbContext), IEfRepository<T>, IReadRepository<T>, IRepository<T> where T : BaseEntity, IAggregateRoot
{
  public async Task<IPaginatedResponse<T, TDestination>> ToPaginatedResponse<TDestination>(PaginationOptions options)
  {
    return await PaginatedResponse<T, TDestination>.CreateAsync(dbContext.Set<T>().AsQueryable(), options);
  }

  public async Task<IPaginatedResponse<T, TDestination>> ToPaginatedResponse<TDestination>(ISpecification<T> specification, PaginationOptions options)
  {
    return await PaginatedResponse<T, TDestination>.CreateAsync(dbContext.Set<T>().AsQueryable().WithSpecification(specification), options);
  }
}
