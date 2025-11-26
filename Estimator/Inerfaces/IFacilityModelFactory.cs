using Estimator.Models.Facility;
using PagedList;

namespace Estimator.Inerfaces;

public interface IFacilityModelFactory
{
    Task<IPagedList<FacilityModel>> PrepareFacilityPagedListAsync(FacilitySearchModel searchModel);
    Task<CreateOrUpdateFacilityModel>PrepareCreateOrEditFacilityModelAsync(int facilityId);
}