using Estimator.Models.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Estimator.Models.EstimateForming;

public class EstimateFormingSearchModel:BaseSearchModel
{
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public int CategoryId { get; set; }
    public List<SelectListItem> AvailableCategories { get; set; } = new();
    public int ItemTypeId { get; set; }
    public List<SelectListItem> AvailableItemTypes { get; set; } = new();
    public int SubCategoryId { get; set; }
    public List<SelectListItem> AvailableSubcategories { get; set; } = new();
    public int CurrencyId { get; set; }
    public List<SelectListItem> AvailableCurrencies { get; set; } = new();
    public int MeasureTypeId { get; set; }
    public List<SelectListItem> AvailableMeasures { get; set; } = new();
}
