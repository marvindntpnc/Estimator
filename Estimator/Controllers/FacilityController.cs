using Estimator.Domain;
using Estimator.Inerfaces;
using Estimator.Models.Facility;
using Microsoft.AspNetCore.Mvc;

namespace Estimator.Controllers;

public class FacilityController: Controller
{
    private readonly IFacilityModelFactory _facilityModelFactory;
    private IFacilityService _facilityService;

    public FacilityController(IFacilityModelFactory facilityModelFactory, 
        IFacilityService facilityService)
    {
        _facilityModelFactory = facilityModelFactory;
        _facilityService = facilityService;
    }

    public IActionResult FacilityList()
    {
        return View(new FacilitySearchModel());
    }
    
    [HttpPost]
    public async Task<IActionResult> FacilityList(FacilitySearchModel searchModel)
    {
        var errors = new List<string>();
        try
        {
            var pagedItems = await _facilityModelFactory.PrepareFacilityPagedListAsync(searchModel);
            
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

    public async Task<IActionResult> CreateFacility()
    {
        var facility = new Facility();
        await _facilityService.CreateFacilityAsync(facility);
        return RedirectToAction("UpdateFacility",new{id=facility.Id});
    }

    public async Task<IActionResult> UpdateFacility(int id)
    {
        var model = await _facilityModelFactory.PrepareCreateOrEditFacilityModelAsync(id);
        
        return View(model);
    }

    public async Task<IActionResult> RemoveFacility(int id)
    {
        await _facilityService.DeleteFacilityAsync(id);
        return RedirectToAction("FacilityList");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateFacility([FromBody] CreateOrUpdateFacilityModel model)
    {
        var errors = new List<string>();
        try
        {
            await _facilityService.UpdateFacilityAsync(model);
            return Json(new {success = true});
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddFacilityContract([FromBody] ContractModel model)
    {
        var errors = new List<string>();
        try
        {
            await _facilityService.AddOrUpdateFacilityContractAsync(model);
            return Json(new {success = true});
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddFacilityDiscount([FromBody] DiscountRequirementModel model)
    {
        var errors = new List<string>();
        try
        {
            await _facilityService.AddFacilityDiscountAsync(model);
            return Json(new {success = true});
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }

    public async Task<IActionResult> RemoveFacilityDiscount(int id, int facilityId)
    {
        await _facilityService.DeleteFacilityDiscountAsync(id);
        return RedirectToAction("UpdateFacility", new {id = facilityId});
    }

    [HttpPost]
    public async Task<IActionResult> GetFacilityContractList([FromBody]int facilityId)
    {
        var errors = new List<string>();
        try
        {
            var contracts=await _facilityService.GetFacilityContractsAsync(facilityId);
            return Json(new
            {
                success = true,
                contracts=contracts
            });
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetContractById([FromBody] int contractId)
    {
        var errors = new List<string>();
        try
        {
            var contract = await _facilityService.GetContractByIdAsync(contractId);
            return Json(new
            {
                success = true,
                contract=contract
            });
        }
        catch (Exception e)
        {
            errors.Add(e.Message);
            return Json(new {success = false, errors = errors});
        }
    }
}