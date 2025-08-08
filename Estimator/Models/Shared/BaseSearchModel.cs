namespace Estimator.Models.Shared;

public class BaseSearchModel
{
    public int PageIndex { get; set; } = 0;
    public int PageSize { get; set; } = 100;
}