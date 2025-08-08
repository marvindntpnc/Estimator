using Estimator.Domain;
using Estimator.Models.Estimate;

namespace Estimator.Inerfaces;

public interface IEstimateService
{
    public Task<Estimate> CreateEstimateAsync(List<EstimateItemMinimizedModel> items,string title,string number,decimal currencyRate,string customerName, bool isDiscounts=false);
    public Task<List<Estimate>> GetEstimateList();
    public Task<Estimate?> GetEstimateByIdAsync(int id);
    public List<EstimateItem> GetEstimateItemsByEstimateIdAsync(int estimateId);
    public (List<EstimateItemModel>, List<EstimateItemModel>) SortKtoItems(List<EstimateItemModel> items);
}