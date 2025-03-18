using System.Linq.Expressions;
using KeepItUp.MagJob.Identity.Core.SharedKernel;
using KeepItUp.MagJob.Identity.UseCases.Common;
using Mapster;

namespace KeepItUp.MagJob.Identity.Infrastructure;

public class PaginatedResponse<TSource, TDestination>(IEnumerable<TDestination> items, int count, int pageNumber, int pageSize) : IPaginatedResponse<TSource, TDestination> where TSource : BaseEntity
{
    public IEnumerable<TDestination> Items { get; } = items;
    public int PageNumber { get; } = pageNumber;
    public int TotalPages { get; } = (int)Math.Ceiling(count / (double)pageSize);
    public int TotalCount { get; } = count;
    public int PageSize { get; } = pageSize;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedResponse<TSource, TDestination>> CreateAsync(IQueryable<TSource> source, PaginationOptions options)
    {
        if (options.PageNumber <= 0)
        {
            throw new ArgumentException("Page number must be greater than 0");
        }

        if (options.PageSize <= 0)
        {
            throw new ArgumentException("Page size must be greater than 0");
        }

        if (!string.IsNullOrEmpty(options.SortField))
        {
            var parameter = Expression.Parameter(typeof(TSource), "x");
            var property = Expression.Property(parameter, options.SortField);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = options.Ascending ? "OrderBy" : "OrderByDescending";
            var orderByMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TSource), property.Type);

            source = orderByMethod.Invoke(null, [source, lambda]) as IQueryable<TSource> ?? throw new Exception();
        }

        var count = await source.CountAsync();
        var items = source.Skip((options.PageNumber - 1) * options.PageSize)
                               .Take(options.PageSize)
                               .ToList()
                               .Adapt<List<TDestination>>();

        return new PaginatedResponse<TSource, TDestination>(items, count, options.PageNumber, options.PageSize);
    }
}
