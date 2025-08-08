using Estimator.Models.Estimate;

namespace Estimator.Inerfaces;

public interface IEstimateModelFactory
{
    public Task<List<EstimateModel>> PrepareEstimateListModel();
    public Task<List<EstimateItemModel>> PrepareEstimateItemModelList(int estimateId);
}