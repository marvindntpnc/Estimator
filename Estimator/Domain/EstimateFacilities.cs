namespace Estimator.Domain;

/// <summary>
/// Join entity between estimate and facility in case of many-to-many relationship.
/// </summary>
public class EstimateFacilities
{
    /// <summary>
    /// Identifier of the estimate.
    /// </summary>
    public int EstimateId { get; set; }
    
    /// <summary>
    /// Identifier of the facility.
    /// </summary>
    public int FacilityId { get; set; }
}