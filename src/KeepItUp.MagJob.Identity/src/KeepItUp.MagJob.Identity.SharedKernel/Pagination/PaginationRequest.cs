namespace KeepItUp.MagJob.Identity.SharedKernel.Pagination;

/// <summary>
/// Interface for queries supporting pagination, compatible with MediatR.
/// </summary>
/// <typeparam name="TResult">Type of the result items</typeparam>
public abstract class PaginationRequest<TResult>
{
    public PaginationParameters<TResult> PaginationParameters { get; set; } = new PaginationParameters<TResult>();
}
