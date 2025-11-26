using Estimator.Domain.Enums;

namespace Estimator.Domain;

/// <summary>
/// Stores currency exchange rate snapshot for a specific estimate.
/// Used for correct recalculation of foreign currency prices to RUB.
/// </summary>
public class EstimateCurrencyRate:BaseEntity
{
    /// <summary>
    /// Identifier of the estimate for which the rate is stored.
    /// </summary>
    public int EstimateId { get; set; }
    
    /// <summary>
    /// Currency type.
    /// </summary>
    public CurrencyType CurrencyType { get; set; }
    
    /// <summary>
    /// Exchange rate value against RUB on the estimate date.
    /// </summary>
    public decimal Rate { get; set; }
}