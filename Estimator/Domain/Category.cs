using Estimator.Inerfaces;

namespace Estimator.Domain;

public class Category:IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int? ParentCategoryId { get; set; }
}