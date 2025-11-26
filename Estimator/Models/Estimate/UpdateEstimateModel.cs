using Estimator.Models.EstimateForming;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Estimator.Models.Estimate;

public class UpdateEstimateModel
{
    public int EstimateId { get; set; }
    public string? EstimateNumber { get; set; }
    public string? EstimateName { get; set; }
    public DateTime EstimateDate { get; set; }
    public DateTime? EstimateClosedDate { get; set; }
    public int FacilityId { get; set; }
    public string? FacilityName { get; set; }
    public decimal? FacilityHourRate { get; set; }
    public int ContractId { get; set; }
    public string? ContractCompiledName { get; set; }
    public bool IsDiscounts { get; set; }
    public decimal? EurRate { get; set; }
    public decimal? UsdRate { get; set; }
    public decimal? CnyRate { get; set; }
    public EstimateItemSearchModel? Items { get; set; }
    public EstimateFormingSearchModel? EstimateFormingSearchModel { get; set; }
    public List<SelectListItem>? AvailableFacilityList { get; set; }
    public List<SelectListItem>? AvailableContractList { get; set; }
    public int? DiscountMaterials { get; set; }
    public List<SelectListItem>? AvailableDiscountMaterialsList { get; set; }
}