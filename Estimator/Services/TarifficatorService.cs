using System.Globalization;
using ClosedXML.Excel;
using Estimator.Domain;
using Estimator.Domain.Enums;
using Estimator.Inerfaces;
using Estimator.Models.EstimateForming;
using Estimator.Models.Shared;
using Microsoft.IdentityModel.Tokens;

namespace Estimator.Services;

public class TarifficatorService: ITarifficatorService
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<TarifficatorItem> _tarifficatorItemRepository;

    public TarifficatorService(
        IRepository<Category> categoryRepository,
        IRepository<TarifficatorItem> tarifficatorItemRepository)
    {
        _categoryRepository = categoryRepository;
        _tarifficatorItemRepository = tarifficatorItemRepository;
    }

    public async Task CreateTarifficator(IFormFile file,TarifficatorType tarifficatorType)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
    
        using var workbook = new XLWorkbook(stream);
        
        var tarifficatorItems = _tarifficatorItemRepository.GetWhereAsync(t => t.TarifficatorType == tarifficatorType);
        if (tarifficatorItems.Count>0)
        {
            foreach (var item in tarifficatorItems)
            {
                await _tarifficatorItemRepository.DeleteAsync(item);
            }
        }
        switch (tarifficatorType)
        {
            case TarifficatorType.FUL:
                var materialsWorksheet = workbook.Worksheet(1);
                foreach (var row in materialsWorksheet.RowsUsed())
                {
                    var cellValue = row.Cell("A").Value.ToString();
                    if (!cellValue.IsNullOrEmpty() && !cellValue.Contains("поз"))
                    {
                        var priceString = row.Cell("I").Value.ToString().Replace(",", ".").Trim();
                        var item = new TarifficatorItem
                        {
                            ItemCode = row.Cell("A").Value.ToString().Trim(),
                            Name = row.Cell("D").Value.ToString().Trim(),
                            Description = row.Cell("E").Value.ToString().Trim(),
                            TarificatorItemType = TarificatorItemType.Material,
                            Price = decimal.Parse(priceString,  NumberStyles.Any, CultureInfo.InvariantCulture),
                            TarifficatorType = tarifficatorType,
                            Discount = string.Empty,
                        };
                        item.Measure = EnumHelper.ConvertMeasureTypeString(row.Cell("F").Value.ToString().Trim());
                        item.CurrencyType = EnumHelper.ConvertCurrencyTypeString(row.Cell("G").Value.ToString().Trim());

                        if (!row.Cell("B").Value.ToString().Trim().IsNullOrEmpty())
                        {
                            var categoryName = row.Cell("B").Value.ToString().Trim();
                            var category = await GetOrCreateCategoryAsync(categoryName);
                            item.CategoryId = category.Id;
                            
                            if (!row.Cell("C").Value.ToString().Trim().IsNullOrEmpty())
                            {
                                var subcategoryName = row.Cell("C").Value.ToString().Trim();
                                var subCategory = await GetOrCreateSubcategoryAsync(subcategoryName, category.Id);
                                item.SubcategoryId = subCategory.Id;
                            }
                        }

                        await _tarifficatorItemRepository.AddAsync(item);
                    }
                }
                
                var servicesWorksheet = workbook.Worksheet(2);
                foreach (var row in servicesWorksheet.RowsUsed())
                {
                    var cellValue = row.Cell("A").Value.ToString();
                    if (!cellValue.IsNullOrEmpty() && !cellValue.Contains("поз"))
                    {
                        var priceString = row.Cell("H").Value.ToString().Replace(",", ".").Trim();
                        var item = new TarifficatorItem
                        {
                            ItemCode = row.Cell("A").Value.ToString().Trim(),
                            Name = row.Cell("C").Value.ToString().Trim(),
                            Description = row.Cell("D").Value.ToString().Trim(),
                            TarificatorItemType = TarificatorItemType.Service,
                            Price = decimal.Parse(priceString,  NumberStyles.Any, CultureInfo.InvariantCulture),
                            TarifficatorType = tarifficatorType,
                            Discount = string.Empty,
                        };
                        item.Measure = EnumHelper.ConvertMeasureTypeString(row.Cell("E").Value.ToString().Trim());
                        item.CurrencyType = EnumHelper.ConvertCurrencyTypeString(row.Cell("F").Value.ToString().Trim());

                        if (!row.Cell("B").Value.ToString().Trim().IsNullOrEmpty())
                        {
                            var categoryName = row.Cell("B").Value.ToString().Trim();
                            var category = await GetOrCreateCategoryAsync(categoryName);
                            item.CategoryId = category.Id;
                        }

                        await _tarifficatorItemRepository.AddAsync(item);
                    }
                }
                break;
                
            case TarifficatorType.KTO:
                var worksheet = workbook.Worksheet(1);
                foreach (var row in worksheet.RowsUsed())
                {
                    var cellValue = row.Cell("A").Value.ToString();
                    if (!cellValue.IsNullOrEmpty() && !cellValue.Contains("поз"))
                    {
                        var priceString = row.Cell("I").Value.ToString().Replace(",", ".").Trim();
                        var item = new TarifficatorItem
                        {
                            ItemCode = row.Cell("A").Value.ToString().Trim(),
                            Name = row.Cell("D").Value.ToString().Trim(),
                            Description = row.Cell("E").Value.ToString().Trim(),
                            TarificatorItemType = TarificatorItemType.KtoItem,
                            Price = decimal.Parse(priceString,  NumberStyles.Any, CultureInfo.InvariantCulture),
                            TarifficatorType = tarifficatorType,
                            Discount = row.Cell("F").Value.ToString().Trim(),
                        };
                        item.Measure = EnumHelper.ConvertMeasureTypeString(row.Cell("G").Value.ToString().Trim());
                        item.CurrencyType = EnumHelper.ConvertCurrencyTypeString(row.Cell("H").Value.ToString().Trim());

                        if (!row.Cell("B").Value.ToString().Trim().IsNullOrEmpty())
                        {
                            var categoryName = row.Cell("B").Value.ToString().Trim();
                            var category = await GetOrCreateCategoryAsync(categoryName);
                            item.CategoryId = category.Id;
                            if (!row.Cell("C").Value.ToString().Trim().IsNullOrEmpty())
                            {
                                var subcategoryName = row.Cell("C").Value.ToString().Trim();
                                var subCategory = await GetOrCreateSubcategoryAsync(subcategoryName, category.Id);
                                item.SubcategoryId = subCategory.Id;
                            }
                        }

                        await _tarifficatorItemRepository.AddAsync(item);
                    }
                }
                break;
            default:
                Console.WriteLine("Tarifficator Type is not supported");
                break;
        }
    }

    public async Task<PagedList<TarifficatorItem>> GetTarifficatorItemsAsync(EstimateFormingSearchModel searchModel,
        TarifficatorType tarifficatorType)
    {
        
            if (searchModel.ItemName.IsNullOrEmpty())
            {
                return await _tarifficatorItemRepository.GetPagedAsync(
                    ti => ti.TarifficatorType == tarifficatorType, 
                    searchModel.PageIndex, 
                    searchModel.PageSize);
            }
            return await _tarifficatorItemRepository.GetPagedAsync(
                ti => ti.TarifficatorType == tarifficatorType && ti.Name.ToLower().Contains(searchModel.ItemName.ToLower()), 
                searchModel.PageIndex, 
                searchModel.PageSize);
        
        return new PagedList<TarifficatorItem>();
    }

    public async Task<string?> GetCategoryNameByCategoryIdAsync(int categoryId)
    {
        return (await _categoryRepository.GetByIdAsync(categoryId))?.Name;
    }
    
    public Category? GetCategoryByName(string categoryName, int parentCategoryId = 0)
    {
        if (parentCategoryId>0)
            return _categoryRepository.GetWhereAsync(c=>c.Name.ToLower().Contains(categoryName.ToLower()) &&
                                                        c.ParentCategoryId==parentCategoryId).FirstOrDefault();
        
        return _categoryRepository.GetWhereAsync(c=>c.Name.ToLower().Contains(categoryName.ToLower())).FirstOrDefault();
    }
    
    private async Task<Category> GetOrCreateCategoryAsync(string categoryName)
    {
        var existingCategory = GetCategoryByName(categoryName);
        if (existingCategory!=null)
        {
            return existingCategory;
        }
        
        var category = new Category { Name = categoryName };
        await _categoryRepository.AddAsync(category);
        return category;
    }
    
    private async Task<Category> GetOrCreateSubcategoryAsync(string subcategoryName, int parentCategoryId)
    {
        var existingSubcategory = GetCategoryByName(subcategoryName,parentCategoryId);
        if (existingSubcategory!=null)
        {
            return existingSubcategory;
        }
        
        var subcategory = new Category
        {
            Name = subcategoryName,
            ParentCategoryId = parentCategoryId,
        };
        await _categoryRepository.AddAsync(subcategory);
        return subcategory;
    }
}