using System.Diagnostics;
using System.Globalization;
using ClosedXML.Excel;
using Estimator.Domain;
using Estimator.Domain.Enums;
using Estimator.Inerfaces;
using Microsoft.AspNetCore.Mvc;
using Estimator.Models;
using Estimator.Models.Estimate;
using Estimator.Models.EstimateForming;
using Estimator.Services;

namespace Estimator.Controllers;

public class HomeController : Controller
{
    private readonly ITarifficatorService _tarifficatorService;
    private readonly ITarifficatorModelFactory _tarifficatorModelFactory;
    private readonly IEstimateService _estimateService;
    private readonly IEstimateModelFactory _estimateModelFactory;

    public HomeController(ITarifficatorService tarifficatorService,
        ITarifficatorModelFactory tarifficatorModelFactory,
    IEstimateService estimateService, 
        IEstimateModelFactory estimateModelFactory)
    {
        _tarifficatorService = tarifficatorService;
        _tarifficatorModelFactory = tarifficatorModelFactory;
        _estimateService = estimateService;
        _estimateModelFactory = estimateModelFactory;
        
    }

    public IActionResult Index()
    {
        return View();
    }
    
    public async Task<IActionResult> UploadTarifficator(IFormFile file,TarifficatorType tarifficatorType)
    {
        var errors = new List<string>();
        try
        {
            await _tarifficatorService.CreateTarifficator(file, tarifficatorType);
            return Json(new {success = true});
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }
    public IActionResult EstimateForming()
    {
        return View(new EstimateFormingSearchModel());
    }

    [HttpPost]
    public async Task<IActionResult> EstimateForming(EstimateFormingSearchModel searchModel)
    {
        var model = await _tarifficatorModelFactory.PrepareEstimateFormingModelAsync(searchModel);
        return Json(model);
    }
    public async Task<IActionResult> CreateEstimate(List<EstimateItemMinimizedModel> items, string title,string number,decimal currencyRate,string customerName, bool isDiscounts=false)
    {
        //IumEnumHelper.ConvertTarifficatorTypeFromString(_tarifficatorType)
        List<string>errors=new List<string>();
        try
        {
            var estimate=await _estimateService.CreateEstimateAsync(items,title,number,currencyRate,customerName, isDiscounts);
            
            return Json(new
            {
                success = true,
                estimateId = estimate.Id,
            });
           
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
        
    }

    public async Task<IActionResult> DownloadEstimate(int id)
    {
        var estimate = await _estimateService.GetEstimateByIdAsync(id);

        if (estimate != null)
        {
            var stream = new MemoryStream();
            var workbook = new XLWorkbook();
            var worksheet = workbook.AddWorksheet("Смета");
            
            worksheet.Column(2).Width=13.71;
            worksheet.Column(3).Width=73;
            worksheet.Column(4).Width=5.57;
            worksheet.Column(5).Width=4.71;
            worksheet.Column(6).Width=9.14;
            worksheet.Column(7).Width=33.57;
            
            string imagePath = "../Estimator/Excel/assets/images/logo.png";
            var image = worksheet.AddPicture(imagePath);
            image.MoveTo(worksheet.Cell("B2"));
            image.WithSize(356, 63);
            
            
            worksheet.Cell("B2").Value = "Юридическая информация";
            worksheet.Range("B2", "G2").Merge();
            worksheet.Range("B2", "G2").Style.Alignment.WrapText = true;
            worksheet.Range("B2", "G2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            worksheet.Range("B2", "G2").Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

            worksheet.Cell("B3").Value = $"Сметный расчет на  ДР \u2116 {estimate.Number} от {estimate.Date:dd.MM.yyyy}";
            worksheet.Cell("B3").Style.Font.Bold = true;
            worksheet.Cell("B3").Style.Font.FontSize = 20;
            worksheet.Range("B3", "G3").Merge();
            worksheet.Range("B3", "G3").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("B3", "G3").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            
            var estimateItems= await _estimateModelFactory.PrepareEstimateItemModelList(estimate.Id);
            if (!estimateItems.Any())
            {
                Console.WriteLine("Items List is Empty");
                return null;
            }
            
            var fulMaterials=estimateItems.Where(ei=>
                ei.TarifficatorItem.TarifficatorType==TarifficatorType.FUL &&
                ei.TarifficatorItem.TarificatorItemType==TarificatorItemType.Material).ToList();
            
            var fulServices=estimateItems.Where(ei=>
                ei.TarifficatorItem.TarifficatorType==TarifficatorType.FUL &&
                ei.TarifficatorItem.TarificatorItemType==TarificatorItemType.Service).ToList();
            
            var (ktoMaterials,ktoServices)=_estimateService.SortKtoItems(estimateItems.Where(ei=>
                ei.TarifficatorItem.TarifficatorType==TarifficatorType.KTO).ToList());

            string tariffType = fulMaterials.Any() || fulServices.Any() ? "ФУЛ" :
                ktoMaterials.Any() || ktoServices.Any() ?
                    "КТО"
                    :String.Empty;
            
            worksheet.Cell("B4").Value =
                $"\u2116 в тарификаторе {tariffType} к договору 1 от 01.01.0001";
            worksheet.Range("B4", "B5").Merge();
            worksheet.Range("B4", "B5").Style.Alignment.WrapText = true;
            worksheet.Range("B4", "B5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("B4", "B5").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("B4", "B5").Style.Font.Bold = true;

            worksheet.Cell("C4").Value = "Наименование";
            worksheet.Cell("C4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell("C4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell("C4").Style.Font.Bold = true;
            
            worksheet.Cell("C5").Value = estimate.LocationName;
            worksheet.Cell("C5").Style.Alignment.WrapText = true;
            worksheet.Cell("C5").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell("C5").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell("C5").Style.Font.Bold = true;
            
            worksheet.Cell("D4").Value = "Кол-во";
            worksheet.Cell("D4").Style.Alignment.WrapText = true;
            worksheet.Cell("D4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell("D4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell("D4").Style.Font.Bold = true;
            
            worksheet.Cell("E4").Value = "ед.изм.";
            worksheet.Cell("E4").Style.Alignment.WrapText = true;
            worksheet.Cell("E4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell("E4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell("E4").Style.Font.Bold = true;
            
            worksheet.Cell("F4").Value = "цена  без НДС за ед.из.";
            worksheet.Cell("F4").Style.Alignment.WrapText = true;
            worksheet.Cell("F4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell("F4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell("F4").Style.Font.Bold = true;
            
            worksheet.Cell("G4").Value = "стоимость, рублей без НДС";
            worksheet.Cell("G4").Style.Alignment.WrapText = true;
            worksheet.Cell("G4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell("G4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Cell("G4").Style.Font.Bold = true;
            
            worksheet.Range("B6", "C6").Merge();
            worksheet.Range("B6", "C6").Style.Alignment.WrapText = true;
            worksheet.Range("B6", "C6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("B6", "C6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            worksheet.Range("B6", "C6").Style.Font.Bold = true;

            worksheet.Cell("D6").Value = "Курс ЕВРО:";
            worksheet.Range("D6", "E6").Merge();
            worksheet.Range("D6", "E6").Style.Alignment.WrapText = true;
            worksheet.Range("D6", "E6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Range("D6", "E6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            
            worksheet.Cell("F6").Value = estimate.CurrencyRate;
            worksheet.Cell("F6").Style.NumberFormat.Format = "0.00000";
            worksheet.Cell("F6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Cell("F6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            
            worksheet.Range("B4", "G6").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
            worksheet.Range("B4", "G6").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
            
            int rowNumber=7;
            if (tariffType=="ФУЛ")
            {
                worksheet.Cell("B6").Value = fulMaterials.Any() ? "Материалы:" : "Услуги:";
                worksheet.Cell("B6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                worksheet.Cell("B6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }
            else
            {
                worksheet.Cell("B6").Value = ktoMaterials.Any() ? "Материалы:" : "Услуги:";
                worksheet.Cell("B6").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                worksheet.Cell("B6").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }

            if (fulMaterials.Any() || fulServices.Any())
            {
                if (fulMaterials.Any())
                {
                    foreach (var fulMaterialsItem in fulMaterials)
                    {
                        FillItemRow(worksheet,fulMaterialsItem,estimate,rowNumber);
                        rowNumber++;
                    }
                }

                if (fulServices.Any())
                {
                    if (fulMaterials.Any())
                    {
                        worksheet.Cell($"B{rowNumber}").Value = "Услуги:";
                        worksheet.Row(rowNumber).Height=25.5;
                        worksheet.Cell($"B{rowNumber}").Style.Font.Bold = true;
                        worksheet.Cell($"B{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell($"B{rowNumber}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        worksheet.Range($"B{rowNumber}", $"G{rowNumber}").Merge();
                        worksheet.Range($"B{rowNumber}", $"G{rowNumber}").Style.Alignment.WrapText = true;
                        worksheet.Cell($"B{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                        worksheet.Cell($"B{rowNumber}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        worksheet.Range($"B{rowNumber}", $"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
                        worksheet.Range($"B{rowNumber}", $"G{rowNumber}").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
                        rowNumber++;
                    }
                    foreach (var fulServicesItem in fulServices)
                    {
                        FillItemRow(worksheet,fulServicesItem,estimate,rowNumber);
                        rowNumber++;
                    }
                }
            }
            
            if (ktoMaterials.Any() || ktoServices.Any())
            {
                if (fulMaterials.Any() || fulServices.Any())
                {
                    worksheet.Cell($"B{rowNumber}").Value =
                        "\u2116 в тарификаторе КТО к договору 1 от 01.01.0001";
                    worksheet.Cell($"B{rowNumber}").Style.Alignment.WrapText = true;
                    worksheet.Cell($"B{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell($"B{rowNumber}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Cell($"B{rowNumber}").Style.Font.Bold = true;

                    worksheet.Cell($"C{rowNumber}").Value = "Наименование";
                    worksheet.Cell($"C{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell($"C{rowNumber}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Cell($"C{rowNumber}").Style.Font.Bold = true;

                    worksheet.Cell($"D{rowNumber}").Value = "Кол-во";
                    worksheet.Cell($"D{rowNumber}").Style.Alignment.WrapText = true;
                    worksheet.Cell($"D{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell($"D{rowNumber}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Cell($"D{rowNumber}").Style.Font.Bold = true;

                    worksheet.Cell($"E{rowNumber}").Value = "ед.изм.";
                    worksheet.Cell($"E{rowNumber}").Style.Alignment.WrapText = true;
                    worksheet.Cell($"E{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell($"E{rowNumber}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Cell($"E{rowNumber}").Style.Font.Bold = true;

                    worksheet.Cell($"F{rowNumber}").Value = "цена  без НДС за ед.из.";
                    worksheet.Cell($"F{rowNumber}").Style.Alignment.WrapText = true;
                    worksheet.Cell($"F{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell($"F{rowNumber}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Cell($"F{rowNumber}").Style.Font.Bold = true;

                    worksheet.Cell($"G{rowNumber}").Value = "стоимость, рублей без НДС";
                    worksheet.Cell($"G{rowNumber}").Style.Alignment.WrapText = true;
                    worksheet.Cell($"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    worksheet.Cell($"G{rowNumber}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Cell($"G{rowNumber}").Style.Font.Bold = true;
                    
                    worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
                    worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);

                    rowNumber++;
                }

                if (ktoMaterials.Any())
                {
                    worksheet.Cell($"B{rowNumber}").Value = "Материалы:";
                    worksheet.Row(rowNumber).Height=25.5;
                    worksheet.Cell($"B{rowNumber}").Style.Font.Bold = true;
                    worksheet.Cell($"B{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Cell($"B{rowNumber}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Range($"B{rowNumber}", $"G{rowNumber}").Merge();
                    worksheet.Range($"B{rowNumber}", $"G{rowNumber}").Style.Alignment.WrapText = true;
                    worksheet.Cell($"B{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Range($"B{rowNumber}", $"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
                    rowNumber++;
                    foreach (var ktoMaterialsItem in ktoMaterials)
                    {
                        FillItemRow(worksheet,ktoMaterialsItem,estimate,rowNumber);
                        rowNumber++;
                    }
                }

                if (ktoServices.Any())
                {
                    worksheet.Cell($"B{rowNumber}").Value = "Услуги:";
                    worksheet.Row(rowNumber).Height=25.5;
                    worksheet.Cell($"B{rowNumber}").Style.Font.Bold = true;
                    worksheet.Cell($"B{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Cell($"B{rowNumber}").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                    worksheet.Range($"B{rowNumber}", $"G{rowNumber}").Merge();
                    worksheet.Range($"B{rowNumber}", $"G{rowNumber}").Style.Alignment.WrapText = true;
                    worksheet.Cell($"B{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                    worksheet.Range($"B{rowNumber}", $"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
                    rowNumber++;
                    
                    foreach (var ktoServicesItem in ktoServices)
                    {
                        FillItemRow(worksheet,ktoServicesItem,estimate,rowNumber);
                        rowNumber++;
                    }
                }
            }
            
            worksheet.Range("B7", $"G{rowNumber-1}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
            
            decimal grandTotal = CalculateItemsTotalInRub(estimateItems,estimate.CurrencyRate);
            
            worksheet.Cell($"B{rowNumber}").Value="ИТОГО за услуги без НДС:";
            worksheet.Row(rowNumber).Height=25.5;
            worksheet.Range($"B{rowNumber}",$"F{rowNumber}").Merge();
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Font.Bold = true;
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
            worksheet.Cell($"G{rowNumber}").Value=fulServices.Sum(fs=>fs.Total)+ktoServices.Sum(fs=>fs.Total);
            worksheet.Cell($"G{rowNumber}").Style.NumberFormat.Format = "# ### ##0.00";
            rowNumber++;

            if (fulMaterials.Any())
            {
                worksheet.Cell($"B{rowNumber}").Value="ИТОГО за материалы по Фулл без НДС:";
                worksheet.Row(rowNumber).Height=25.5;
                worksheet.Range($"B{rowNumber}",$"F{rowNumber}").Merge();
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Font.Bold = true;
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
                worksheet.Cell($"G{rowNumber}").Value=fulMaterials.Sum(fs=>fs.Total);
                worksheet.Cell($"G{rowNumber}").Style.NumberFormat.Format = "# ### ##0.00";
                rowNumber++;
            }
            
            if (ktoMaterials.Any())
            {
                worksheet.Cell($"B{rowNumber}").Value="ИТОГО за материалы по КТО без НДС:";
                worksheet.Row(rowNumber).Height=25.5;
                worksheet.Range($"B{rowNumber}",$"F{rowNumber}").Merge();
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Font.Bold = true;
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
                worksheet.Cell($"G{rowNumber}").Value=ktoMaterials.Sum(fs=>fs.Total);
                worksheet.Cell($"G{rowNumber}").Style.NumberFormat.Format = "# ### ##0.00";
                rowNumber++;
            }

            if (estimate.IsDiscounts)
            {
                decimal materialsTotal = CalculateItemsTotalInRub(fulMaterials, estimate.CurrencyRate) +
                                         CalculateItemsTotalInRub(ktoMaterials, estimate.CurrencyRate);
                
                worksheet.Cell($"B{rowNumber}").Value="Демонтажные работы (%)";
                worksheet.Row(rowNumber).Height=25.5;
                worksheet.Range($"B{rowNumber}",$"E{rowNumber}").Merge();
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Font.Bold = true;
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
                worksheet.Cell($"G{rowNumber}").FormulaA1=$"={materialsTotal.ToString(CultureInfo.InvariantCulture)}/100*F{rowNumber}";
                worksheet.Cell($"G{rowNumber}").Style.NumberFormat.Format = "# ### ##0.00";
                rowNumber++;
                
                worksheet.Cell($"B{rowNumber}").Value="Монтажные работы (%)";
                worksheet.Row(rowNumber).Height=25.5;
                worksheet.Range($"B{rowNumber}",$"E{rowNumber}").Merge();
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Font.Bold = true;
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
                worksheet.Cell($"G{rowNumber}").FormulaA1=$"={materialsTotal.ToString(CultureInfo.InvariantCulture)}/100*F{rowNumber}";
                worksheet.Cell($"G{rowNumber}").Style.NumberFormat.Format = "# ### ##0.00";
                rowNumber++;
                
                worksheet.Cell($"B{rowNumber}").Value="Расходные материалы (%)";
                worksheet.Row(rowNumber).Height=25.5;
                worksheet.Range($"B{rowNumber}",$"E{rowNumber}").Merge();
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Font.Bold = true;
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
                worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
                worksheet.Cell($"G{rowNumber}").FormulaA1=$"={materialsTotal.ToString(CultureInfo.InvariantCulture)}/100*F{rowNumber}";
                worksheet.Cell($"G{rowNumber}").Style.NumberFormat.Format = "# ### ##0.00";
                rowNumber++;
            }
            
            worksheet.Cell($"B{rowNumber}").Value="ИТОГО без НДС:";
            worksheet.Row(rowNumber).Height=25.5;
            worksheet.Range($"B{rowNumber}",$"F{rowNumber}").Merge();
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Font.Bold = true;
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
            worksheet.Cell($"G{rowNumber}").FormulaA1=$"{grandTotal.ToString(CultureInfo.InvariantCulture)}+G{rowNumber-3}+G{rowNumber-2}+G{rowNumber-1}";
            worksheet.Cell($"G{rowNumber}").Style.NumberFormat.Format = "# ### ##0.00";
            rowNumber++;

            worksheet.Cell($"B{rowNumber}").Value="НДС 20%:";
            worksheet.Row(rowNumber).Height=25.5;
            worksheet.Range($"B{rowNumber}",$"F{rowNumber}").Merge();
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Font.Bold = true;
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
            worksheet.Cell($"G{rowNumber}").FormulaA1=$"G{rowNumber-1}*0.2";
            worksheet.Cell($"G{rowNumber}").Style.NumberFormat.Format = "# ### ##0.00";
            rowNumber++;

            worksheet.Cell($"B{rowNumber}").Value="Итого по смете с НДС:";
            worksheet.Row(rowNumber).Height=25.5;
            worksheet.Range($"B{rowNumber}",$"F{rowNumber}").Merge();
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Font.Bold = true;
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
            worksheet.Cell($"G{rowNumber}").FormulaA1=$"G{rowNumber-2}+G{rowNumber-1}";
            worksheet.Cell($"G{rowNumber}").Style.NumberFormat.Format = "# ### ##0.00";
            rowNumber+=2;
            
            worksheet.Cell($"B{rowNumber}").Value="* срок поставки и выполнения работ с момента согласования - 4-8 раб. недели";
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Merge();
            rowNumber+=3;
            
            worksheet.Cell($"B{rowNumber}").Value="* Подписание сметного расчета, означает согласование Заказчика с собственником проведение всех работ,  замены материалов и запчастей";
            worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Merge();
            rowNumber+=2;
            
            worksheet.Cell($"B{rowNumber}").Value="Исполнитель";
            worksheet.Range($"B{rowNumber}",$"C{rowNumber}").Merge();
            worksheet.Range($"B{rowNumber}",$"C{rowNumber}").Style.Font.Bold = true;
            
            worksheet.Cell($"F{rowNumber}").Value="Заказчик";
            worksheet.Range($"F{rowNumber}",$"G{rowNumber}").Merge();
            worksheet.Range($"F{rowNumber}",$"G{rowNumber}").Style.Font.Bold = true;
            worksheet.Range($"F{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            rowNumber++;
            
            worksheet.Cell($"B{rowNumber}").Value="ООО \"Название компании\"";
            worksheet.Range($"B{rowNumber}",$"C{rowNumber}").Merge();
            worksheet.Range($"B{rowNumber}",$"C{rowNumber}").Style.Font.Bold = true;
            
            worksheet.Cell($"F{rowNumber}").Value=$"{estimate.CustomerName}";
            worksheet.Range($"F{rowNumber}",$"G{rowNumber}").Merge();
            worksheet.Range($"F{rowNumber}",$"G{rowNumber}").Style.Font.Bold = true;
            worksheet.Range($"F{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            rowNumber++;
            
            worksheet.Cell($"D{rowNumber}").Value="согласовано, оплату гарантируем";
            worksheet.Range($"D{rowNumber}",$"G{rowNumber}").Merge();
            worksheet.Range($"D{rowNumber}",$"G{rowNumber}").Style.Font.Bold = true;
            worksheet.Range($"D{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            rowNumber++;
            
            worksheet.Cell($"B{rowNumber}").Value="___________________/Иванов И.И./";
            worksheet.Range($"B{rowNumber}",$"C{rowNumber}").Merge();
            
            worksheet.Cell($"E{rowNumber}").Value="_________________________/__________________/";
            worksheet.Range($"E{rowNumber}",$"G{rowNumber}").Merge();
            worksheet.Range($"E{rowNumber}",$"G{rowNumber}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            worksheet.Row(rowNumber).Height = 48.75;
            
            worksheet.Row(2).Height = 106.5;
            worksheet.Row(3).Height = 57;
            worksheet.Row(4).Height = 71.25;
            //worksheet.Row(5).Height = 72;
            worksheet.Row(6).Height = 25.5;
            
            workbook.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Смета - {estimate.LocationName}.xlsx");
            
        }
        else
        {
            return Json(new {success = false, errors="Estimate not found"});
        }
    }

    public async Task<IActionResult> EstimateList()
    {
        var model = await _estimateModelFactory.PrepareEstimateListModel();
        return View(model);
    }

    private void FillItemRow(IXLWorksheet worksheet,EstimateItemModel fulMaterialsItem,Estimate estimate,int rowNumber)
    {
        worksheet.Cell($"B{rowNumber}").Value = $"{fulMaterialsItem.TarifficatorItem.ItemCode}";
        worksheet.Cell($"C{rowNumber}").Value = $"{fulMaterialsItem.TarifficatorItem.Name}";
        worksheet.Cell($"D{rowNumber}").Value = fulMaterialsItem.Qty;
        worksheet.Cell($"G{rowNumber}").Style.NumberFormat.Format = "# ### ##0.00";
        worksheet.Cell($"E{rowNumber}").Value = EnumHelper.ConvertMeasureTypeToString(fulMaterialsItem.TarifficatorItem.Measure);
        worksheet.Cell($"F{rowNumber}").Value = fulMaterialsItem.TarifficatorItem.Price;
        worksheet.Cell($"F{rowNumber}").Style.NumberFormat.Format = "# ### ##0.00";
        worksheet.Cell($"G{rowNumber}").Value = fulMaterialsItem.TarifficatorItem.CurrencyType==CurrencyType.RUB?
            fulMaterialsItem.Total:
            fulMaterialsItem.Total*estimate.CurrencyRate;
        worksheet.Cell($"G{rowNumber}").Style.NumberFormat.Format = "# ### ##0.00";
        worksheet.Row(rowNumber).Style.Alignment.Vertical=XLAlignmentVerticalValues.Center;
        worksheet.Range($"B{rowNumber}",$"G{rowNumber}").Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
    }

    private decimal CalculateItemsTotalInRub(List<EstimateItemModel> items, decimal currencyRate)
    {
        decimal totalInRub = 0;
        foreach (var item in items)
        {
            totalInRub+=item.TarifficatorItem.CurrencyType==CurrencyType.RUB?
                item.Total:
                item.Total*currencyRate;
        }
        return totalInRub;
    }
}