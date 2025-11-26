using Estimator.Data;
using Estimator.Domain;
using Estimator.Domain.Enums;
using Estimator.Inerfaces;
using Estimator.Models.Estimate;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PagedList;

namespace Estimator.Services;

public class EstimateService :IEstimateService
{
    private readonly IRepository<Estimate> _estimateRepository;
    private readonly IRepository<EstimateItem> _estimateItemRepository;
    private readonly ITarifficatorService _tarifficatorService;
    private readonly ITarifficatorModelFactory _tarifficatorModelFactory;
    private readonly IRepository<EstimateCurrencyRate> _currencyRateRepository;
    private readonly IFacilityService _facilityService;

    public EstimateService(
        IRepository<Estimate> estimateRepository,
        IRepository<EstimateItem> estimateItemRepository, 
        ITarifficatorService tarifficatorService, 
        ITarifficatorModelFactory tarifficatorModelFactory, 
        IRepository<EstimateCurrencyRate> currencyRateRepository,
        IFacilityService facilityService)
    {
        _estimateRepository = estimateRepository;
        _estimateItemRepository = estimateItemRepository;
        _tarifficatorService = tarifficatorService;
        _tarifficatorModelFactory = tarifficatorModelFactory;
        _currencyRateRepository = currencyRateRepository;
        _facilityService = facilityService;
    }

    public async Task<Estimate> CreateEstimateAsync(List<EstimateItemMinimizedModel> items,string title,string number,Dictionary<CurrencyType,decimal> currencyRates,string customerName, bool isDiscounts=false)
    {
        var estimate = new Estimate
        {
            CreatedOn = DateTime.Now,
            Number = number,
            EstimateName = title,
            CurrencyRate = currencyRates,
            IsDiscounts = isDiscounts,
            CustomerName = customerName,
            EstimateItems = new List<EstimateItem>()
        };
        await _estimateRepository.InsertAsync(estimate);
        
        // persist currency rates rows
        if (currencyRates != null && currencyRates.Count > 0)
        {
            // we need ApplicationContext, get it from the repository using pattern knowledge
            var repoImpl = _estimateRepository as RepositoryService<Estimate>;
            if (repoImpl != null)
            {
                var contextField = typeof(RepositoryService<Estimate>).GetField("_dbContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var ctx = (ApplicationContext?)contextField?.GetValue(repoImpl);
                if (ctx != null)
                {
                    foreach (var kv in currencyRates)
                    {
                        ctx.EstimateCurrencyRates.Add(new EstimateCurrencyRate
                        {
                            EstimateId = estimate.Id,
                            CurrencyType = kv.Key,
                            Rate = kv.Value
                        });
                    }
                    await ctx.SaveChangesAsync();
                }
            }
        }
        
        foreach (var item in items)
        {
            if (item.Qty>0)
            {
                await _estimateItemRepository.InsertAsync(new EstimateItem
                {
                    EstimateId = estimate.Id,
                    Qty = item.Qty,
                    TarifficatorItemId = item.ItemId,
                    CustomRate = item.CustomRate
                });
            }
        }
        return estimate;
    }
    public async Task<List<Estimate>> GetEstimateList(){
        var list = await _estimateRepository.GetAllAsync();
        var repoImpl = _estimateRepository as RepositoryService<Estimate>;
        if (repoImpl != null)
        {
            var contextField = typeof(RepositoryService<Estimate>).GetField("_dbContext", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var ctx = (ApplicationContext?)contextField?.GetValue(repoImpl);
            if (ctx != null)
            {
                var grouped = ctx.EstimateCurrencyRates.AsQueryable().ToList()
                    .GroupBy(r => r.EstimateId)
                    .ToDictionary(g => g.Key, g => g.ToDictionary(r => r.CurrencyType, r => r.Rate));
                foreach (var e in list)
                {
                    if (grouped.TryGetValue(e.Id, out var dict))
                        e.CurrencyRate = dict;
                }
            }
        }
        return list;
    }

    public async Task<Estimate?> GetEstimateByIdAsync(int id)
    {
        var estimate = await _estimateRepository.GetByIdAsync(id);
        if (estimate == null) return null;
        estimate.CurrencyRate = await GetEstimateCurrencyRatesByEstimateIdAsync(id);
        return estimate;
    }

    public async Task<List<EstimateItem>> GetEstimateItemsByEstimateIdAsync(int estimateId)
    {
        return await _estimateItemRepository.Table.Where(ei=>ei.EstimateId==estimateId).ToListAsync();
    }

    public async Task<IPagedList<EstimateModel>> GetEstimateModelPagedListAsync(EstimateSearchModel searchModel)
    {
        var query = _estimateRepository.Table;
        if (!searchModel.EstimateName.IsNullOrEmpty())
        {
            query=query.Where(e=>e.EstimateName.ToLower().Contains(searchModel.EstimateName.ToLower()));
        }

        if (searchModel.FacilityId>0)
        {
            query=query.Where(e=>e.FacilityId==searchModel.FacilityId);
        }

        if (!searchModel.EstimateNumber.IsNullOrEmpty())
        {
            query=query.Where(e=>e.Number.ToLower().Contains(searchModel.EstimateNumber.ToLower()));
        }

        if (searchModel.EstimateStartDate.HasValue)
        {
            query=query.Where(e=>e.CreatedOn>=searchModel.EstimateStartDate);
        }

        if (searchModel.EstimateEndDate.HasValue)
        {
            query=query.Where(e=>e.CreatedOn<=searchModel.EstimateEndDate);
        }

        // Если чекбокс отмечен (isClosed = true) - показываем только закрытые сметы
        // Если не отмечен (isClosed = false) - показываем только не закрытые сметы
        if (searchModel.isClosed)
        {
            query = query.Where(e => e.ClosedOn.HasValue);
        }
        else
        {
            query = query.Where(e => !e.ClosedOn.HasValue);
        }
        
        var list=await query.ToListAsync();

        var itemsList = new List<EstimateModel>();
        foreach (var item in list)
        {
            itemsList.Add(new EstimateModel
            {
                Id = item.Id,
                CreatedAt = item.CreatedOn.ToString("dd.MM.yyyy"),
                Name = item.EstimateName,
                ClosedAt = item.CreatedOn.AddDays(30).ToString("dd.MM.yyyy"),
                EstimateTotal = await CalculateEstimateGrandTotal(item.Id),
                FacilityName = await _facilityService.GetFacilityNameByIdAsync(item.FacilityId),
                EstimateNumber = item.Number,
            });
        }

        return itemsList.ToPagedList(searchModel.PageIndex+1,searchModel.PageSize);
    }

    public async Task<decimal> CalculateEstimateItemRublePrice(EstimateItem estimateItem)
    {
        var estimate= await GetEstimateByIdAsync(estimateItem.EstimateId);
        var item=await _tarifficatorModelFactory.PrepareTarifficatorItemModelByTarifficatorItemId(
            estimateItem.TarifficatorItemId);
        if (estimate == null || item == null)
            return 0;
        
        return item.CurrencyType != CurrencyType.RUB
            ? Math.Round(item.Price * estimate.CurrencyRate.FirstOrDefault(cr => cr.Key == item.CurrencyType).Value,2)
            : Math.Round(item.Price,2);
    }
    
    public async Task<decimal> CalculateEstimateItemRublePrice(EstimateItemModel estimateItem)
    {
        var estimate= await GetEstimateByIdAsync(estimateItem.EstimateId);
        if (estimate == null || estimateItem.TarifficatorItem == null)
            return 0;
        
        return estimateItem.TarifficatorItem.CurrencyType != CurrencyType.RUB
            ? estimateItem.TarifficatorItem.Price * estimate.CurrencyRate.FirstOrDefault(cr => cr.Key == estimateItem.TarifficatorItem.CurrencyType).Value
            : estimateItem.TarifficatorItem.Price;
    }

    public async Task<decimal> CalculateEstimateItemRubleTotal(EstimateItem estimateItem)
    {
        var estimate=await GetEstimateByIdAsync(estimateItem.EstimateId);
        var tarifficatorItem=await _tarifficatorService.GetTarifficatorItemByIdAsync(estimateItem.TarifficatorItemId);
        
        if (estimate == null || tarifficatorItem == null)
            throw new Exception("Estimate or Tarifficator Item not found");
        
        var facility = await _facilityService.GetFacilityByIdAsync(estimate.FacilityId);
        decimal tempHourRate = 0;
        
        if (facility == null)//TODO temporary decision. Remove for production.
            tempHourRate=1;
        else 
            tempHourRate= facility.HourRate;
        
        if (tarifficatorItem.TarificatorItemType==TarificatorItemType.Service)
        {
            if (estimateItem.CustomRate>0 && tarifficatorItem.Measure==MeasureType.Hours)
                return Math.Round(estimateItem.Qty*tempHourRate*estimateItem.CustomRate*await CalculateEstimateItemRublePrice(estimateItem),2);

            if (tarifficatorItem.Measure == MeasureType.Hours)
                return Math.Round(estimateItem.Qty * tempHourRate * await CalculateEstimateItemRublePrice(estimateItem),2);
            
            return Math.Round(estimateItem.Qty*await CalculateEstimateItemRublePrice(estimateItem),2);
        }

        return Math.Round(estimateItem.Qty * await CalculateEstimateItemRublePrice(estimateItem),2);
    }
    
    public async Task<decimal> CalculateEstimateItemRubleTotal(EstimateItemModel estimateItem)
    {
        var estimate=await GetEstimateByIdAsync(estimateItem.EstimateId);
        
        if (estimate == null || estimateItem.TarifficatorItem == null)
            throw new Exception("Estimate or Tarifficator Item not found");
        
        var facility = await _facilityService.GetFacilityByIdAsync(estimate.FacilityId);
        decimal tempHourRate = 0;
        
        if (facility == null)
            tempHourRate=1;
        else 
            tempHourRate= facility.HourRate;
        
        if (estimateItem.TarifficatorItem.TarificatorItemType==TarificatorItemType.Service)
        {
            if (estimateItem.CustomRate > 0 && estimateItem.TarifficatorItem.Measure == MeasureType.Hours)
                return Math.Round(estimateItem.Qty * tempHourRate * estimateItem.CustomRate *
                                  await CalculateEstimateItemRublePrice(estimateItem),2);

            if (estimateItem.TarifficatorItem.Measure == MeasureType.Hours)
                return Math.Round(estimateItem.Qty * tempHourRate * await CalculateEstimateItemRublePrice(estimateItem),2);

            return Math.Round(estimateItem.Qty*await CalculateEstimateItemRublePrice(estimateItem),2);
        }

        return Math.Round(estimateItem.Qty * await CalculateEstimateItemRublePrice(estimateItem),2);
    }

    public async Task CreateEstimateAsync(Estimate estimate)
    {
        await _estimateRepository.InsertAsync(estimate);
    }

    public async Task<Dictionary<CurrencyType, decimal>> GetEstimateCurrencyRatesByEstimateIdAsync(int estimateId)
    {
        var estimateCurrencyRates =await _currencyRateRepository.Table.Where(ecr=>ecr.EstimateId==estimateId).ToListAsync();
        return estimateCurrencyRates.ToDictionary(ecr=>ecr.CurrencyType,ecr=>ecr.Rate);
    }

    public async Task UpdateEstimateAsync(UpdateEstimateModel model)
    {
        var estimate = await GetEstimateByIdAsync(model.EstimateId);
        if (estimate==null)
            throw new Exception("Estimate not found");
        
        estimate.Number=model.EstimateNumber;
        estimate.CreatedOn=model.EstimateDate;
        estimate.EstimateName=model.EstimateName;
        estimate.ClosedOn=model.EstimateClosedDate;
        estimate.FacilityId=model.FacilityId;
        estimate.ContractId=model.ContractId;
        estimate.IsDiscounts=model.IsDiscounts;

        switch (model.DiscountMaterials)
        {
            case -1:
                estimate.DiscountMaterials=null;
                break;
            case (int)TarifficatorType.FUL:
                estimate.DiscountMaterials=TarifficatorType.FUL;
                break;
            case (int)TarifficatorType.KTO:
                estimate.DiscountMaterials=TarifficatorType.KTO;
                break;
        }
        
        await _estimateRepository.UpdateAsync(estimate);
        
        var estimateCurrencyRates=new Dictionary<CurrencyType, decimal>();
        if (model.EurRate>0)
            estimateCurrencyRates.Add(CurrencyType.EUR,model.EurRate.Value);
        
        if (model.UsdRate>0)
            estimateCurrencyRates.Add(CurrencyType.USD,model.UsdRate.Value);
        
        if (model.CnyRate>0)
            estimateCurrencyRates.Add(CurrencyType.CNY,model.CnyRate.Value);

        if (estimateCurrencyRates.Count > 0)
            await UpdateEstimateCurrencyRates(model.EstimateId,estimateCurrencyRates);
    }

    private async Task UpdateEstimateCurrencyRates(int estimateId,
        Dictionary<CurrencyType, decimal> estimateCurrencyRates)
    {
        var currencyRates=await _currencyRateRepository.Table.Where(ecr=>ecr.EstimateId==estimateId).ToListAsync();
        foreach (var estimateCurrencyRate in estimateCurrencyRates)
        {
            var rate = new EstimateCurrencyRate
            {
                EstimateId = estimateId,
                CurrencyType = estimateCurrencyRate.Key,
            };
            if (currencyRates.Any(cr=>cr.CurrencyType==estimateCurrencyRate.Key))
                rate=currencyRates.FirstOrDefault(cr=>cr.CurrencyType==estimateCurrencyRate.Key);
            
            rate.Rate=estimateCurrencyRate.Value;
            if (currencyRates.Any(cr => cr.CurrencyType == estimateCurrencyRate.Key))
            {
                await _currencyRateRepository.UpdateAsync(rate);
            }
            else
            {
                await _currencyRateRepository.InsertAsync(rate);
            }
            
        }
    }

    public async Task AddEstimateItemAsync(EstimateItemMinimizedModel estimateItemModel)
    {
        if (estimateItemModel.EstimateId>0 && estimateItemModel.Qty>0)
        {
            await _estimateItemRepository.InsertAsync(new EstimateItem
            {
                EstimateId = estimateItemModel.EstimateId,
                CustomRate =estimateItemModel.CustomRate,
                Qty = estimateItemModel.Qty,
                TarifficatorItemId = estimateItemModel.ItemId
            });
        }
    }

    public async Task DeleteEstimateItemAsync(int estimateItemId)
    {
        var estimateItem=await _estimateItemRepository.GetByIdAsync(estimateItemId);
        if (estimateItem!=null) 
            await _estimateItemRepository.DeleteAsync(estimateItem);
    }

    public async Task UpdateEstimateItemAsync(EstimateItemMinimizedModel estimateItemModel)
    {
        var estimateItem = await _estimateItemRepository.GetByIdAsync(estimateItemModel.EstimateItemId);
        if (estimateItem!=null)
        {
            estimateItem.Qty = estimateItemModel.Qty;
            estimateItem.CustomRate = estimateItemModel.CustomRate;
            await _estimateItemRepository.UpdateAsync(estimateItem);
            
            var tarifficatorItem=await _tarifficatorService.GetTarifficatorItemByIdAsync(estimateItem.TarifficatorItemId);
            if (tarifficatorItem != null) 
                await _tarifficatorService.UpdateTarifficatorItemInfoAsync(tarifficatorItem.Id,estimateItemModel.TarifficatorItemType,tarifficatorItem.IsCustomAdding);
        }
    }

    public async Task<decimal> CalculateEstimateGrandTotal(int estimateId)
    {
        var estimate=await GetEstimateByIdAsync(estimateId);
        var estimateItems = await GetEstimateItemsByEstimateIdAsync(estimateId);
        decimal grandTotal=0;
        foreach (var estimateItem in estimateItems)
        {
            grandTotal+=await CalculateEstimateItemRubleTotal(estimateItem);
        }

        if (estimate.IsDiscounts)
        {
            var ktoMaterials = await GetKtoMaterialsFromEstimateItemsAsync(estimateItems);
            
            decimal ktoMaterialsTotal=Math.Round(ktoMaterials.Sum(km=>km.RublePrice*km.Qty),2);
            
            var estimateDiscount=await GetEstimateDiscountListByEstimateIdAsync(estimate,grandTotal);
            if (estimateDiscount != null)
            {
                decimal i = Math.Round(ktoMaterialsTotal * (estimateDiscount.InstallRate / 100),2);
                decimal u=Math.Round(ktoMaterialsTotal * (estimateDiscount.UninstallRate/100),2);
                decimal s=Math.Round(ktoMaterialsTotal * (estimateDiscount.SuppliesRate/100),2);
                decimal discountSum = i + u + s;
                
                grandTotal += discountSum;
            }
            
        }

        grandTotal += grandTotal * 0.2m;
        return Math.Round(grandTotal,2);
    }

    public async Task<List<EstimateItemModel>> GetKtoMaterialsFromEstimateItemsAsync(List<EstimateItem> estimateItem)
    {
        var ktoMaterials = new List<EstimateItemModel>();
        foreach (var item in estimateItem)
        {
            var tarifficatorItemModel=await _tarifficatorModelFactory.PrepareTarifficatorItemModelByTarifficatorItemId(item.TarifficatorItemId);
            if (tarifficatorItemModel!=null &&
                tarifficatorItemModel.TarifficatorType == TarifficatorType.KTO && 
                tarifficatorItemModel.TarificatorItemType==TarificatorItemType.Material)
            {
                ktoMaterials.Add(new EstimateItemModel
                {
                    CustomRate = item.CustomRate,
                    TarifficatorItem = tarifficatorItemModel,
                    EstimateId = item.EstimateId,
                    Qty = item.Qty,
                    Id = item.Id,
                    RublePrice = await CalculateEstimateItemRublePrice(item)
                });
            }
        }

        return ktoMaterials;
    }

    public async Task<DiscountRequirement?> GetEstimateDiscountListByEstimateIdAsync(Estimate estimate,decimal grandTotal)
    {
        var facilityDiscounts = await _facilityService.GetFacilityDiscountsAsync(estimate.FacilityId);
        if (facilityDiscounts.Any()) 
            return facilityDiscounts.FirstOrDefault(dr=>grandTotal>=dr.StartRange && grandTotal<=dr.EndRange);
        
        return null;
    }

    public async Task DeleteEstimateAsync(int estimateId)
    {
        var estimate=await GetEstimateByIdAsync(estimateId);
        if (estimate != null)
            await _estimateRepository.DeleteAsync(estimate);
    }
}