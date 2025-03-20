namespace KeepItUp.MagJob.SharedKernel.Pagination;

/// <summary>
/// Interfejs dla zapytań obsługujących paginację, kompatybilny z MediatR.
/// </summary>
/// <typeparam name="TResult">Typ elementów wynikowych</typeparam>
public abstract class PaginationRequest<TResult>
{
    public PaginationParameters<TResult> PaginationParameters { get; set; } = new PaginationParameters<TResult>();
}
