using Estimator.Domain;
using Estimator.Domain.Enums;
using Estimator.Models.Estimate;
using PagedList;

namespace Estimator.Inerfaces;

public interface IEstimateService
{
    /// <summary>
    /// Creates a new estimate together with its items and currency rates in one operation.
    /// </summary>
    /// <param name="items">List of items with quantities and custom rates.</param>
    /// <param name="title">Estimate name (object description).</param>
    /// <param name="number">Internal estimate number.</param>
    /// <param name="currencyRate">Dictionary with currency rates for non‑RUB items.</param>
    /// <param name="customerName">Customer full name.</param>
    /// <param name="isDiscounts">Flag indicating whether discounts should be applied.</param>
    /// <returns>Created estimate entity.</returns>
    public Task<Estimate> CreateEstimateAsync(List<EstimateItemMinimizedModel> items,string title,string number,Dictionary<CurrencyType,decimal> currencyRate,string customerName, bool isDiscounts=false);
    
    /// <summary>
    /// Returns list of all estimates with loaded currency rates.
    /// </summary>
    public Task<List<Estimate>> GetEstimateList();
    
    /// <summary>
    /// Gets estimate by identifier and loads its currency rates.
    /// </summary>
    public Task<Estimate?> GetEstimateByIdAsync(int id);
    
    /// <summary>
    /// Returns all estimate items for specified estimate.
    /// </summary>
    public Task<List<EstimateItem>> GetEstimateItemsByEstimateIdAsync(int estimateId);
    
    /// <summary>
    /// Returns paged list of estimates according to search criteria.
    /// </summary>
    Task<IPagedList<EstimateModel>> GetEstimateModelPagedListAsync(EstimateSearchModel searchModel);
    
    /// <summary>
    /// Calculates base price of estimate item in RUB according to its currency and estimate rates.
    /// </summary>
    /// <param name="estimateItem">Estimate item entity.</param>
    /// <returns>Price for single unit in RUB.</returns>
    public Task<decimal> CalculateEstimateItemRublePrice(EstimateItem estimateItem);//TODO rewright two methods with common BaseInterface.   
    
    /// <summary>
    /// Calculates base price of estimate item model in RUB according to its currency and estimate rates.
    /// </summary>
    /// <param name="estimateItem">Estimate item view model.</param>
    /// <returns>Price for single unit in RUB.</returns>
    public Task<decimal> CalculateEstimateItemRublePrice(EstimateItemModel estimateItem);
    
    /// <summary>
    /// Inserts new estimate entity without items.
    /// </summary>
    public Task CreateEstimateAsync(Estimate estimate);
    
    /// <summary>
    /// Returns dictionary of currency rates for given estimate.
    /// </summary>
    public Task<Dictionary<CurrencyType, decimal>> GetEstimateCurrencyRatesByEstimateIdAsync(int estimateId);
    
    /// <summary>
    /// Updates main estimate fields and its currency rates according to edit model.
    /// </summary>
    public Task UpdateEstimateAsync(UpdateEstimateModel model);
    
    /// <summary>
    /// Adds new estimate item to existing estimate.
    /// </summary>
    public Task AddEstimateItemAsync(EstimateItemMinimizedModel estimateItemModel);
    
    /// <summary>
    /// Deletes estimate item by identifier.
    /// </summary>
    public Task DeleteEstimateItemAsync(int estimateItemId);
    
    /// <summary>
    /// Updates quantity, custom rate and type of existing estimate item.
    /// </summary>
    public Task UpdateEstimateItemAsync(EstimateItemMinimizedModel estimateItemModel);
    
    /// <summary>
    /// Calculates total amount (qty * price, considering discounts and hour rate) for estimate item in RUB.
    /// </summary>
    public Task<decimal> CalculateEstimateItemRubleTotal(EstimateItem estimateItem);//TODO rewright two methods with common BaseInterface.   
    
    /// <summary>
    /// Calculates total amount for estimate item model in RUB.
    /// </summary>
    public Task<decimal> CalculateEstimateItemRubleTotal(EstimateItemModel estimateItem);
    
    /// <summary>
    /// Deletes estimate together with its related information.
    /// </summary>
    public Task DeleteEstimateAsync(int estimateId);
    
    /// <summary>
    /// Calculates grand total for the estimate including VAT and discounts.
    /// </summary>
    public Task<decimal> CalculateEstimateGrandTotal(int estimateId);
    
    /// <summary>
    /// Returns list of KTO material items (view models) built from estimate items.
    /// </summary>
    public Task<List<EstimateItemModel>> GetKtoMaterialsFromEstimateItemsAsync(List<EstimateItem> estimateItem);

    /// <summary>
    /// Returns discount rule for estimate based on its facility and grand total.
    /// </summary>
    /// <param name="estimate">Estimate entity.</param>
    /// <param name="grandTotal">Grand total calculated for estimate.</param>
    /// <returns>Matched discount requirement or null.</returns>
    public Task<DiscountRequirement?> GetEstimateDiscountListByEstimateIdAsync(Estimate estimate,
        decimal grandTotal);
}