namespace Estimator.Models.Facility;

public class FacilityModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string AddressString { get; set; }
    public decimal HourPrice { get; set; }
    public List<ContractModel> ContractList { get; set; }
}