namespace Estimator.Models.Shared.DataTables;

public class DataTableModel
{
    public string Name { get; set; }
    public string Url { get; set; }
    public int Length { get; set; } = 10;
    public List<ColumnDefinition> Columns { get; set; } = new List<ColumnDefinition>();
    public List<ButtonDefinition> Buttons { get; set; } = new List<ButtonDefinition>();
    public string Dom { get; set; } = "<'row'<'col-md-6'l><'col-md-6'f>>" +
                                      "<'row'<'col-md-12'tr>>" +
                                      "<'row'<'col-md-5'i><'col-md-7'p>>";
    public bool ServerSide { get; set; } = true;
    public bool Processing { get; set; } = true;
    public bool Ordering { get; set; } = true;
    public bool Searching { get; set; } = true;
    public List<int> Order { get; set; } = new List<int> { 0 };
    public Dictionary<string, object> AdditionalParameters { get; set; } = new Dictionary<string, object>();
    public List<string> SearchFieldNames { get; set; } = new List<string>();
    public string SearchButtonId { get; set; } = string.Empty;
}