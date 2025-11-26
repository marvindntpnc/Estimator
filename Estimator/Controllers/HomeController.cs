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
using Microsoft.IdentityModel.Tokens;

namespace Estimator.Controllers;

public class HomeController : Controller
{
    
    private readonly ITarifficatorService _tarifficatorService;
    private readonly ITarifficatorModelFactory _tarifficatorModelFactory;
    private readonly IEstimateService _estimateService;
    private readonly IEstimateModelFactory _estimateModelFactory;
    private readonly IExcelService _excelService;

    public HomeController(
        ITarifficatorService tarifficatorService,
        ITarifficatorModelFactory tarifficatorModelFactory, 
        IEstimateService estimateService, 
        IEstimateModelFactory estimateModelFactory,
        IExcelService excelService)
    {
        _tarifficatorService = tarifficatorService;
        _tarifficatorModelFactory = tarifficatorModelFactory;
        _estimateService = estimateService;
        _estimateModelFactory = estimateModelFactory;
        _excelService = excelService;
    }

    public IActionResult Index()
    {
        return RedirectToAction("EstimateList");
    }

    public IActionResult UploadTarifficator()
    {
        return View();
    }
    
    public async Task<IActionResult> EstimateForming()
    {
        var model =await _tarifficatorModelFactory.PrepareEstimateFormingSearchModelAsync();
        return View(model);
    }
    
    public async Task<IActionResult> EstimateList()
    {
        var model = await _estimateModelFactory.PrepareEstimateSearchModel();
        return View(model);
    }
    
    [HttpPost]
    public async Task<IActionResult> EstimateList(EstimateSearchModel searchModel)
    {
        var errors = new List<string>();
        try
        {
            var pagedItems = await _estimateService.GetEstimateModelPagedListAsync(searchModel);
            
            return Json(new
            {
                draw = searchModel.Draw,
                recordsTotal = pagedItems.TotalItemCount,
                recordsFiltered = pagedItems.TotalItemCount,
                data = pagedItems
            });
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }

    public async Task<IActionResult> CreateEstimate()
    {
        var estimate=new Estimate
        {
            CreatedOn = DateTime.Now
        };
        await _estimateService.CreateEstimateAsync(estimate);
        return RedirectToAction("UpdateEstimate",new{id=estimate.Id});
    }

    public async Task<IActionResult> UpdateEstimate(int id)
    {
        var model = await _estimateModelFactory.PrepareUpdateEstimateModelAsync(id);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateEstimate([FromBody] UpdateEstimateModel model)
    {
        var errors = new List<string>();
        try
        {
            await _estimateService.UpdateEstimateAsync(model);
            
            return Json(new
            {
                success=true
            });
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }

    [HttpPost]
    public async Task<IActionResult> EstimateItemsList(EstimateItemSearchModel searchModel)
    {
        var errors = new List<string>();
        try
        {
            var pagedItemModelList = await _estimateModelFactory.PrepareEstimateItemModelPagedList(searchModel);
            
            return Json(new
            {
                draw = searchModel.Draw,
                recordsTotal = pagedItemModelList.TotalItemCount,
                recordsFiltered = pagedItemModelList.TotalItemCount,
                data = pagedItemModelList
            });
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }

    public async Task<IActionResult> ResetApp()
    {
        await _tarifficatorService.ResetAppAsync();
        return RedirectToAction("Index");
    }
    
    /// <summary>
    /// Method to update Tarifficator Items in DB.
    /// Take information (Excel doc and Tarifficator Type) provided by the user on page and analyze every row of Excel-table,
    /// converting it in Tarifficator Item and save in DB. 
    /// </summary>
    /// <param name="file">Excel-doc downloaded by user</param>
    /// <param name="tarifficatorType">Type of tarifficator</param>
    /// <returns>Json-object with errors list or success-message. Alert-message on UI with results.</returns>
    [HttpPost]
    public async Task<IActionResult> UploadTarifficatorExcel(IFormFile file,TarifficatorType tarifficatorType)
    {
        var errors = new List<string>();
        try
        {
            var resultModel = await _tarifficatorService.CreateTarifficatorAsync(file, tarifficatorType);
            return Json(new
            {
                success = true,
                result = resultModel
            });
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }
    
    /// <summary>
    /// Method to forming Tarifficator Item Tables using DataTables.
    /// Include Order, sort and filer options.
    /// </summary>
    /// <param name="searchModel">Model for filtering items by ItemName, Category, SubCategory, Currency, Measure</param>
    /// <param name="tarifficatorType">Type of tarifficator</param>
    /// <returns>Json-object with data and assistive info for filling DataTable on page</returns>
    [HttpPost]
    public async Task<IActionResult> EstimateForming(EstimateFormingSearchModel searchModel, TarifficatorType tarifficatorType)
    {
        var errors = new List<string>();
        try
        {
            var pagedItems = await _tarifficatorService.GetTarifficatorItemsAsync(searchModel, tarifficatorType);
            var modelItems = new List<TarrificatorItemModel>();
            foreach (var item in pagedItems)
            {
                modelItems.Add(await _tarifficatorModelFactory.PrepareTarifficatorItemModel(item));
            }

            return Json(new
            {
                draw = searchModel.Draw,
                recordsTotal = pagedItems.TotalItemCount,
                recordsFiltered = pagedItems.TotalItemCount,
                data = modelItems
            });
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }

    /// <summary>
    /// Method to forming and downloading Excel-document for Estimate.
    /// Includes info about Estimate Items and summary calculation.
    /// </summary>
    /// <param name="id">Estimate Id</param>
    /// <returns>Excel-document designed to be sent to customers</returns>
    public async Task<IActionResult> DownloadEstimate(int id)
    {
        var estimate = await _estimateService.GetEstimateByIdAsync(id);

        if (estimate != null)
        {
            string formatedName=estimate.EstimateName.Replace(" ","_");
            var stream = new MemoryStream();
            var workbook = await _excelService.FormEstimateExcelBookAsync(estimate);
            workbook.SaveAs(stream);
            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"Смета_на_ДР_№{estimate.Number}_от_{estimate.CreatedOn:dd_MM_yyyy}_{formatedName}.xlsx");
            
        }
        else
        {
            return Json(new {success = false, errors="Estimate not found"});
        }
    }

    /// <summary>
    /// Method for changing the type of position, which is required in case of inaccurate or erroneous processing by the initial tarifficator.
    /// It also changes the method of adding an item to the estimate: when the parameter "isCustomAdding" is set to true,
    /// additional options will be displayed for more accurate cost calculation.
    /// </summary>
    /// <returns>Json-object with errors list or success-message. Alert-message on UI with results.</returns>
    [HttpPost]
    public async Task<IActionResult> UpdateTarifficatorItemInfo(int id,TarificatorItemType tarifficatorItemType, bool isCustomAdding)
    {
        var errors = new List<string>();
        try
        {
            await _tarifficatorService.UpdateTarifficatorItemInfoAsync(id,tarifficatorItemType, isCustomAdding);
            return Json(new {success = true});
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }

    [HttpPost]
    public async Task<IActionResult> ValidateEstimate([FromBody]int id)
    {
        var estimate = await _estimateService.GetEstimateByIdAsync(id);
        if (estimate==null ||
            estimate.EstimateName.IsNullOrEmpty() ||
            estimate.Number.IsNullOrEmpty() ||
            estimate.FacilityId<1 ||
            estimate.ContractId<1)
        {
            return Json(new {success = false});
        }
        else
        {
            return Json(new {success = true});
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddEstimateItem([FromBody] EstimateItemMinimizedModel estimateItemModel)
    {
        var errors = new List<string>();
        try
        {
            await _estimateService.AddEstimateItemAsync(estimateItemModel);
            return Json(new {success = true});
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteEstimateItem([FromBody] int estimateItemId)
    {
        var errors = new List<string>();
        try
        {
            await _estimateService.DeleteEstimateItemAsync(estimateItemId);
            return Json(new {success = true});
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }
    [HttpPost]
    public async Task<IActionResult> UpdateEstimateItem([FromBody] EstimateItemMinimizedModel estimateItemModel)
    {
        var errors = new List<string>();
        try
        {
            await _estimateService.UpdateEstimateItemAsync(estimateItemModel);
            return Json(new {success = true});
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }

    public async Task<IActionResult> DeleteEstimate(int estimateId)
    {
        await _estimateService.DeleteEstimateAsync(estimateId);
        return RedirectToAction("EstimateList");
    }
}