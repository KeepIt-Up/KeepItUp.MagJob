using Ardalis.Specification;
using KeepItUp.MagJob.Identity.Core.SharedKernel;

namespace KeepItUp.MagJob.Identity.UseCases.Common;

public interface IEfRepository<TSource> where TSource : BaseEntity, IAggregateRoot
{
  Task<IPaginatedResponse<TSource, TDestination>> ToPaginatedResponse<TDestination>(PaginationOptions options);
  Task<IPaginatedResponse<TSource, TDestination>> ToPaginatedResponse<TDestination>(ISpecification<TSource> specification, PaginationOptions options);
}