using Estimator.Domain.Enums;
using Estimator.Inerfaces;

namespace Estimator.Domain;

public class Estimate:BaseEntity
{
    public int Id { get; set; }
    /// <summary>
    /// Company inner document number
    /// </summary>
    public string? Number { get; set; }
    /// <summary>
    /// Date of creating
    /// </summary>
    public DateTime CreatedOn { get; set; }
    
    /// <summary>
    /// Currency exchange rate list on the day of the estimate formation
    /// </summary>
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public Dictionary<CurrencyType,decimal> CurrencyRate { get; set; }
    /// <summary>
    /// Is discounts included
    /// </summary>
    public bool IsDiscounts { get; set; }
    /// <summary>
    /// Customer full name
    /// </summary>
    public string? CustomerName { get; set; }
    public List<EstimateItem> EstimateItems { get; set; }
    public int FacilityId { get; set; }
    /// <summary>
    /// The name of fixing object (Address, name of building, etc.)
    /// </summary>
    public string? EstimateName { get; set; }
    public DateTime? ClosedOn { get; set; }
    public int ContractId { get; set; }
    public TarifficatorType? DiscountMaterials { get; set; }
}