using Estimator.Domain;
using Estimator.Models.Facility;
using PagedList;

namespace Estimator.Inerfaces;

public interface IFacilityService
{
    /// <summary>
    /// Returns paged list of facilities according to search filters.
    /// </summary>
    public Task<IPagedList<Facility?>> GetFacilityPagedListAsync(FacilitySearchModel searchModel);
    
    /// <summary>
    /// Gets facility by identifier.
    /// </summary>
    public Task<Facility?> GetFacilityByIdAsync(int facilityId);
    
    /// <summary>
    /// Creates new facility.
    /// </summary>
    public Task CreateFacilityAsync(Facility facility);
    
    /// <summary>
    /// Returns list of contracts configured for facility.
    /// </summary>
    public Task<List<Contract>> GetFacilityContractsAsync(int facilityId);
    
    /// <summary>
    /// Returns list of discount rules configured for facility.
    /// </summary>
    public Task<List<DiscountRequirement>>GetFacilityDiscountsAsync(int facilityId);
    
    /// <summary>
    /// Deletes facility by identifier.
    /// </summary>
    public Task DeleteFacilityAsync(int facilityId);
    
    /// <summary>
    /// Updates facility data according to edit model.
    /// </summary>
    public Task UpdateFacilityAsync(CreateOrUpdateFacilityModel model);
    
    /// <summary>
    /// Creates new contract or updates existing contract for facility.
    /// Also updates active contract identifier on facility.
    /// </summary>
    public Task AddOrUpdateFacilityContractAsync(ContractModel model);
    
    /// <summary>
    /// Adds new discount rule to facility.
    /// </summary>
    public Task AddFacilityDiscountAsync(DiscountRequirementModel model);
    
    /// <summary>
    /// Deletes discount rule by identifier.
    /// </summary>
    public Task DeleteFacilityDiscountAsync(int id);
    
    /// <summary>
    /// Gets contract by identifier.
    /// </summary>
    public Task<Contract?> GetContractByIdAsync(int contractId);
    
    /// <summary>
    /// Returns list of all facilities.
    /// </summary>
    public Task<List<Facility>> GetFacilityListAsync();
    
    /// <summary>
    /// Returns facility name by identifier.
    /// </summary>
    public Task<string> GetFacilityNameByIdAsync(int facilityId);
}