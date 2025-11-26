using Estimator.Models.Estimate;
using PagedList;

namespace Estimator.Inerfaces;

public interface IEstimateModelFactory
{
    /// <summary>
    /// Prepares full info for estimate items of selected estimate.
    /// </summary>
    /// <returns>List of estimate item view models with calculated totals and tarifficator data.</returns>
    public Task<List<EstimateItemModel>> PrepareEstimateItemModelList(int estimateId);
    
    /// <summary>
    /// Prepares paged list of estimate item view models for DataTables.
    /// </summary>
    /// <param name="searchModel">Search and paging parameters.</param>
    /// <returns>Paged list of estimate item view models.</returns>
    public Task<IPagedList<EstimateItemModel>> PrepareEstimateItemModelPagedList(EstimateItemSearchModel searchModel);
    
    /// <summary>
    /// Prepares search model for estimate list page including available facilities.
    /// </summary>
    /// <returns>Estimate search model used to render filter form.</returns>
    public Task<EstimateSearchModel> PrepareEstimateSearchModel();
    
    /// <summary>
    /// Prepares model for estimate editing page by estimate identifier.
    /// Includes facility, contract and discount materials selectors.
    /// </summary>
    /// <param name="id">Estimate identifier.</param>
    /// <returns>Edit model for estimate.</returns>
    public Task<UpdateEstimateModel> PrepareUpdateEstimateModelAsync(int id);
}