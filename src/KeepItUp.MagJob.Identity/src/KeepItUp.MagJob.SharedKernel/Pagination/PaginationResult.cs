namespace KeepItUp.MagJob.SharedKernel.Pagination;

/// <summary>
/// Reprezentuje stronicowany wynik zapytania.
/// </summary>
/// <typeparam name="T">Typ elementów kolekcji</typeparam>
public class PaginationResult<T>
{
    /// <summary>
    /// Lista elementów na aktualnej stronie.
    /// </summary>
    public List<T> Items { get; private set; }

    /// <summary>
    /// Całkowita liczba elementów spełniających kryteria.
    /// </summary>
    public int TotalCount { get; private set; }

    /// <summary>
    /// Liczba stron.
    /// </summary>
    public int TotalPages { get; private set; }

    /// <summary>
    /// Aktualny numer strony.
    /// </summary>
    public int PageNumber { get; private set; }

    /// <summary>
    /// Rozmiar strony.
    /// </summary>
    public int PageSize { get; private set; }

    /// <summary>
    /// Czy istnieje poprzednia strona.
    /// </summary>
    public bool HasPrevious => PageNumber > 1;

    /// <summary>
    /// Czy istnieje następna strona.
    /// </summary>
    public bool HasNext => PageNumber < TotalPages;

    /// <summary>
    /// Pole, po którym posortowano wyniki.
    /// </summary>
    public string SortField { get; private set; }

    /// <summary>
    /// Czy sortowanie jest rosnące.
    /// </summary>
    public bool Ascending { get; private set; }

    private PaginationResult(List<T> items, int totalCount, int pageNumber, int pageSize, string sortField, bool ascending)
    {
        Items = items;
        TotalCount = totalCount;
        PageNumber = pageNumber;
        PageSize = pageSize;
        SortField = sortField;
        Ascending = ascending;
        TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
    }

    /// <summary>
    /// Tworzy nowy stronicowany wynik.
    /// </summary>
    /// <param name="items">Elementy na aktualnej stronie</param>
    /// <param name="totalCount">Całkowita liczba elementów</param>
    /// <param name="pageNumber">Numer strony</param>
    /// <param name="pageSize">Rozmiar strony</param>
    /// <param name="sortField">Pole sortowania</param>
    /// <param name="ascending">Kierunek sortowania</param>
    /// <returns>Nowy obiekt PagedResult</returns>
    public static PaginationResult<T> Create(List<T> items, int totalCount, int pageNumber, int pageSize, string sortField, bool ascending)
    {
        return new PaginationResult<T>(items, totalCount, pageNumber, pageSize, sortField, ascending);
    }

    /// <summary>
    /// Tworzy nowy stronicowany wynik na podstawie parametrów paginacji.
    /// </summary>
    /// <param name="items">Elementy na aktualnej stronie</param>
    /// <param name="totalCount">Całkowita liczba elementów</param>
    /// <param name="parameters">Parametry paginacji</param>
    /// <returns>Nowy obiekt PagedResult</returns>
    public static PaginationResult<T> Create(List<T> items, int totalCount, PaginationParameters<T> parameters)
    {
        return new PaginationResult<T>(
            items,
            totalCount,
            parameters.PageNumber,
            parameters.PageSize,
            parameters.SortField,
            parameters.Ascending);
    }

    /// <summary>
    /// Tworzy pusty stronicowany wynik.
    /// </summary>
    /// <returns>Pusty obiekt PagedResult</returns>
    public static PaginationResult<T> Empty()
    {
        return new PaginationResult<T>(
            new List<T>(),
            0,
            1,
            10,
            "Id",
            true);
    }
}
