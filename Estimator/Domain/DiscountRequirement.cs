namespace Estimator.Domain;

/// <summary>
/// Represents discount conditions for a facility, applied to KTO materials totals.
/// Contains percentage rates and amount ranges for discount calculation.
/// </summary>
public class DiscountRequirement:BaseEntity
{
    /// <summary>
    /// Identifier of the facility for which the discount rule is configured.
    /// </summary>
    public int FacilityId { get; set; }
    
    /// <summary>
    /// Lower bound of the grand total range (inclusive) for which the rule is valid.
    /// </summary>
    public decimal StartRange { get; set; }
    
    /// <summary>
    /// Upper bound of the grand total range (inclusive) for which the rule is valid.
    /// </summary>
    public decimal EndRange { get; set; }
    
    /// <summary>
    /// Uninstall work percentage applied to KTO materials total.
    /// </summary>
    public decimal UninstallRate { get; set; }
    
    /// <summary>
    /// Install work percentage applied to KTO materials total.
    /// </summary>
    public decimal InstallRate { get; set; }
    
    /// <summary>
    /// Supplies percentage applied to KTO materials total.
    /// </summary>
    public decimal SuppliesRate { get; set; }
    
    /// <summary>
    /// Navigation property to the facility for which the rule is configured.
    /// </summary>
    public Facility Facility { get; set; }
}