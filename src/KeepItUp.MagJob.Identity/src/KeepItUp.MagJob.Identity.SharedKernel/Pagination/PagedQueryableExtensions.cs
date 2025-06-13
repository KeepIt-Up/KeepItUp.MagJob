using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.Identity.SharedKernel.Pagination;

/// <summary>
/// Extensions for IQueryable to support pagination and sorting.
/// </summary>
public static class PagedQueryableExtensions
{
    /// <summary>
    /// Creates a paginated result based on IQueryable and pagination parameters.
    /// </summary>
    /// <typeparam name="TEntity">Type of the entity from the database</typeparam>
    /// <typeparam name="TDto">Type of the DTO</typeparam>
    /// <param name="queryable">Query to the database</param>
    /// <param name="selector">Function mapping from entity to DTO</param>
    /// <param name="parameters">Pagination parameters</param>
    /// <returns>Paginated result</returns>
    public static PaginationResult<TDestination> ToPaginationResult<TSource, TDestination>(
        this IQueryable<TSource> queryable,
        Expression<Func<TSource, TDestination>> selector,
        PaginationParameters<TDestination> parameters)
    {

        // Map queryable<TSource> to queryable<TDestination>
        // Conversion must happen at this stage to ensure sorting and pagination are performed on TDestination
        IQueryable<TDestination> destinationQueryable = queryable.Select(selector);

        // Get the total number of elements from the source IQueryable
        var totalCount = destinationQueryable.Count();

        if (totalCount == 0)
        {
            return PaginationResult<TDestination>.Create(new List<TDestination>(), 0, parameters);
        }

        // Dynamic sorting
        IQueryable<TDestination> sortedQuery = ApplySorting(destinationQueryable, parameters);

        // Apply pagination
        var pagedQuery = sortedQuery
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);

        // Map to DTO and get the result
        var items = pagedQuery.ToList();

        // Create a paginated result
        return PaginationResult<TDestination>.Create(items, totalCount, parameters);
    }

    /// <summary>
    /// Asynchronously creates a paginated result based on IQueryable and pagination parameters.
    /// </summary>
    /// <typeparam name="TSource">Type of the source entity</typeparam>
    /// <typeparam name="TDestination">Type of the destination DTO</typeparam>
    /// <param name="queryable">Source query IQueryable</param>
    /// <param name="selector">Expression mapping from TSource to TDestination</param>
    /// <param name="parameters">Pagination parameters</param>
    /// <param name="countAsync">Function to asynchronously count elements</param>
    /// <param name="toListAsync">Function to asynchronously get a list of elements</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated result</returns>
    public static async Task<PaginationResult<TDestination>> ToPaginationResultAsync<TSource, TDestination>(
        this IQueryable<TSource> queryable,
        Expression<Func<TSource, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default)
    {


        // Map queryable<TSource> to queryable<TDestination>
        // Conversion must happen at this stage to ensure sorting and pagination are performed on TDestination
        IQueryable<TDestination> destinationQueryable = queryable.Select(selector);

        // Get the total number of elements from the source IQueryable
        var totalCount = await queryable.CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return PaginationResult<TDestination>.Create(new List<TDestination>(), 0, parameters);
        }

        // Sorting must be performed on TDestination, because the SortField property comes from TDestination
        // and may not exist in TSource
        var sortedQuery = ApplySorting(destinationQueryable, parameters);

        // Apply pagination on the sorted queryable<TDestination>
        var pagedQuery = sortedQuery
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);

        // Get the result
        var items = await pagedQuery.ToListAsync(cancellationToken);

        // Create a paginated result
        return PaginationResult<TDestination>.Create(items, totalCount, parameters);
    }

    private static IQueryable<TEntity> ApplySorting<TEntity>(IQueryable<TEntity> queryable, PaginationParameters<TEntity> parameters)
    {
        // Default implementation of sorting based on reflection and dynamic LINQ
        var type = typeof(TEntity);
        var property = type.GetProperty(parameters.SortField,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
        {
            // Try to find the default Id property
            property = type.GetProperty("Id",
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                parameters.SortField = "Id";
            }
            else
            {
                // If no sorting field is found, return the original query
                return queryable;
            }
        }

        // Create a parameter for the lambda expression
        var parameter = Expression.Parameter(type, "x");

        // Create an expression to access the property
        var propertyAccess = Expression.Property(parameter, property);

        // Create a lambda expression for sorting
        var lambda = Expression.Lambda(propertyAccess, parameter);

        // Create a method call for OrderBy or OrderByDescending
        var methodName = parameters.Ascending ? "OrderBy" : "OrderByDescending";

        // Find the appropriate method
        var methods = typeof(Queryable).GetMethods()
            .Where(m => m.Name == methodName && m.IsGenericMethodDefinition && m.GetParameters().Length == 2);

        var method = methods.FirstOrDefault()?.MakeGenericMethod(type, property.PropertyType);

        if (method == null)
        {
            return queryable;
        }

        // Call the OrderBy method on the query
        try
        {
            return (IQueryable<TEntity>)method.Invoke(null!, new object[] { queryable, lambda })!;
        }
        catch
        {
            // In case of an error, return the original query
            return queryable;
        }
    }
}
