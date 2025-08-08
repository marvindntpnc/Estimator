using Estimator.Domain.Enums;

namespace Estimator.Domain;

public class Tarifficator
{
    public int Id { get; set; }
    public TarifficatorType TarifficatorType{get;set;}
    public List<TarifficatorItem> TarifficatorItems { get; set; } 
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
}