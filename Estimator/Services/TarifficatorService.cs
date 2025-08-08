using System.Globalization;
using ClosedXML.Excel;
using Estimator.Domain;
using Estimator.Domain.Enums;
using Estimator.Inerfaces;
using Microsoft.IdentityModel.Tokens;

namespace Estimator.Services;

public class TarifficatorService: ITarifficatorService
{
    private readonly IRepository<Tarifficator> _tarifficatorRepository;
    private readonly IRepository<Category> _categoryRepository;
    private readonly IRepository<TarifficatorItem> _tarifficatorItemRepository;

    public TarifficatorService(
        IRepository<Tarifficator> tarifficatorRepository,
        IRepository<Category> categoryRepository, IRepository<TarifficatorItem> tarifficatorItemRepository)
    {
        _tarifficatorRepository = tarifficatorRepository;
        _categoryRepository = categoryRepository;
        _tarifficatorItemRepository = tarifficatorItemRepository;
    }

    public async Task CreateTarifficator(IFormFile file,TarifficatorType tarifficatorType)
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
    
        using var workbook = new XLWorkbook(stream);
        var activeTarifficator = (await _tarifficatorRepository.GetAllAsync()).FirstOrDefault(t=>t.IsActive && t.TarifficatorType == tarifficatorType);
        if (activeTarifficator != null)
            await _tarifficatorRepository.DeleteAsync(activeTarifficator);
        var tarificator=new Tarifficator
        {
            TarifficatorItems = new List<TarifficatorItem>(),
            TarifficatorType = tarifficatorType,
            CreatedOn = DateTime.Now,
            IsActive = true
        };
        await _tarifficatorRepository.AddAsync(tarificator);
        switch (tarifficatorType)
        {
            case TarifficatorType.FUL:
                var materialsWorksheet = workbook.Worksheet(1);
                foreach (var row in materialsWorksheet.RowsUsed())
                {
                    var cellValue = row.Cell("A").Value.ToString();
                    if (!cellValue.IsNullOrEmpty() && !cellValue.Contains("поз"))
                    {
                        var priceString = row.Cell("I").Value.ToString().Replace(",", ".").Trim();
                        var item = new TarifficatorItem
                        {
                            ItemCode = row.Cell("A").Value.ToString().Trim(),
                            Name = row.Cell("D").Value.ToString().Trim(),
                            Description = row.Cell("E").Value.ToString().Trim(),
                            TarificatorItemType = TarificatorItemType.Material,
                            Price = decimal.Parse(priceString,  NumberStyles.Any, CultureInfo.InvariantCulture),
                            TarificatorId = tarificator.Id,
                            Tarifficator = tarificator,
                            Discount = string.Empty,
                        };
                        item.Measure = IumEnumHelper.ConvertMeasureTypeString(row.Cell("F").Value.ToString().Trim());
                        item.CurrencyType = IumEnumHelper.ConvertCurrencyTypeString(row.Cell("G").Value.ToString().Trim());

                        if (!row.Cell("B").Value.ToString().Trim().IsNullOrEmpty())
                        {
                            var categoryList = await _categoryRepository.GetAllAsync();
                            var category = categoryList.FirstOrDefault(x => x.Name == row.Cell("B").Value.ToString().Trim());
                            if (category==null)
                            {
                                category = new Category
                                {
                                    Name = row.Cell("B").Value.ToString().Trim(),
                                };
                                await _categoryRepository.AddAsync(category);
                            }
                            item.CategoryId = category.Id;
                            if (!row.Cell("C").Value.ToString().Trim().IsNullOrEmpty())
                            {
                                var subCategory=categoryList.FirstOrDefault(sc=>
                                    sc.Name == row.Cell("C").Value.ToString().Trim() &&
                                    sc.ParentCategoryId == category.Id);

                                if (subCategory == null)
                                {
                                    subCategory = new Category
                                    {
                                        Name = row.Cell("C").Value.ToString().Trim(),
                                        ParentCategoryId = category.Id,
                                    };
                                    await _categoryRepository.AddAsync(subCategory);
                                }
                                item.SubcategoryId = subCategory.Id;
                            }
                        }

                        await _tarifficatorItemRepository.AddAsync(item);
                    }
                }
                
                var servicesWorksheet = workbook.Worksheet(2);
                foreach (var row in servicesWorksheet.RowsUsed())
                {
                    var cellValue = row.Cell("A").Value.ToString();
                    if (!cellValue.IsNullOrEmpty() && !cellValue.Contains("поз"))
                    {
                        var priceString = row.Cell("H").Value.ToString().Replace(",", ".").Trim();
                        var item = new TarifficatorItem
                        {
                            ItemCode = row.Cell("A").Value.ToString().Trim(),
                            Name = row.Cell("C").Value.ToString().Trim(),
                            Description = row.Cell("D").Value.ToString().Trim(),
                            TarificatorItemType = TarificatorItemType.Service,
                            Price = decimal.Parse(priceString,  NumberStyles.Any, CultureInfo.InvariantCulture),
                            TarificatorId = tarificator.Id,
                            Tarifficator = tarificator,
                            Discount = string.Empty,
                        };
                        item.Measure = IumEnumHelper.ConvertMeasureTypeString(row.Cell("E").Value.ToString().Trim());
                        item.CurrencyType = IumEnumHelper.ConvertCurrencyTypeString(row.Cell("F").Value.ToString().Trim());

                        if (!row.Cell("B").Value.ToString().Trim().IsNullOrEmpty())
                        {
                            var categoryList = await _categoryRepository.GetAllAsync();
                            var category = categoryList.FirstOrDefault(x => x.Name == row.Cell("B").Value.ToString().Trim());
                            if (category==null)
                            {
                                category = new Category
                                {
                                    Name = row.Cell("B").Value.ToString().Trim(),
                                };
                                await _categoryRepository.AddAsync(category);
                            }
                            item.CategoryId = category.Id;
                        }

                        await _tarifficatorItemRepository.AddAsync(item);
                    }
                }
                break;
                
            default:
                var worksheet = workbook.Worksheet(1);
                foreach (var row in worksheet.RowsUsed())
                {
                    var cellValue = row.Cell("A").Value.ToString();
                    if (!cellValue.IsNullOrEmpty() && !cellValue.Contains("поз"))
                    {
                        var priceString = row.Cell("I").Value.ToString().Replace(",", ".").Trim();
                        var item = new TarifficatorItem
                        {
                            ItemCode = row.Cell("A").Value.ToString().Trim(),
                            Name = row.Cell("D").Value.ToString().Trim(),
                            Description = row.Cell("E").Value.ToString().Trim(),
                            TarificatorItemType = TarificatorItemType.KtoItem,
                            Price = decimal.Parse(priceString,  NumberStyles.Any, CultureInfo.InvariantCulture),
                            TarificatorId = tarificator.Id,
                            Tarifficator = tarificator,
                            Discount = row.Cell("F").Value.ToString().Trim(),
                        };
                        item.Measure = IumEnumHelper.ConvertMeasureTypeString(row.Cell("G").Value.ToString().Trim());
                        item.CurrencyType = IumEnumHelper.ConvertCurrencyTypeString(row.Cell("H").Value.ToString().Trim());

                        if (!row.Cell("B").Value.ToString().Trim().IsNullOrEmpty())
                        {
                            var categoryList = await _categoryRepository.GetAllAsync();
                            var category = categoryList.FirstOrDefault(x => x.Name == row.Cell("B").Value.ToString().Trim());
                            if (category==null)
                            {
                                category = new Category
                                {
                                    Name = row.Cell("B").Value.ToString().Trim(),
                                };
                                await _categoryRepository.AddAsync(category);
                            }
                            item.CategoryId = category.Id;
                            if (!row.Cell("C").Value.ToString().Trim().IsNullOrEmpty())
                            {
                                var subCategory=categoryList.FirstOrDefault(sc=>
                                    sc.Name == row.Cell("C").Value.ToString().Trim() &&
                                    sc.ParentCategoryId == category.Id);

                                if (subCategory == null)
                                {
                                    subCategory = new Category
                                    {
                                        Name = row.Cell("C").Value.ToString().Trim(),
                                        ParentCategoryId = category.Id,
                                    };
                                    await _categoryRepository.AddAsync(subCategory);
                                }
                                item.SubcategoryId = subCategory.Id;
                            }
                        }

                        await _tarifficatorItemRepository.AddAsync(item);
                    }
                }
                break;
        }
    }
}