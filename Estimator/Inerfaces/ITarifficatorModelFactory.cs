using Estimator.Models.EstimateForming;

namespace Estimator.Inerfaces;

public interface ITarifficatorModelFactory
{
    Task<EstimateFormingModel> PrepareEstimateFormingModelAsync(EstimateFormingSearchModel searchModel);
}