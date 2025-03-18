
using KeepItUp.MagJob.Identity.UseCases.Common;

namespace KeepItUp.MagJob.Identity.Web.Common;
public class RequestWithPagination
{
    [QueryParam]
    public PaginationOptions Options { get; init; } = new PaginationOptions();
}
