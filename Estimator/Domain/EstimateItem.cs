using Estimator.Inerfaces;

namespace Estimator.Domain;

public class EstimateItem:IEntity
{
    public int Id { get; set; }
    public Estimate Estimate { get; set; }
    public int EstimateId { get; set; }
    public int TarifficatorItemId { get; set; }
    public int Qty { get; set; }
}