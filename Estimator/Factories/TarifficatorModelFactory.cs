using Estimator.Domain;
using Estimator.Domain.Enums;
using Estimator.Inerfaces;
using Estimator.Models;
using Estimator.Models.EstimateForming;
using Estimator.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Estimator.Factories;

public class TarifficatorModelFactory:ITarifficatorModelFactory
{
    private readonly ITarifficatorService _tarifficatorService;

    public TarifficatorModelFactory(ITarifficatorService tarifficatorService)
    {
        _tarifficatorService = tarifficatorService;
    }

    public async Task<EstimateFormingSearchModel> PrepareEstimateFormingSearchModelAsync()
    {
        var model = new EstimateFormingSearchModel();
        var categoryList = (await _tarifficatorService.GetCategoryListAsync()).OrderBy(x => x.Name).ToList();
        var subcategoryList= (await _tarifficatorService.GetSubcategoryListAsync()).OrderBy(x => x.Name).ToList();
        
        model.AvailableCategories.Add(new SelectListItem
        {
            Text = "Все категории",
            Value = "0",
        });
        foreach (var category in categoryList)
        {
            model.AvailableCategories.Add(new SelectListItem
            {
                Text = category.Name,
                Value = category.Id.ToString(),
            });
        }
        
        model.AvailableSubcategories.Add(new SelectListItem
        {
            Text = "Все подкатегории",
            Value = "0",
        });
        foreach (var subcategory in subcategoryList)
        {
            model.AvailableSubcategories.Add(new SelectListItem
            {
                Text = subcategory.Name,
                Value = subcategory.Id.ToString(),
            });
        }
        
        model.AvailableCurrencies.Add(new SelectListItem
        {
            Text = "Все валюты",
            Value = "0",
        });
        
        foreach (var currency in Enum.GetValues(typeof(CurrencyType)))
        {
            model.AvailableCurrencies.Add(new SelectListItem
            {
                Text = EnumHelper.ConvertCurrencyTypeToString((CurrencyType)currency),
                Value = ((int)currency).ToString(),
            });
        }
        
        model.AvailableMeasures.Add(new SelectListItem
        {
            Text = "Все единицы измерения",
            Value = "0",
        });
        
        foreach (var measure in Enum.GetValues(typeof(MeasureType)))
        {
            model.AvailableMeasures.Add(new SelectListItem
            {
                Text = EnumHelper.ConvertMeasureTypeToString((MeasureType)measure),
                Value = ((int)measure).ToString(),
            });
        }
        
        model.AvailableItemTypes.Add(new SelectListItem
        {
            Text = "Все типы",
            Value = "0",
        });
        
        foreach (var itemType in Enum.GetValues(typeof(TarificatorItemType)))
        {
            model.AvailableItemTypes.Add(new SelectListItem
            {
                Text = EnumHelper.ConvertTarifficatorItemTypeToString((TarificatorItemType)itemType),
                Value = ((int)itemType).ToString(),
            });
        }
        return model;
    }
    
    public async Task<TarrificatorItemModel> PrepareTarifficatorItemModel(TarifficatorItem item)
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
            MeasureString = EnumHelper.ConvertMeasureTypeToString(item.Measure),
            Name = item.Name,
            Price = item.Price,
            CurrencyType = item.CurrencyType,
            TarificatorItemTypeString = EnumHelper.ConvertTarifficatorItemTypeToString(item.TarificatorItemType),
            CurrencyString = EnumHelper.ConvertCurrencyTypeToString(item.CurrencyType),
            ItemCode = item.ItemCode,
            TarificatorItemType = item.TarificatorItemType,
            TarifficatorType = item.TarifficatorType,
            IsCustomAdding = item.IsCustomAdding
        };
        
        return model;
    }

    public async Task<TarrificatorItemModel?> PrepareTarifficatorItemModelByTarifficatorItemId(int id)
    {
        var item = await _tarifficatorService.GetTarifficatorItemByIdAsync(id);
        if (item == null)
            return null;
        return await PrepareTarifficatorItemModel(item);
    }
    
}