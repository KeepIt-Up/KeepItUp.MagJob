
namespace KeepItUp.MagJob.SharedKernel.Pagination;

/// <summary>
/// Interfejs dla zapytań obsługujących paginację, kompatybilny z MediatR.
/// </summary>
/// <typeparam name="TResult">Typ elementów wynikowych</typeparam>
public abstract class PaginationQuery<TResult> : IQuery<Result<PaginationResult<TResult>>>
{
    public PaginationParameters<TResult> PaginationParameters { get; set; } = new PaginationParameters<TResult>();
}
