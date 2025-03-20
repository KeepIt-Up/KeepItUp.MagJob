using System;

namespace KeepItUp.MagJob.SharedKernel.Pagination;

/// <summary>
/// Parametry paginacji.
/// </summary>
public class PaginationParameters<T>
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;
    private int _pageSize = DefaultPageSize;

    /// <summary>
    /// Numer strony (indeksowany od 1).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Liczba elementów na stronie.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : (value <= 0 ? DefaultPageSize : value);
    }

    /// <summary>
    /// Pole, po którym sortować.
    /// </summary>
    public string SortField { get; set; } = "Id";

    /// <summary>
    /// Czy sortować rosnąco.
    /// </summary>
    public bool Ascending { get; set; } = true;

    /// <summary>
    /// Waliduje i normalizuje parametry paginacji
    /// </summary>
    /// <returns>Zwalidowane i znormalizowane parametry paginacji</returns>
    public PaginationParameters<T> Validate()
    {
        // Zapewnia, że numer strony jest większy od 0
        if (PageNumber <= 0)
        {
            PageNumber = 1;
        }

        // PageSize jest już walidowany w setterze

        // Upewnia się, że pole sortowania nie jest null
        SortField = string.IsNullOrWhiteSpace(SortField) ? "Id" : SortField;

        return this;
    }

    /// <summary>
    /// Tworzy nową instancję parametrów paginacji
    /// </summary>
    /// <param name="pageNumber">Numer strony</param>
    /// <param name="pageSize">Rozmiar strony</param>
    /// <param name="sortField">Pole sortowania</param>
    /// <param name="ascending">Kierunek sortowania</param>
    /// <returns>Nowa instancja parametrów paginacji</returns>
    public static PaginationParameters<T> Create(int pageNumber = 1, int pageSize = DefaultPageSize, string sortField = "Id", bool ascending = true)
    {
        return new PaginationParameters<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortField = sortField,
            Ascending = ascending
        }.Validate();
    }
}
