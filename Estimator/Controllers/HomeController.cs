using System.Diagnostics;
using Estimator.Domain.Enums;
using Estimator.Inerfaces;
using Microsoft.AspNetCore.Mvc;
using Estimator.Models;

namespace Estimator.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ITarifficatorService _tarifficatorService;

    public HomeController(ILogger<HomeController> logger, ITarifficatorService tarifficatorService)
    {
        _logger = logger;
        _tarifficatorService = tarifficatorService;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
    
    public async Task<IActionResult> Download(IFormFile file,TarifficatorType tarifficatorType)
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
}