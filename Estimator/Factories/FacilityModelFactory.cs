using Estimator.Inerfaces;
using Estimator.Models.Facility;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using PagedList;

namespace Estimator.Factories;

public class FacilityModelFactory:IFacilityModelFactory
{
    private readonly IFacilityService _facilityService;

    public FacilityModelFactory(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    public async Task<IPagedList<FacilityModel>> PrepareFacilityPagedListAsync(FacilitySearchModel searchModel)
    {
        var pagedItems = await _facilityService.GetFacilityPagedListAsync(searchModel);
        
        var list=new List<FacilityModel>();
        foreach (var item in pagedItems)
        {
            string area=!item.AreaName.IsNullOrEmpty()?$"{item.AreaName}, ":String.Empty;
            string enclosure=!item.EnclosureNumber.IsNullOrEmpty()?$"корп.{item.EnclosureNumber}, ":String.Empty;
            string building=!item.BuildingNumber.IsNullOrEmpty()?$"ст.{item.BuildingNumber}":String.Empty;
            list.Add(new FacilityModel
            {
                Id = item.Id,
               Name = item.Name,
               ContractList = new List<ContractModel>(),
               HourPrice = item.HourRate,
               AddressString = $"{item.StateName}, {area} г.{item.CityName}, {item.Address}, д.{item.HouseNumber}, {enclosure} {building}"
            });
        }
        
        
        return list.ToPagedList(searchModel.PageIndex+1,searchModel.PageSize);
    }

    public async Task<CreateOrUpdateFacilityModel> PrepareCreateOrEditFacilityModelAsync(int facilityId)
    {
        var facility=await _facilityService.GetFacilityByIdAsync(facilityId);
        var contractList=await _facilityService.GetFacilityContractsAsync(facilityId);
        var discountList=await _facilityService.GetFacilityDiscountsAsync(facilityId);
        
        var model = new CreateOrUpdateFacilityModel
        {
            ContractList = new List<SelectListItem>(),
            DiscountRequirements = new List<DiscountRequirementModel>(),
            Id = facility.Id,
            HourPrice = facility.HourRate,
            AreaName = facility.AreaName,
            EnclosureNumber = facility.EnclosureNumber,
            BuildingNumber = facility.BuildingNumber,
            StateName = facility.StateName,
            CityName = facility.CityName,
            HouseNumber = facility.HouseNumber,
            FacilityAddress = facility.Address,
            FacilityName = facility.Name,
            ActiveContractId = facility.ActiveContractId
        };

        foreach (var discount in discountList)
        {
            model.DiscountRequirements.Add(new DiscountRequirementModel
            {
                Id = discount.Id,
                InstallRate = discount.InstallRate,
                StartRange =discount.StartRange,
                EndRange = discount.EndRange,
                SuppliesRate = discount.SuppliesRate,
                UninstallRate = discount.UninstallRate,
                FacilityId = facility.Id,
                    
            });
        }
        if (!contractList.Any())
        {
            model.ContractList.Add(new SelectListItem
            {
                Value = "0",
                Text = "Нет добавленных договоров"
            });
        }
        else
        {
            foreach (var contract in contractList)
            {
                model.ContractList.Add(new SelectListItem
                {
                    Value = contract.Id.ToString(),
                    Text = $"№{contract.Number} от {contract.StartDate:dd.MM.yyyy}"
                });
            }
        }
        
        return model;
    }
}