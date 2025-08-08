namespace Estimator.Models.Estimate;

public class EstimateItemModel
{
    public int Id { get; set; }
    public int EstimateId { get; set; }
    public int Qty { get; set; }
    public decimal Total => Qty * TarifficatorItem.Price;
    public TarrificatorItemModel TarifficatorItem { get; set; }
}