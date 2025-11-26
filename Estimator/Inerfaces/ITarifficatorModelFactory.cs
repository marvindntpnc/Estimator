using Estimator.Domain;
using Estimator.Models;
using Estimator.Models.EstimateForming;

namespace Estimator.Inerfaces;

public interface ITarifficatorModelFactory
{
    /// <summary>
    /// Prepare Search model for Estimate forming page.
    /// Filling Lists for search selects on page by saved category names and Enums
    /// </summary>
    /// <returns>Search model for page rendering</returns>
    Task<EstimateFormingSearchModel> PrepareEstimateFormingSearchModelAsync();
    /// <summary>
    /// Convert integer Entity-values to strings for further work
    /// </summary>
    /// <param name="item">Tarifficator Item Entity</param>
    /// <returns>Tarifficator Item Model with names of Category, Subcategory and Enums</returns>
    Task<TarrificatorItemModel> PrepareTarifficatorItemModel(TarifficatorItem item);
    
    /// <summary>
    /// Builds tarifficator item model by tarifficator item identifier.
    /// </summary>
    /// <param name="id">Identifier of <see cref="TarifficatorItem"/>.</param>
    /// <returns>Tarifficator item model or null if item is not found.</returns>
    Task<TarrificatorItemModel?> PrepareTarifficatorItemModelByTarifficatorItemId(int id);
}