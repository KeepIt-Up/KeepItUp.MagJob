using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;

namespace Organizations.Infrastructure.Common;

public class PaginationOptions
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string SortField { get; set; } = "Id";
    public bool Ascending { get; set; } = true;
}

public abstract record QueryWithPaginationOptions
{
    public PaginationOptions Options { get; init; } = new PaginationOptions();
}

public class PaginatedList<TSource, TDestination>(IEnumerable<TDestination> items, int count, int pageNumber, int pageSize)
{
    public IEnumerable<TDestination> Items { get; } = items;
    public int PageNumber { get; } = pageNumber;
    public int TotalPages { get; } = (int)Math.Ceiling(count / (double)pageSize);
    public int TotalCount { get; } = count;
    public int PageSize { get; } = pageSize;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    public static async Task<PaginatedList<TSource, TDestination>> CreateAsync(IQueryable<TSource> source, PaginationOptions options)
    {
        return await CreateAsync(source, options.PageNumber, options.PageSize, options.SortField, options.Ascending);
    }

    public static async Task<PaginatedList<TSource, TDestination>> CreateAsync(
        IQueryable<TSource> source,
        int pageNumber,
        int pageSize,
        string? sortField,
        bool ascending = true)
    {
        if (!string.IsNullOrEmpty(sortField))
        {
            var parameter = Expression.Parameter(typeof(TSource), "x");
            var property = Expression.Property(parameter, sortField);
            var lambda = Expression.Lambda(property, parameter);

            var methodName = ascending ? "OrderBy" : "OrderByDescending";
            var orderByMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(TSource), property.Type);

            source = orderByMethod.Invoke(null, [source, lambda]) as IQueryable<TSource> ?? throw new Exception();
        }

        var count = await source.CountAsync();
        var items = source.Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToList()
                               .Adapt<List<TDestination>>();

        return new PaginatedList<TSource, TDestination>(items, count, pageNumber, pageSize);
    }
}