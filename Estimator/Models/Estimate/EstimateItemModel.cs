using System.Text.Json.Serialization;
using Estimator.Services;

namespace Estimator.Models.Estimate;

public class EstimateItemModel
{
    public int Id { get; set; }
    public int EstimateId { get; set; }
    public decimal Qty { get; set; }
    public decimal CustomRate { get; set; }
    public decimal Total { get; set; }
    public string TotalString => Helper.FormatPrice(Total);

    [JsonIgnore]
    public TarrificatorItemModel TarifficatorItem { get; set; }
    public decimal RublePrice { get; set; }
    public string RublePriceString => Helper.FormatPrice(RublePrice);
    public string ItemCode => TarifficatorItem?.ItemCode ?? "";
    public string CategoryName => TarifficatorItem?.CategoryName ?? "";
    public string SubCategoryName => TarifficatorItem?.SubCategoryName ?? "";
    public string Name => TarifficatorItem?.Name ?? "";
    public string Description => TarifficatorItem?.Description ?? "";
    public string TarificatorItemTypeString => TarifficatorItem?.TarificatorItemTypeString ?? "";
}