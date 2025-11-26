namespace Estimator.Domain;

/// <summary>
/// Tarifficator Item Category.
/// Using for Primary and Subcategories.
/// </summary>
public class Category:BaseEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentCategoryId { get; set; }
}