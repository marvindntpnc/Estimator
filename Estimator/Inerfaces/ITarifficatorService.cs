using Estimator.Domain;
using Estimator.Domain.Enums;
using Estimator.Models.EstimateForming;
using Estimator.Models.Shared;
using PagedList;

namespace Estimator.Inerfaces;

public interface ITarifficatorService
{
    /// <summary>
    /// Imports tarifficator data from Excel file, compares with existing data and applies changes.
    /// </summary>
    /// <param name="file">Uploaded Excel file with tarifficator data.</param>
    /// <param name="tarifficatorType">Type of tarifficator (FUL/KTO).</param>
    /// <returns>Import result with information about added/changed/removed items and duplicates.</returns>
    Task<UploadTarifficatorResultModel> CreateTarifficatorAsync(IFormFile file,TarifficatorType tarifficatorType);
    
    /// <summary>
    /// Returns paged list of tarifficator items according to search filters.
    /// </summary>
    Task<IPagedList<TarifficatorItem?>> GetTarifficatorItemsAsync(EstimateFormingSearchModel searchModel,
        TarifficatorType tarifficatorType);
    
    /// <summary>
    /// Gets list of top-level categories.
    /// </summary>
    Task<List<Category>> GetCategoryListAsync();
    
    /// <summary>
    /// Gets list of subcategories.
    /// </summary>
    Task<List<Category>> GetSubcategoryListAsync();
    
    /// <summary>
    /// Returns category name by its identifier.
    /// </summary>
    Task<string?> GetCategoryNameByCategoryIdAsync(int categoryId);
    
    /// <summary>
    /// Finds category by name and optional parent category identifier.
    /// </summary>
    Task<Category?> GetCategoryByNameAsync(string categoryName,int parentCategoryId=0);
    
    /// <summary>
    /// Clears application data: tarifficator items, categories and estimates.
    /// </summary>
    Task ResetAppAsync();
    
    /// <summary>
    /// Updates type and custom adding flag for tarifficator item.
    /// </summary>
    Task UpdateTarifficatorItemInfoAsync(int id, TarificatorItemType tarifficatorItemType, bool isCustomAdding);
    
    /// <summary>
    /// Gets tarifficator item by its identifier.
    /// </summary>
    Task<TarifficatorItem?> GetTarifficatorItemByIdAsync(int id);
}