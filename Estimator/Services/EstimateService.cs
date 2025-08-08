using Estimator.Domain;
using Estimator.Inerfaces;
using Estimator.Models.Estimate;

namespace Estimator.Services;

public class EstimateService : IEstimateService
{
    private readonly IRepository<Estimate> _estimateRepository;
    private readonly IRepository<EstimateItem> _estimateItemRepository;
    private readonly ITarifficatorService _tarifficatorService;

    public EstimateService(
        IRepository<Estimate> estimateRepository,
        IRepository<EstimateItem> estimateItemRepository,
        ITarifficatorService tarifficatorService)
    {
        _estimateRepository = estimateRepository;
        _estimateItemRepository = estimateItemRepository;
        _tarifficatorService = tarifficatorService;
    }

    public async Task<Estimate> CreateEstimateAsync(List<EstimateItemMinimizedModel> items, string title, string number,
        decimal currencyRate, string customerName, bool isDiscounts = false)
    {
        var estimate = new Estimate
        {
            Date = DateTime.Now,
            Number = number,
            LocationName = title,
            CurrencyRate = currencyRate,
            IsDiscounts = isDiscounts,
            CustomerName = customerName,
            EstimateItems = new List<EstimateItem>()
        };
        await _estimateRepository.AddAsync(estimate);

        foreach (var item in items)
        {
            if (item.Qty > 0)
            {
                await _estimateItemRepository.AddAsync(new EstimateItem
                {
                    EstimateId = estimate.Id,
                    Qty = item.Qty,
                    TarifficatorItemId = item.ItemId,
                });
            }
        }

        return estimate;
    }

    public async Task<List<Estimate>> GetEstimateList()
    {
        return await _estimateRepository.GetAllAsync();
    }

    public async Task<Estimate?> GetEstimateByIdAsync(int id)
    {
        return await _estimateRepository.GetByIdAsync(id);
    }

    public List<EstimateItem> GetEstimateItemsByEstimateIdAsync(int estimateId)
    {
        return _estimateItemRepository.GetWhereAsync(ei => ei.EstimateId == estimateId);
    }

    public (List<EstimateItemModel>, List<EstimateItemModel>) SortKtoItems(List<EstimateItemModel> items)
    {
        var ktoServices = items.Where(i =>
            i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("Доставка - вывоз")?.Id ||
            i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("Предоставление ДГУ")?.Id ||
            i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("Услуги ассенизации")?.Id ||
            i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("Аренда спецтехники")?.Id ||
            i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("Спец.услуги")?.Id ||

            (i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("Благоустройство")?.Id &&
             i.TarifficatorItem.SubcategoryId == _tarifficatorService.GetCategoryByName("Дорожная разметка")?.Id) ||

            (i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("Реклама")?.Id &&
             i.TarifficatorItem.SubcategoryId == _tarifficatorService.GetCategoryByName("Услуги по рекламе")?.Id) ||

            (i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("РЦ. Услуги и материалы")?.Id &&
             i.TarifficatorItem.SubcategoryId == _tarifficatorService.GetCategoryByName("Промышленные полы")?.Id) ||

            (i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("РЦ. Услуги и материалы")?.Id &&
             i.TarifficatorItem.SubcategoryId ==
             _tarifficatorService.GetCategoryByName("Стеллажи с глубинными полками")?.Id) ||

            (i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("РЦ. Услуги и материалы")?.Id &&
             i.TarifficatorItem.SubcategoryId ==
             _tarifficatorService.GetCategoryByName("Стеллажи с гравитац. полками")?.Id) ||

            (i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("РЦ. Услуги и материалы")?.Id &&
             i.TarifficatorItem.SubcategoryId == _tarifficatorService.GetCategoryByName("Стеллажи фронтальные")?.Id) ||

            (i.TarifficatorItem.CategoryId ==
             _tarifficatorService.GetCategoryByName("Узлы учёта тепловой энергии")?.Id &&
             i.TarifficatorItem.SubcategoryId ==
             _tarifficatorService.GetCategoryByName("Специализированные Работы")?.Id) ||

            (i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("Электрика")?.Id &&
             i.TarifficatorItem.SubcategoryId == _tarifficatorService.GetCategoryByName("Прочее")?.Id &&
             i.TarifficatorItem.ItemCode == "10.29.7") ||

            (i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("Электрика")?.Id &&
             i.TarifficatorItem.SubcategoryId == _tarifficatorService.GetCategoryByName("Прочее")?.Id &&
             i.TarifficatorItem.ItemCode == "10.29.8") ||

            (i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("Электрика")?.Id &&
             i.TarifficatorItem.SubcategoryId == _tarifficatorService.GetCategoryByName("Прочее")?.Id &&
             i.TarifficatorItem.ItemCode == "10.29.9") ||

            (i.TarifficatorItem.CategoryId == _tarifficatorService.GetCategoryByName("Электрика")?.Id &&
             i.TarifficatorItem.SubcategoryId == _tarifficatorService.GetCategoryByName("Прочее")?.Id &&
             i.TarifficatorItem.ItemCode == "10.29.24")).ToList();

        var ktoMaterials = new List<EstimateItemModel>(items);
        foreach (var item in items)
        {
            if (ktoServices.Any(ks => ks == item))
                ktoMaterials.Remove(item);
        }

        return (ktoMaterials, ktoServices);
    }
}