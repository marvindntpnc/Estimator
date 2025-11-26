using System.Globalization;

namespace Estimator.Models.Estimate;

public class EstimateModel
{
    public int Id { get; set; }
    public string EstimateNumber { get; set; }
    public string Name { get; set; }
    public string FacilityName { get; set; }
    public string ClosedAt { get; set; }
    public decimal EstimateTotal { get; set; }
    public string EstimateTotalString => EstimateTotal.ToString("C",new CultureInfo("ru-RU"));
    public string CreatedAt { get; set; }
}