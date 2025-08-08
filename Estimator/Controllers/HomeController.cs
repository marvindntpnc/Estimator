using System.Diagnostics;
using Estimator.Domain.Enums;
using Estimator.Inerfaces;
using Microsoft.AspNetCore.Mvc;
using Estimator.Models;
using Estimator.Models.EstimateForming;

namespace Estimator.Controllers;

public class HomeController : Controller
{
    private readonly ITarifficatorService _tarifficatorService;
    private readonly ITarifficatorModelFactory _tarifficatorModelFactory;

    public HomeController(ITarifficatorService tarifficatorService,
        ITarifficatorModelFactory tarifficatorModelFactory)
    {
        _tarifficatorService = tarifficatorService;
        _tarifficatorModelFactory = tarifficatorModelFactory;
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
}