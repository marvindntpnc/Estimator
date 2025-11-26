namespace Estimator.Domain;

/// <summary>
/// Represents a serviced facility or building where works are performed.
/// Contains location, addressing and pricing information.
/// </summary>
public class Facility:BaseEntity
{
    /// <summary>
    /// Facility name (short title).
    /// </summary>
    public string Name { get; set; }=string.Empty;
    
    /// <summary>
    /// Region or state name.
    /// </summary>
    public string StateName { get; set; }=string.Empty;
    
    /// <summary>
    /// Area or district name (optional).
    /// </summary>
    public string? AreaName { get; set; }
    
    /// <summary>
    /// City name.
    /// </summary>
    public string CityName { get; set; }=string.Empty;
    
    /// <summary>
    /// Street name or general address part.
    /// </summary>
    public string Address { get; set; }=string.Empty;
    
    /// <summary>
    /// House number.
    /// </summary>
    public string HouseNumber { get; set; }
    
    /// <summary>
    /// Enclosure (building section) number, if applicable.
    /// </summary>
    public string? EnclosureNumber { get; set; }
    
    /// <summary>
    /// Building number, if applicable.
    /// </summary>
    public string? BuildingNumber { get; set; }
    
    /// <summary>
    /// Base man-hour rate for works on this facility.
    /// </summary>
    public decimal HourRate { get; set; }
    
    /// <summary>
    /// Identifier of the active contract used by default for estimates.
    /// </summary>
    public int ActiveContractId { get; set; }
    
    /// <summary>
    /// All contracts associated with the facility.
    /// </summary>
    public List<Contract> ContractList { get; set; }
    
    /// <summary>
    /// Configured discount requirements based on grand totals.
    /// </summary>
    public List<DiscountRequirement> DiscountRequirements { get; set; }
}