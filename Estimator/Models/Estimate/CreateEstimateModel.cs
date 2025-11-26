using Estimator.Domain.Enums;

namespace Estimator.Models.Estimate;

public class CreateEstimateModel
{
    public List<EstimateItemMinimizedModel> Items { get; set; } = new();
    public string Title { get; set; } = string.Empty;
    public string Number { get; set; } = string.Empty;
    public Dictionary<CurrencyType, decimal> CurrencyRates { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public bool IsDiscounts { get; set; } = false;
}