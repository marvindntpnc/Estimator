using System.ComponentModel.DataAnnotations.Schema;
using Estimator.Domain.Enums;
using Estimator.Inerfaces;

namespace Estimator.Domain;

/// <summary>
/// Represents a single item from an imported tarifficator (price list).
/// Contains pricing, measurement and classification information used to build estimates.
/// </summary>
public class TarifficatorItem:BaseEntity
{
    /// <summary>
    /// Internal identifier of the tarifficator item.
    /// </summary>
    public int Id{get;set;}
    
    /// <summary>
    /// Original code of the item from Excel tarifficator.
    /// </summary>
    public string ItemCode{get;set;}
    
    /// <summary>
    /// Identifier of the main category.
    /// </summary>
    public int CategoryId{get;set;}
    
    /// <summary>
    /// Identifier of the subcategory.
    /// </summary>
    public int SubcategoryId{get;set;}
    
    /// <summary>
    /// Human readable item name.
    /// </summary>
    public string Name{get;set;}
    
    /// <summary>
    /// Detailed description from tarifficator.
    /// </summary>
    public string Description{get;set;}
    
    /// <summary>
    /// Base price of the item in the specified currency.
    /// </summary>
    public decimal Price{get;set;}
    
    /// <summary>
    /// Currency of the item price.
    /// </summary>
    public CurrencyType CurrencyType{get;set;}
    
    /// <summary>
    /// Measure unit (pieces, hours, m2, etc.).
    /// </summary>
    public MeasureType Measure{get;set;}
    
    /// <summary>
    /// Type of the item (material or service).
    /// </summary>
    public TarificatorItemType TarificatorItemType{get;set;}
    
    /// <summary>
    /// Discount description or code, if present in the source file.
    /// </summary>
    public string Discount{get;set;}
    
    /// <summary>
    /// Type of tarifficator (FUL, KTO, etc.) from which the item was imported.
    /// </summary>
    public TarifficatorType TarifficatorType{get;set;}
    
    /// <summary>
    /// Logical deletion flag, used instead of physical removal from database.
    /// </summary>
    public bool IsDeleted{get;set;}
    
    /// <summary>
    /// Indicates whether the item is intended to be added with custom parameters on estimate editing screen.
    /// </summary>
    public bool IsCustomAdding { get; set; }
}