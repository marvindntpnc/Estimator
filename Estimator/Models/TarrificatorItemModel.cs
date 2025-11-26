using Estimator.Domain;

namespace Estimator.Models;

public class TarrificatorItemModel:TarifficatorItem
{
    public string TarificatorItemTypeString{get;set;}
    public string MeasureString { get; set; }
    public string CurrencyString { get; set; }
    public string CategoryName { get; set; }
    public string SubCategoryName { get; set; }
    public decimal? CustomRate { get; set; }
}