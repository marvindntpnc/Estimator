using Estimator.Domain.Enums;
using Estimator.Inerfaces;

namespace Estimator.Domain;

public class Tarifficator:IEntity
{
    public int Id { get; set; }
    public TarifficatorType TarifficatorType{get;set;}
    public List<TarifficatorItem> TarifficatorItems { get; set; } 
    public bool IsActive { get; set; }
    public DateTime CreatedOn { get; set; }
}