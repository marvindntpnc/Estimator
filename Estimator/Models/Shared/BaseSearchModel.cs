namespace Estimator.Models.Shared;

public class BaseSearchModel
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 25;

    // DataTables-specific bindings
    public int Draw { get; set; } = 0;
    public int Start { get; set; } = 0;
    public int Length { get; set; } = 25;
    public string SearchValue { get; set; }
    public int? OrderColumnIndex { get; set; }
    public string OrderColumnData { get; set; }
    public string OrderDirection { get; set; } // "asc" | "desc"
}