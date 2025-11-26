namespace Estimator.Models.Facility;

public class ContractModel
{
    public int Id { get; set; }
    public int FacilityId { get; set; }
    public string ContractNumber { get; set; }
    public DateTime StartDate { get; set; }
}