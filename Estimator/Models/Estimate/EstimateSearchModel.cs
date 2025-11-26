using Estimator.Models.Shared;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Estimator.Models.Estimate;

public class EstimateSearchModel:BaseSearchModel
{
    public string EstimateName { get; set; }
    public int FacilityId { get; set; }
    public string EstimateNumber { get; set; }
    public DateTime? EstimateStartDate { get; set; }
    public DateTime? EstimateEndDate { get; set; }
    public bool isClosed { get; set; }
    public List<SelectListItem> AvailableFacilityList { get; set; } 
}