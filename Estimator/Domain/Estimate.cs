using Estimator.Inerfaces;

namespace Estimator.Domain;

public class Estimate:IEntity
{
    public int Id { get; set; }
    public string Number { get; set; }
    public DateTime Date { get; set; }
    public string LocationName { get; set; }
    public decimal CurrencyRate { get; set; }
    public bool IsDiscounts { get; set; }
    public string CustomerName { get; set; }
    public List<EstimateItem> EstimateItems { get; set; }
}