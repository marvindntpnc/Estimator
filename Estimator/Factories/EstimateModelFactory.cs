using Estimator.Domain;
using Estimator.Domain.Enums;
using Estimator.Inerfaces;
using Estimator.Models.Estimate;
using Microsoft.AspNetCore.Mvc.Rendering;
using PagedList;

namespace Estimator.Factories;

public class EstimateModelFactory :IEstimateModelFactory
{
    private readonly IEstimateService _estimateService;
    private readonly IRepository<TarifficatorItem> _tarifficatorItemRepository;
    private readonly ITarifficatorModelFactory _tarifficatorModelFactory;
    private readonly ITarifficatorService _tarifficatorService;
    private readonly IFacilityService _facilityService;

    public EstimateModelFactory(
        IEstimateService estimateService, 
        IRepository<TarifficatorItem> tarifficatorItemRepository, 
        ITarifficatorModelFactory tarifficatorModelFactory, 
        ITarifficatorService tarifficatorService,
        IFacilityService facilityService)
    {
        _estimateService = estimateService;
        _tarifficatorItemRepository = tarifficatorItemRepository;
        _tarifficatorModelFactory = tarifficatorModelFactory;
        _tarifficatorService = tarifficatorService;
        _facilityService = facilityService;
    }
    
    /// <summary>
    /// Using for preparing full info for estimate item of selected Estimate.
    /// </summary>
    /// <returns>List of Estimate Items info</returns>
    public async Task<List<EstimateItemModel>> PrepareEstimateItemModelList(int estimateId)
    {
        var model = new List<EstimateItemModel>();
        var estimateItems =await _estimateService.GetEstimateItemsByEstimateIdAsync(estimateId);
        foreach (var estimateItem in estimateItems)
        {
            var tarifficatorItem=await _tarifficatorItemRepository.GetByIdAsync(estimateItem.TarifficatorItemId);
            if (tarifficatorItem != null)
            {
                model.Add(new EstimateItemModel
                {
                    Id = estimateItem.Id,
                    EstimateId = estimateItem.EstimateId,
                    Qty = estimateItem.Qty,
                    CustomRate = estimateItem.CustomRate,
                    Total = await _estimateService.CalculateEstimateItemRubleTotal(estimateItem),
                    TarifficatorItem = await _tarifficatorModelFactory.PrepareTarifficatorItemModel(tarifficatorItem)
                });
            }
        }
        return model;
    }

    /// <summary>
    /// Prepares search model for estimate list page including facility selector.
    /// </summary>
    /// <returns>Estimate search model with filled list of facilities.</returns>
    public async Task<EstimateSearchModel> PrepareEstimateSearchModel()
    {
        var facilityList = await _facilityService.GetFacilityListAsync();
        var model = new EstimateSearchModel
        {
            AvailableFacilityList = new List<SelectListItem>()
        };
        model.AvailableFacilityList.Add(new SelectListItem
        {
            Value = "0",
            Text = "Все объекты"
        });
        foreach (var facility in facilityList)
        {
            model.AvailableFacilityList.Add(new SelectListItem
            {
                Value = facility.Id.ToString(),
                Text = facility.Name
            });
        }
        
        return model;
    }
    
    /// <summary>
    /// Prepares model for estimate editing page by estimate identifier.
    /// Includes facility, contract and discount materials dropdown lists.
    /// </summary>
    /// <param name="estimateId">Estimate identifier.</param>
    /// <returns>Filled update estimate model.</returns>
    public async Task<UpdateEstimateModel> PrepareUpdateEstimateModelAsync(int estimateId)
    {
        var estimate=await _estimateService.GetEstimateByIdAsync(estimateId);
        var estimateCurrencyRates=await _estimateService.GetEstimateCurrencyRatesByEstimateIdAsync(estimate.Id);
        var facility = await _facilityService.GetFacilityByIdAsync(estimate.FacilityId);
        var contract=await _facilityService.GetContractByIdAsync(estimate.ContractId);
        
        if (estimate==null)
            return null;
        
        var model = new UpdateEstimateModel
        {
            EstimateId = estimate.Id,
            EstimateNumber = estimate.Number,
            EstimateName=estimate.EstimateName,
            EstimateDate=estimate.CreatedOn,
            EstimateClosedDate=estimate.ClosedOn,
            FacilityId=estimate.FacilityId,
            FacilityName=facility!=null?facility.Name:String.Empty,
            FacilityHourRate=facility?.HourRate ?? 0,
            ContractId=estimate.ContractId,
            ContractCompiledName=contract!=null?$"№{contract.Number} от {contract.StartDate:dd.MM.yyyy}":String.Empty,
            IsDiscounts=estimate.IsDiscounts,
            EurRate=estimateCurrencyRates.FirstOrDefault(cr=>cr.Key==CurrencyType.EUR).Value,
            UsdRate=estimateCurrencyRates.FirstOrDefault(cr=>cr.Key==CurrencyType.USD).Value,
            CnyRate=estimateCurrencyRates.FirstOrDefault(cr=>cr.Key==CurrencyType.CNY).Value,
            AvailableFacilityList = new List<SelectListItem>(),
            Items = new EstimateItemSearchModel
            {
                EstimateId = estimate.Id,
            },
            EstimateFormingSearchModel=await _tarifficatorModelFactory.PrepareEstimateFormingSearchModelAsync(),
            AvailableContractList = new List<SelectListItem>(),
            DiscountMaterials = (int?)estimate.DiscountMaterials,
            AvailableDiscountMaterialsList = new List<SelectListItem>()
        };

        var facilityList = await _facilityService.GetFacilityListAsync();
        
        model.AvailableFacilityList.Add(new SelectListItem
        {
            Value = "0",
            Text = "Все объекты"
        });
        foreach (var f in facilityList)
        {
            model.AvailableFacilityList.Add(new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Name
            });
        }

        var contractList = await _facilityService.GetFacilityContractsAsync(estimate.FacilityId);
        
        if (contractList.Count > 0)
        {
            foreach (var c in contractList)
            {
                model.AvailableContractList.Add(new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"№{c.Number} от {c.StartDate:dd.MM.yyyy}"
                });
            }
        }
        else
        {
            model.AvailableContractList.Add(new SelectListItem
            {
                Value = "0",
                Text = "Выберите объект"
            });
        }
        
        model.AvailableDiscountMaterialsList.Add(new SelectListItem
        {
            Value = "-1",
            Text = "Все материалы"
        });
        
        model.AvailableDiscountMaterialsList.Add(new SelectListItem
        {
            Value = ((int)TarifficatorType.FUL).ToString(),
            Text = "Материалы ФУЛ"
        });
        
        model.AvailableDiscountMaterialsList.Add(new SelectListItem
        {
            Value = ((int)TarifficatorType.KTO).ToString(),
            Text = "Материалы КТО"
        });
        
        return model;
    }
    
    /// <summary>
    /// Prepares paged list of estimate items for DataTables on estimate editing page.
    /// </summary>
    /// <param name="searchModel">Search and paging parameters, including estimate identifier.</param>
    /// <returns>Paged list of estimate item models.</returns>
    public async Task<IPagedList<EstimateItemModel>> PrepareEstimateItemModelPagedList(EstimateItemSearchModel searchModel)
    {
        var estimateItems = await _estimateService.GetEstimateItemsByEstimateIdAsync(searchModel.EstimateId);
        
        var list=new List<EstimateItemModel>();
        foreach (var item in estimateItems)
        {
            list.Add(new EstimateItemModel
            {
                Id = item.Id,
                EstimateId = item.EstimateId,
                Qty = item.Qty,
                CustomRate = item.CustomRate,
                TarifficatorItem = await _tarifficatorModelFactory.PrepareTarifficatorItemModelByTarifficatorItemId(item.TarifficatorItemId),
                RublePrice = await _estimateService.CalculateEstimateItemRublePrice(item),
                Total = await _estimateService.CalculateEstimateItemRubleTotal(item)
            });
        }
        
        
        return list.ToPagedList(searchModel.PageIndex+1,searchModel.PageSize);
    }
}