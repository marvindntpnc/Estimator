using Estimator.Domain;
using Estimator.Inerfaces;
using Estimator.Models.Facility;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PagedList;

namespace Estimator.Services;

public class FacilityService: IFacilityService
{
    private readonly IRepository<Facility> _facilityRepository;
    private readonly IRepository<Contract> _contractRepository;
    private readonly IRepository<DiscountRequirement> _discountRepository;

    public FacilityService(IRepository<Facility> facilityRepository,
        IRepository<Contract> contractRepository,
        IRepository<DiscountRequirement> discountRepository)
    {
        _facilityRepository = facilityRepository;
        _contractRepository = contractRepository;
        _discountRepository = discountRepository;
    }

    public async Task<IPagedList<Facility?>> GetFacilityPagedListAsync(FacilitySearchModel searchModel)
    {
        var query = _facilityRepository.Table;
        if (!searchModel.FacilityName.IsNullOrEmpty())
        {
            query = query.Where(f => f.Name.ToLower().Contains(searchModel.FacilityName.ToLower()));
        }
        
        var result=await query.ToPagedListAsync(searchModel.PageIndex + 1, searchModel.PageSize);

        return result;
    }

    public async Task<Facility?> GetFacilityByIdAsync(int facilityId)
    {
        return await _facilityRepository.GetByIdAsync(facilityId);
    }

    public async Task CreateFacilityAsync(Facility facility)
    {
        await _facilityRepository.InsertAsync(facility);
    }

    public async Task<List<Contract>> GetFacilityContractsAsync(int facilityId)
    {
        return await _contractRepository.Table.Where(c => c.FacilityId == facilityId).ToListAsync();
    }
    
    public async Task<List<DiscountRequirement>> GetFacilityDiscountsAsync(int facilityId)
    {
        return await _discountRepository.Table.Where(dr => dr.FacilityId == facilityId).ToListAsync();
    }

    public async Task DeleteFacilityAsync(int facilityId)
    {
        var facility = await _facilityRepository.GetByIdAsync(facilityId);
        if (facility != null) 
            await _facilityRepository.DeleteAsync(facility);
    }

    public async Task UpdateFacilityAsync(CreateOrUpdateFacilityModel model)
    {
        var facility = await _facilityRepository.GetByIdAsync(model.Id);

        if (facility != null)
        {
            facility.Name = model.FacilityName;
            facility.StateName = model.StateName;
            facility.AreaName = model.AreaName;
            facility.CityName = model.CityName;
            facility.Address = model.FacilityAddress;
            facility.HouseNumber = model.HouseNumber;
            facility.EnclosureNumber = model.EnclosureNumber;
            facility.BuildingNumber = model.BuildingNumber;
            facility.HourRate = model.HourPrice;
            facility.ActiveContractId = model.ActiveContractId;
            
            await _facilityRepository.UpdateAsync(facility);
        }
    }

    public async Task AddOrUpdateFacilityContractAsync(ContractModel model)
    {
        var contract = await _contractRepository.GetByIdAsync(model.Id);
        if (contract==null)
        {
            contract = new Contract
            {
                FacilityId = model.FacilityId,
                Number = model.ContractNumber,
                StartDate = model.StartDate
            };
            await _contractRepository.InsertAsync(contract);
        }
        else
        {
            contract.Number = model.ContractNumber;
            contract.StartDate = model.StartDate;
        
            await _contractRepository.UpdateAsync(contract);
        }
        
        var facility = await _facilityRepository.GetByIdAsync(model.FacilityId);
        if (facility != null)
        {
            facility.ActiveContractId=contract.Id;
            await _facilityRepository.UpdateAsync(facility);
        }
    }

    public async Task AddFacilityDiscountAsync(DiscountRequirementModel model)
    {
        await _discountRepository.InsertAsync(new DiscountRequirement
        {
            FacilityId = model.FacilityId,
            SuppliesRate = model.SuppliesRate,
            UninstallRate = model.UninstallRate,
            InstallRate = model.InstallRate,
            StartRange = model.StartRange,
            EndRange = model.EndRange
        });
    }

    public async Task DeleteFacilityDiscountAsync(int id)
    {
        var facilityDiscount = await _discountRepository.GetByIdAsync(id);
        if (facilityDiscount != null) 
            await _discountRepository.DeleteAsync(facilityDiscount);
    }

    public async Task<Contract?> GetContractByIdAsync(int contractId)
    {
        return await _contractRepository.GetByIdAsync(contractId);
    }

    public async Task<List<Facility>> GetFacilityListAsync()
    {
        return await _facilityRepository.Table.ToListAsync();
    }

    public async Task<string> GetFacilityNameByIdAsync(int facilityId)
    {
        var facility = await _facilityRepository.GetByIdAsync(facilityId);
        return facility!=null?facility.Name:String.Empty;
    }
}