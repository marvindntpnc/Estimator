namespace Estimator.Models.Facility;

public class DiscountRequirementModel
{
    public int Id { get; set; }
    public int FacilityId { get; set; }
    public decimal StartRange { get; set; }
    public decimal EndRange { get; set; }
    public decimal UninstallRate { get; set; }
    public decimal InstallRate { get; set; }
    public decimal SuppliesRate { get; set; }
}