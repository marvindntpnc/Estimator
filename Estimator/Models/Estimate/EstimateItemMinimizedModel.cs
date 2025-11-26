using Estimator.Domain.Enums;

namespace Estimator.Models.Estimate;

public class EstimateItemMinimizedModel
{
    public int EstimateId { get; set; }
    public int EstimateItemId{ get; set; }
    public decimal Qty{ get; set; }
    public decimal CustomRate { get; set; }
    public TarificatorItemType TarifficatorItemType { get; set; }
    public int ItemId{ get; set; }
}