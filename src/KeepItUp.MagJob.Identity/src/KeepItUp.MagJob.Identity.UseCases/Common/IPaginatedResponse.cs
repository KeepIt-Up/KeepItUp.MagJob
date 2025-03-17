
using KeepItUp.MagJob.Identity.Core.SharedKernel;

namespace KeepItUp.MagJob.Identity.UseCases.Common;

public interface IPaginatedResponse<TSource, TDestination> where TSource : BaseEntity
{
  IEnumerable<TDestination> Items { get; }
  int PageNumber { get; }
  int TotalPages { get; }
  int TotalCount { get; }
  int PageSize { get; }
  bool HasPreviousPage { get; }
  bool HasNextPage { get; }
}