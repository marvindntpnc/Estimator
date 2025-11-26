using System.ComponentModel.DataAnnotations.Schema;
using Estimator.Inerfaces;

namespace Estimator.Domain;

/// <summary>
/// Represents a single line of an estimate linked to a tarifficator item.
/// Stores quantity and custom rate used for total calculation.
/// </summary>
public class EstimateItem:BaseEntity
{
    /// <summary>
    /// Internal identifier of the estimate item.
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Navigation property to the parent estimate.
    /// </summary>
    public Estimate Estimate { get; set; }
    
    /// <summary>
    /// Identifier of the parent estimate.
    /// </summary>
    public int EstimateId { get; set; }
    
    /// <summary>
    /// Identifier of the underlying tarifficator item.
    /// </summary>
    public int TarifficatorItemId { get; set; }
    
    /// <summary>
    /// Quantity of the item in estimate units.
    /// </summary>
    public decimal Qty { get; set; }
    [Column(TypeName = "decimal(18,5)")]
    /// <summary>
    /// Custom coefficient for price calculation (1 - default, &gt;1 - customized).
    /// Used to adjust base tarifficator price.
    /// </summary>
    public decimal CustomRate { get; set; }//TODO change adding: 1 (default value) and >1 if it's customized. And change Item Price by "TarifficatorDefaultPrice * CustomRate" 
}