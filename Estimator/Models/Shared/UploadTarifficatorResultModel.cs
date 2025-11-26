using Estimator.Domain;
using Estimator.Services;

namespace Estimator.Models.Shared;

public class UploadTarifficatorResultModel
{
    public CompareResult<TarifficatorItem> ActionsInfo{ get; set; }
    public List<DuplicateGroup<TarifficatorItem>> DuplicatesInfo { get; set; }
}