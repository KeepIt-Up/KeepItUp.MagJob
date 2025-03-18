
namespace KeepItUp.MagJob.Identity.UseCases.Common;

public abstract class IQueryWithPaginationOptions
{
    public PaginationOptions Options { get; init; } = new PaginationOptions();
}

