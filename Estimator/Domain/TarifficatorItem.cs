using System.ComponentModel.DataAnnotations.Schema;
using Estimator.Domain.Enums;
using Estimator.Inerfaces;

namespace Estimator.Domain;

public class TarifficatorItem:IEntity
{
    public int Id{get;set;}
    public string ItemCode{get;set;}
    public int CategoryId{get;set;}
    public int SubcategoryId{get;set;}
    public string Name{get;set;}
    public string Description{get;set;}
    public decimal Price{get;set;}
    public CurrencyType CurrencyType{get;set;}
    public MeasureType Measure{get;set;}
    public TarificatorItemType TarificatorItemType{get;set;}
    public string Discount{get;set;}
    public TarifficatorType TarifficatorType{get;set;}
}