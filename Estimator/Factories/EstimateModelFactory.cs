using Estimator.Domain;
using Estimator.Inerfaces;
using Estimator.Models.Estimate;

namespace Estimator.Factories;

public class EstimateModelFactory : IEstimateModelFactory
{
    private readonly IEstimateService _estimateService;
    private readonly IRepository<TarifficatorItem> _tarifficatorItemRepository;
    private readonly ITarifficatorModelFactory _tarifficatorModelFactory;

    public EstimateModelFactory(
        IEstimateService estimateService,
        IRepository<TarifficatorItem> tarifficatorItemRepository, ITarifficatorModelFactory tarifficatorModelFactory)
    {
        _estimateService = estimateService;
        _tarifficatorItemRepository = tarifficatorItemRepository;
        _tarifficatorModelFactory = tarifficatorModelFactory;
    }

    public async Task<List<EstimateModel>> PrepareEstimateListModel()
    {
        var model = new List<EstimateModel>();
        var estimates = await _estimateService.GetEstimateList();
        foreach (var estimate in estimates)
        {
            model.Add(new EstimateModel
            {
                Name = estimate.LocationName,
                Date = estimate.Date,
                Id = estimate.Id,
            });
        }

        return model;
    }

    public async Task<List<EstimateItemModel>> PrepareEstimateItemModelList(int estimateId)
    {
        var model = new List<EstimateItemModel>();
        var estimateItems = _estimateService.GetEstimateItemsByEstimateIdAsync(estimateId);
        foreach (var estimateItem in estimateItems)
        {
            var tarifficatorItem = await _tarifficatorItemRepository.GetByIdAsync(estimateItem.TarifficatorItemId);
            if (tarifficatorItem != null)
            {
                model.Add(new EstimateItemModel
                {
                    Id = estimateItem.Id,
                    EstimateId = estimateItem.EstimateId,
                    Qty = estimateItem.Qty,
                    TarifficatorItem = await _tarifficatorModelFactory.PrepareTarifficatorItemModel(tarifficatorItem)
                });
            }
        }

        return model;
    }
}