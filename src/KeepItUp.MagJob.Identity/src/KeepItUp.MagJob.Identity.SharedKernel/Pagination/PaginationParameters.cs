namespace KeepItUp.MagJob.Identity.SharedKernel.Pagination;

/// <summary>
/// Pagination parameters.
/// </summary>
public class PaginationParameters<T>
{
    private const int MaxPageSize = 100;
    private const int DefaultPageSize = 10;
    private int _pageSize = DefaultPageSize;

    /// <summary>
    /// Page number (indexed from 1).
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Number of elements per page.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value > MaxPageSize ? MaxPageSize : (value <= 0 ? DefaultPageSize : value);
    }

    /// <summary>
    /// Field to sort by.
    /// </summary>
    public string SortField { get; set; } = "Id";

    /// <summary>
    /// Whether to sort ascending.
    /// </summary>
    public bool Ascending { get; set; } = true;

    /// <summary>
    /// Validates and normalizes pagination parameters
    /// </summary>
    /// <returns>Validated and normalized pagination parameters</returns>
    public PaginationParameters<T> Validate()
    {
        // Ensure that the page number is greater than 0
        if (PageNumber <= 0)
        {
            PageNumber = 1;
        }

        // PageSize is already validated in the setter

        // Ensure that the sort field is not null
        SortField = string.IsNullOrWhiteSpace(SortField) ? "Id" : SortField;

        return this;
    }

    /// <summary>
    /// Creates a new instance of pagination parameters
    /// </summary>
    /// <param name="pageNumber">Page number</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="sortField">Sort field</param>
    /// <param name="ascending">Sorting direction</param>
    /// <returns>New instance of pagination parameters</returns>
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
