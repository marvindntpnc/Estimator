namespace Estimator.Domain;

/// <summary>
/// Base type for all entities with integer identifier.
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// Primary key identifier.
    /// </summary>
    public int Id { get; set; }
}