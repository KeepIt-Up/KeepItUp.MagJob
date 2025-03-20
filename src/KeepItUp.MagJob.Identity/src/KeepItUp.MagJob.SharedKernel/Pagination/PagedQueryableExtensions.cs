using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace KeepItUp.MagJob.SharedKernel.Pagination;

/// <summary>
/// Rozszerzenia dla IQueryable do obsługi paginacji i sortowania.
/// </summary>
public static class PagedQueryableExtensions
{
    /// <summary>
    /// Tworzy stronicowany wynik na podstawie IQueryable oraz parametrów paginacji.
    /// </summary>
    /// <typeparam name="TEntity">Typ encji z bazy danych</typeparam>
    /// <typeparam name="TDto">Typ DTO</typeparam>
    /// <param name="queryable">Zapytanie do bazy danych</param>
    /// <param name="selector">Funkcja mapująca z encji na DTO</param>
    /// <param name="parameters">Parametry paginacji</param>
    /// <returns>Stronicowany wynik</returns>
    public static PaginationResult<TDestination> ToPaginationResult<TSource, TDestination>(
        this IQueryable<TSource> queryable,
        Expression<Func<TSource, TDestination>> selector,
        PaginationParameters<TDestination> parameters)
    {

        // Mapujemy queryable<TSource> na queryable<TDestination>
        // Konwersja musi nastąpić na tym etapie, aby sortowanie i paginacja były wykonywane na TDestination
        IQueryable<TDestination> destinationQueryable = queryable.Select(selector);

        // Pobieramy całkowitą liczbę elementów z źródłowego IQueryable
        var totalCount = destinationQueryable.Count();

        if (totalCount == 0)
        {
            return PaginationResult<TDestination>.Create(new List<TDestination>(), 0, parameters);
        }

        // Dynamiczne sortowanie
        IQueryable<TDestination> sortedQuery = ApplySorting(destinationQueryable, parameters);

        // Stosujemy paginację
        var pagedQuery = sortedQuery
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);

        // Mapujemy na DTO i pobieramy wynik
        var items = pagedQuery.ToList();

        // Tworzymy stronicowany wynik
        return PaginationResult<TDestination>.Create(items, totalCount, parameters);
    }

    /// <summary>
    /// Asynchronicznie tworzy stronicowany wynik na podstawie IQueryable oraz parametrów paginacji.
    /// </summary>
    /// <typeparam name="TSource">Typ źródłowy encji</typeparam>
    /// <typeparam name="TDestination">Typ docelowy DTO</typeparam>
    /// <param name="queryable">Źródłowe zapytanie IQueryable</param>
    /// <param name="selector">Wyrażenie mapujące z TSource na TDestination</param>
    /// <param name="parameters">Parametry paginacji</param>
    /// <param name="countAsync">Funkcja do asynchronicznego liczenia elementów</param>
    /// <param name="toListAsync">Funkcja do asynchronicznego pobierania listy elementów</param>
    /// <param name="cancellationToken">Token anulowania</param>
    /// <returns>Stronicowany wynik</returns>
    public static async Task<PaginationResult<TDestination>> ToPaginationResultAsync<TSource, TDestination>(
        this IQueryable<TSource> queryable,
        Expression<Func<TSource, TDestination>> selector,
        PaginationParameters<TDestination> parameters,
        CancellationToken cancellationToken = default)
    {


        // Mapujemy queryable<TSource> na queryable<TDestination>
        // Konwersja musi nastąpić na tym etapie, aby sortowanie i paginacja były wykonywane na TDestination
        IQueryable<TDestination> destinationQueryable = queryable.Select(selector);

        // Pobieramy całkowitą liczbę elementów z źródłowego IQueryable
        var totalCount = await queryable.CountAsync(cancellationToken);

        if (totalCount == 0)
        {
            return PaginationResult<TDestination>.Create(new List<TDestination>(), 0, parameters);
        }

        // Sortowanie musi być wykonywane na TDestination, ponieważ pole SortField pochodzi z TDestination
        // i może nie istnieć w TSource
        var sortedQuery = ApplySorting(destinationQueryable, parameters);

        // Stosujemy paginację na posortowanym queryable<TDestination>
        var pagedQuery = sortedQuery
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize);

        // Pobieramy wynik
        var items = await pagedQuery.ToListAsync(cancellationToken);

        // Tworzymy stronicowany wynik
        return PaginationResult<TDestination>.Create(items, totalCount, parameters);
    }

    private static IQueryable<TEntity> ApplySorting<TEntity>(IQueryable<TEntity> queryable, PaginationParameters<TEntity> parameters)
    {
        // Domyślna implementacja sortowania oparta na refleksji i dynamicznym LINQ
        var type = typeof(TEntity);
        var property = type.GetProperty(parameters.SortField,
            BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (property == null)
        {
            // Próbujemy znaleźć domyślną właściwość Id
            property = type.GetProperty("Id",
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (property != null)
            {
                parameters.SortField = "Id";
            }
            else
            {
                // Jeśli nie znaleziono pola do sortowania, zwracamy oryginalne zapytanie
                return queryable;
            }
        }

        // Tworzymy parametr dla wyrażenia lambda
        var parameter = Expression.Parameter(type, "x");

        // Tworzymy wyrażenie dostępu do właściwości
        var propertyAccess = Expression.Property(parameter, property);

        // Tworzymy wyrażenie lambda do sortowania
        var lambda = Expression.Lambda(propertyAccess, parameter);

        // Tworzymy wywołanie metody OrderBy lub OrderByDescending
        var methodName = parameters.Ascending ? "OrderBy" : "OrderByDescending";

        // Znajdujemy odpowiednią metodę
        var methods = typeof(Queryable).GetMethods()
            .Where(m => m.Name == methodName && m.IsGenericMethodDefinition && m.GetParameters().Length == 2);

        var method = methods.FirstOrDefault()?.MakeGenericMethod(type, property.PropertyType);

        if (method == null)
        {
            return queryable;
        }

        // Wywołujemy metodę OrderBy na zapytaniu
        try
        {
            return (IQueryable<TEntity>)method.Invoke(null!, new object[] { queryable, lambda })!;
        }
        catch
        {
            // W przypadku błędu zwracamy oryginalne zapytanie
            return queryable;
        }
    }
}
