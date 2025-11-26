using Microsoft.AspNetCore.Mvc.Rendering;

namespace Estimator.Models.Facility;

public class CreateOrUpdateFacilityModel
{
    public int Id { get; set; }
    public string FacilityName { get; set; }
    public string StateName { get; set; }
    public string? AreaName { get; set; }
    public string CityName { get; set; }
    public string FacilityAddress { get; set; }
    public string HouseNumber { get; set; }
    public string? EnclosureNumber { get; set; }
    public string? BuildingNumber { get; set; }
    public decimal HourPrice { get; set; }
    public int ActiveContractId { get; set; }
    public List<SelectListItem> ContractList { get; set; }
    public List<DiscountRequirementModel> DiscountRequirements { get; set; }
}