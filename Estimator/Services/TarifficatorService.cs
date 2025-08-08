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
    private readonly IRepository<Tarifficator> _tarifficatorRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<TarifficatorItem> _tarifficatorItemRepository;

    public TarifficatorService(
        IRepository<Tarifficator> tarifficatorRepository,
        IRepository<Category> categoryRepository, IRepository<TarifficatorItem> tarifficatorItemRepository)
    {
        _tarifficatorRepository = tarifficatorRepository;
        _categoryRepository = categoryRepository;
        _tarifficatorItemRepository = tarifficatorItemRepository;
    }

    public async Task CreateTarifficator(IFormFile file,TarifficatorType tarifficatorType)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
    
        using var workbook = new XLWorkbook(stream);
        
        // Оптимизированный поиск активного тарификатора
        var activeTarifficators = _tarifficatorRepository.GetWhereAsync(t => t.IsActive && t.TarifficatorType == tarifficatorType);
        var activeTarifficator = activeTarifficators.FirstOrDefault();
        if (activeTarifficator != null)
            await _tarifficatorRepository.DeleteAsync(activeTarifficator);
            
        var tarificator=new Tarifficator
        {
            TarifficatorItems = new List<TarifficatorItem>(),
            TarifficatorType = tarifficatorType,
            CreatedOn = DateTime.Now,
            IsActive = true
        };
        await _tarifficatorRepository.AddAsync(tarificator);
        
        // Кэшируем категории для оптимизации производительности
        var allCategories = await _categoryRepository.GetAllAsync();
        var categoryCache = allCategories.ToDictionary(c => c.Name, c => c);
        var subcategoryCache = allCategories.Where(c => c.ParentCategoryId.HasValue)
                                          .ToDictionary(c => $"{c.Name}_{c.ParentCategoryId}", c => c);
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
                            TarificatorId = tarificator.Id,
                            Tarifficator = tarificator,
                            Discount = string.Empty,
                        };
                        item.Measure = EnumHelper.ConvertMeasureTypeString(row.Cell("F").Value.ToString().Trim());
                        item.CurrencyType = EnumHelper.ConvertCurrencyTypeString(row.Cell("G").Value.ToString().Trim());

                        if (!row.Cell("B").Value.ToString().Trim().IsNullOrEmpty())
                        {
                            var categoryName = row.Cell("B").Value.ToString().Trim();
                            var category = await GetOrCreateCategoryAsync(categoryName, categoryCache);
                            item.CategoryId = category.Id;
                            
                            if (!row.Cell("C").Value.ToString().Trim().IsNullOrEmpty())
                            {
                                var subcategoryName = row.Cell("C").Value.ToString().Trim();
                                var subCategory = await GetOrCreateSubcategoryAsync(subcategoryName, category.Id, subcategoryCache);
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
                            TarificatorId = tarificator.Id,
                            Tarifficator = tarificator,
                            Discount = string.Empty,
                        };
                        item.Measure = EnumHelper.ConvertMeasureTypeString(row.Cell("E").Value.ToString().Trim());
                        item.CurrencyType = EnumHelper.ConvertCurrencyTypeString(row.Cell("F").Value.ToString().Trim());

                        if (!row.Cell("B").Value.ToString().Trim().IsNullOrEmpty())
                        {
                            var categoryName = row.Cell("B").Value.ToString().Trim();
                            var category = await GetOrCreateCategoryAsync(categoryName, categoryCache);
                            item.CategoryId = category.Id;
                        }

                        await _tarifficatorItemRepository.AddAsync(item);
                    }
                }
                break;
                
            default:
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
                            TarificatorId = tarificator.Id,
                            Tarifficator = tarificator,
                            Discount = row.Cell("F").Value.ToString().Trim(),
                        };
                        item.Measure = EnumHelper.ConvertMeasureTypeString(row.Cell("G").Value.ToString().Trim());
                        item.CurrencyType = EnumHelper.ConvertCurrencyTypeString(row.Cell("H").Value.ToString().Trim());

                        if (!row.Cell("B").Value.ToString().Trim().IsNullOrEmpty())
                        {
                            var categoryName = row.Cell("B").Value.ToString().Trim();
                            var category = await GetOrCreateCategoryAsync(categoryName, categoryCache);
                            item.CategoryId = category.Id;
                            if (!row.Cell("C").Value.ToString().Trim().IsNullOrEmpty())
                            {
                                var subcategoryName = row.Cell("C").Value.ToString().Trim();
                                var subCategory = await GetOrCreateSubcategoryAsync(subcategoryName, category.Id, subcategoryCache);
                                item.SubcategoryId = subCategory.Id;
                            }
                        }

                        await _tarifficatorItemRepository.AddAsync(item);
                    }
                }
                break;
        }
    }

    public async Task<PagedList<TarifficatorItem>> GetTarifficatorItemsAsync(EstimateFormingSearchModel searchModel,
        TarifficatorType tarifficatorType)
    {
        var tarifficators = _tarifficatorRepository.GetWhereAsync(t => t.TarifficatorType == tarifficatorType);
        var tarifficator = tarifficators.FirstOrDefault(t => t.IsActive);
        
        if (tarifficator != null)
        {
            if (searchModel.ItemName.IsNullOrEmpty())
            {
                return await _tarifficatorItemRepository.GetPagedAsync(
                    ti => ti.TarificatorId == tarifficator.Id, 
                    searchModel.PageIndex, 
                    searchModel.PageSize);
            }
            return await _tarifficatorItemRepository.GetPagedAsync(
                ti => ti.TarificatorId == tarifficator.Id && ti.Name.ToLower().Contains(searchModel.ItemName.ToLower()), 
                searchModel.PageIndex, 
                searchModel.PageSize);
        }
        
        return new PagedList<TarifficatorItem>();
    }

    public async Task<string?> GetCategoryNameByCategoryIdAsync(int categoryId)
    {
        return (await _categoryRepository.GetByIdAsync(categoryId))?.Name;
    }
    
    private async Task<Category> GetOrCreateCategoryAsync(string categoryName, Dictionary<string, Category> categoryCache)
    {
        if (categoryCache.TryGetValue(categoryName, out var existingCategory))
        {
            return existingCategory;
        }
        
        var category = new Category { Name = categoryName };
        await _categoryRepository.AddAsync(category);
        categoryCache[categoryName] = category;
        return category;
    }
    
    private async Task<Category> GetOrCreateSubcategoryAsync(string subcategoryName, int parentCategoryId, Dictionary<string, Category> subcategoryCache)
    {
        var cacheKey = $"{subcategoryName}_{parentCategoryId}";
        if (subcategoryCache.TryGetValue(cacheKey, out var existingSubcategory))
        {
            return existingSubcategory;
        }
        
        var subcategory = new Category
        {
            Name = subcategoryName,
            ParentCategoryId = parentCategoryId,
        };
        await _categoryRepository.AddAsync(subcategory);
        subcategoryCache[cacheKey] = subcategory;
        return subcategory;
    }
}