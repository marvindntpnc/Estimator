using Estimator.Domain;
using Estimator.Domain.Enums;
using Estimator.Inerfaces;
using Estimator.Models;
using Estimator.Models.EstimateForming;

namespace Estimator.Factories;

public class TarifficatorModelFactory:ITarifficatorModelFactory
{
    private readonly ITarifficatorService _tarifficatorService;

    public TarifficatorModelFactory(ITarifficatorService tarifficatorService)
    {
        _tarifficatorService = tarifficatorService;
    }

    public async Task<EstimateFormingModel> PrepareEstimateFormingModelAsync(EstimateFormingSearchModel searchModel)
    {
        var model=new EstimateFormingModel
        {
            FulTarifficator =  new List<TarrificatorItemModel>(),
            KtoTarifficator = new List<TarrificatorItemModel>()
        };
        var fulItems = await _tarifficatorService.GetTarifficatorItemsAsync(searchModel,TarifficatorType.FUL);
        var ktoItems = await _tarifficatorService.GetTarifficatorItemsAsync(searchModel,TarifficatorType.KTO);

        foreach (var fulItem in fulItems.Items)
        {
            model.FulTarifficator.Add(await PrepareTarifficatorItemModel(fulItem));
        }
        
        foreach (var ktoItem in ktoItems.Items)
        {
            model.KtoTarifficator.Add(await PrepareTarifficatorItemModel(ktoItem));
        }
        
        return model;
    }

    private async Task<TarrificatorItemModel> PrepareTarifficatorItemModel(TarifficatorItem item)
    {
        var model = new TarrificatorItemModel
        {
            CategoryId = item.CategoryId,
            SubcategoryId = item.SubcategoryId,
            Description = item.Description,
            CategoryName = item.CategoryId>0?await _tarifficatorService.GetCategoryNameByCategoryIdAsync(item.CategoryId):String.Empty,
            Discount = item.Discount,
            Id = item.Id,
            SubCategoryName = item.SubcategoryId>0?await _tarifficatorService.GetCategoryNameByCategoryIdAsync(item.SubcategoryId):String.Empty,
            Measure = item.Measure,
            Name = item.Name,
            Price = item.Price,
            Tarifficator = null,
            CurrencyType = item.CurrencyType,
            ItemCode = item.ItemCode,
            TarificatorId =item.TarificatorId,
            TarificatorItemType = item.TarificatorItemType
        };
        
        return model;
    }
}