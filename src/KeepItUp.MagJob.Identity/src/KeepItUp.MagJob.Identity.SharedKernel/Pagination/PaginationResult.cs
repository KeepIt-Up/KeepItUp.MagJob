namespace KeepItUp.MagJob.Identity.SharedKernel.Pagination;

/// <summary>
/// Represents a paginated result of a query.
/// </summary>
/// <typeparam name="T">Type of the collection items</typeparam>
public class PaginationResult<T>
{
    /// <summary>
    /// List of items on the current page.
    /// </summary>
    public List<T> Items { get; private set; }

    /// <summary>
    /// Total number of items satisfying the criteria.
    /// </summary>
    public int TotalCount { get; private set; }

    /// <summary>
    /// Number of pages.
    /// </summary>
    public int TotalPages { get; private set; }

    /// <summary>
    /// Current page number.
    /// </summary>
    public int PageNumber { get; private set; }

    /// <summary>
    /// Page size.
    /// </summary>
    public int PageSize { get; private set; }

    /// <summary>
    /// Whether there is a previous page.
    /// </summary>
    public bool HasPrevious => PageNumber > 1;

    /// <summary>
    /// Whether there is a next page.
    /// </summary>
    public bool HasNext => PageNumber < TotalPages;

    /// <summary>
    /// Field by which the results are sorted.
    /// </summary>
    public string SortField { get; private set; }

    /// <summary>
    /// Whether the sorting is ascending.
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
    /// Creates a new paginated result.
    /// </summary>
    /// <param name="items">Items on the current page</param>
    /// <param name="totalCount">Total number of items</param>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="sortField">Sort field</param>
    /// <param name="ascending">Sorting direction</param>
    /// <returns>New PaginationResult object</returns>
    public static PaginationResult<T> Create(List<T> items, int totalCount, int pageNumber, int pageSize, string sortField, bool ascending)
    {
        return new PaginationResult<T>(items, totalCount, pageNumber, pageSize, sortField, ascending);
    }

    /// <summary>
    /// Creates a new paginated result based on pagination parameters.
    /// </summary>
    /// <param name="items">Items on the current page</param>
    /// <param name="totalCount">Total number of items</param>
    /// <param name="parameters">Pagination parameters</param>
    /// <returns>New PaginationResult object</returns>
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
    /// Creates an empty paginated result.
    /// </summary>
    /// <returns>Empty PaginationResult object</returns>
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
