using Estimator.Domain;
using Estimator.Models;
using Estimator.Models.EstimateForming;

namespace Estimator.Inerfaces;

public interface ITarifficatorModelFactory
{
    Task<EstimateFormingModel> PrepareEstimateFormingModelAsync(EstimateFormingSearchModel searchModel);
    Task<TarrificatorItemModel> PrepareTarifficatorItemModel(TarifficatorItem item);
}