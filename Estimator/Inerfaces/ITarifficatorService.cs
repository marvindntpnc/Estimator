using Estimator.Domain;
using Estimator.Domain.Enums;
using Estimator.Models.EstimateForming;
using Estimator.Models.Shared;

namespace Estimator.Inerfaces;

public interface ITarifficatorService
{
    Task CreateTarifficator(IFormFile file,TarifficatorType tarifficatorType);
    Task<PagedList<TarifficatorItem>> GetTarifficatorItemsAsync(EstimateFormingSearchModel searchModel,
        TarifficatorType tarifficatorType);
    Task<string?> GetCategoryNameByCategoryIdAsync(int categoryId);
    Category? GetCategoryByName(string categoryName,int parentCategoryId=0);
}