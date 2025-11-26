namespace Estimator.Domain;

/// <summary>
/// Represents a contract for a specific facility.
/// Used to link estimates to the active customer contract.
/// </summary>
public class Contract:BaseEntity
{
    /// <summary>
    /// Contract number in human readable form.
    /// </summary>
    public string Number { get; set; }
    
    /// <summary>
    /// Date when the contract came into force.
    /// </summary>
    public DateTime StartDate { get; set; }
    
    /// <summary>
    /// Identifier of the facility for which the contract was signed.
    /// </summary>
    public int FacilityId { get; set; }
    
    /// <summary>
    /// Navigation property to the facility.
    /// </summary>
    public Facility Facility { get; set; }
}