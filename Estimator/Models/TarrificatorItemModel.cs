using Estimator.Domain;

namespace Estimator.Models;

public class TarrificatorItemModel:TarifficatorItem
{
    public string CategoryName { get; set; }
    public string SubCategoryName { get; set; }
}