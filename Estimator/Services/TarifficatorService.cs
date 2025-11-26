using System.Globalization;
using ClosedXML.Excel;
using Estimator.Domain;
using Estimator.Domain.Enums;
using Estimator.Inerfaces;
using Estimator.Models.EstimateForming;
using Estimator.Models.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PagedList;

namespace Estimator.Services;

public class TarifficatorService: ITarifficatorService
{
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<TarifficatorItem> _tarifficatorItemRepository;
    private readonly IRepository<Estimate> _estimateRepository;

    public TarifficatorService(
        IRepository<Category> categoryRepository,
        IRepository<TarifficatorItem> tarifficatorItemRepository,
        IRepository<Estimate> estimateRepository)
    {
        _categoryRepository = categoryRepository;
        _tarifficatorItemRepository = tarifficatorItemRepository;
        _estimateRepository = estimateRepository;
    }

    public async Task ResetAppAsync()
    {
        var tarifficatorItems = await _tarifficatorItemRepository.GetAllAsync();
        foreach (var tarifficatorItem in tarifficatorItems)
        {
            await _tarifficatorItemRepository.DeleteAsync(tarifficatorItem);
        }
        
        var categoryItems=await _categoryRepository.GetAllAsync();
        foreach (var categoryItem in categoryItems)
        {
            await _categoryRepository.DeleteAsync(categoryItem);
        }
        
        var estimates = await _estimateRepository.GetAllAsync();
        foreach (var estimate in estimates)
        {
            await _estimateRepository.DeleteAsync(estimate);
        }
    }

    public async Task<UploadTarifficatorResultModel> CreateTarifficatorAsync(IFormFile file,TarifficatorType tarifficatorType)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
    
        using var workbook = new XLWorkbook(stream);
        
        var oldItems =await _tarifficatorItemRepository.Table.Where(t => t.TarifficatorType == tarifficatorType).ToListAsync();
        var newItems = new List<TarifficatorItem>();
        // if (tarifficatorItems.Count>0)
        // {
        //     foreach (var item in tarifficatorItems)
        //     {
        //         await _tarifficatorItemRepository.DeleteAsync(item);
        //     }
        // }
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
                        
                        newItems.Add(item);
                        //await _tarifficatorItemRepository.AddAsync(item);
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

                        newItems.Add(item);
                        //await _tarifficatorItemRepository.AddAsync(item);
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
                            TarificatorItemType = TarificatorItemType.Material,
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

                        newItems.Add(item);
                        //await _tarifficatorItemRepository.AddAsync(item);
                    }
                }
                break;
            default:
                Console.WriteLine("Tarifficator Type is not supported");
                break;
        }
        
        var (result,duplicates)=ListCompareHelper.Compare(oldItems,newItems);
        foreach (var addedItem in result.Added)
        {
            await _tarifficatorItemRepository.InsertAsync(addedItem);
        }

        foreach (var changedItem in result.Changed)
        {
            var existing = changedItem.OldValue;
            var updated = changedItem.NewValue;

            foreach (var propName in changedItem.ChangedProperties)
            {
                var prop = typeof(TarifficatorItem).GetProperty(propName);
                if (prop == null || !prop.CanWrite) continue;
                var newVal = prop.GetValue(updated);
                prop.SetValue(existing, newVal);
            }

            await _tarifficatorItemRepository.UpdateAsync(existing);
        }

        foreach (var removedItem in result.Removed)    
        {
            removedItem.IsDeleted=true;
            await _tarifficatorItemRepository.UpdateAsync(removedItem);
        }

        return new UploadTarifficatorResultModel
        {
            ActionsInfo = result,
            DuplicatesInfo = duplicates
        };
    }

    public async Task<IPagedList<TarifficatorItem?>> GetTarifficatorItemsAsync(EstimateFormingSearchModel searchModel,
        TarifficatorType tarifficatorType)
    {
        bool hasName = !searchModel.ItemName.IsNullOrEmpty();
        bool hasCode = !searchModel.ItemCode.IsNullOrEmpty();
        bool hasType=searchModel.ItemTypeId > 0;
        bool hasCategory = searchModel.CategoryId > 0;
        bool hasSubcategory = searchModel.SubCategoryId > 0;
        bool hasCurrency = searchModel.CurrencyId > 0;
        bool hasMeasure = searchModel.MeasureTypeId > 0;
        
        

        // Map DataTables order to entity field names (data corresponds to property names in Domain/Model)
        string? orderBy = null;
        bool descending = false;
        if (!string.IsNullOrWhiteSpace(searchModel.OrderColumnData))
        {
            orderBy = searchModel.OrderColumnData;
            descending = string.Equals(searchModel.OrderDirection, "desc", StringComparison.OrdinalIgnoreCase);
        }

        var query =await _tarifficatorItemRepository.Table.Where(ti => ti.TarifficatorType == tarifficatorType
                                                                        && (!hasName || ti.Name.ToLower().Contains(searchModel.ItemName.ToLower()))
                                                                        && (!hasCode || ti.ItemCode.ToLower().Contains(searchModel.ItemCode.ToLower()))
                                                                        && (!hasType || ti.TarificatorItemType == (TarificatorItemType)searchModel.ItemTypeId)
                                                                        && (!hasCategory || ti.CategoryId == searchModel.CategoryId)
                                                                        && (!hasSubcategory || ti.SubcategoryId == searchModel.SubCategoryId)
                                                                        && (!hasCurrency || ti.CurrencyType == (CurrencyType)searchModel.CurrencyId)
                                                                        && (!hasMeasure || ti.Measure == (MeasureType)searchModel.MeasureTypeId) 
                                                                        && !ti.IsDeleted).ToListAsync();

        return query.ToPagedList(searchModel.PageIndex+1, searchModel.PageSize);
    }

    public async Task<string?> GetCategoryNameByCategoryIdAsync(int categoryId)
    {
        return (await _categoryRepository.GetByIdAsync(categoryId))?.Name;
    }

    public async Task<Category?> GetCategoryByNameAsync(string categoryName, int parentCategoryId = 0)
    {
        if (parentCategoryId>0)
            return await _categoryRepository.Table.Where(c=>c.Name.ToLower().Contains(categoryName.ToLower()) &&
                                                        c.ParentCategoryId==parentCategoryId).FirstOrDefaultAsync();
        
        return await _categoryRepository.Table.Where(c=>c.Name.ToLower().Contains(categoryName.ToLower())).FirstOrDefaultAsync();
    }

    public async Task<List<Category>> GetCategoryListAsync()
    {
        return await _categoryRepository.Table.Where(c=>c.ParentCategoryId==null).ToListAsync();
    }
    
    public async Task<List<Category>> GetSubcategoryListAsync()
    {
        return await _categoryRepository.Table.Where(c=>c.ParentCategoryId!=null).ToListAsync();
    }

    public async Task UpdateTarifficatorItemInfoAsync(int id,TarificatorItemType tarifficatorItemType, bool isCustomAdding)
    {
        var item = await _tarifficatorItemRepository.GetByIdAsync(id);
        
        item.TarificatorItemType = tarifficatorItemType;
        item.IsCustomAdding = isCustomAdding;

        await _tarifficatorItemRepository.UpdateAsync(item);
    }

    public async Task<TarifficatorItem?> GetTarifficatorItemByIdAsync(int id)
    {
        return await _tarifficatorItemRepository.GetByIdAsync(id);
    }

    #region Utilities

    private async Task<Category> GetOrCreateCategoryAsync(string categoryName)
    {
        var existingCategory = await GetCategoryByNameAsync(categoryName);
        if (existingCategory!=null)
        {
            return existingCategory;
        }
        
        var category = new Category { Name = categoryName };
        await _categoryRepository.InsertAsync(category);
        return category;
    }
    
    private async Task<Category> GetOrCreateSubcategoryAsync(string subcategoryName, int parentCategoryId)
    {
        var existingSubcategory = await GetCategoryByNameAsync(subcategoryName,parentCategoryId);
        if (existingSubcategory!=null)
        {
            return existingSubcategory;
        }
        
        var subcategory = new Category
        {
            Name = subcategoryName,
            ParentCategoryId = parentCategoryId,
        };
        await _categoryRepository.InsertAsync(subcategory);
        return subcategory;
    }

    #endregion
}