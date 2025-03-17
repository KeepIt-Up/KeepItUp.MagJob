
namespace KeepItUp.MagJob.Identity.UseCases.Common;

public class PaginationOptions
{
  public int PageNumber { get; set; } = 1;
  public int PageSize { get; set; } = 10;
  public string SortField { get; set; } = "Id";
  public bool Ascending { get; set; } = true;
}
