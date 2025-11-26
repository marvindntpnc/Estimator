namespace Estimator.Models.Shared.DataTables;

public class ColumnDefinition
{
    public string Data { get; set; }
    public string Title { get; set; }
    public bool Orderable { get; set; } = true;
    public bool Searchable { get; set; } = true;
    public string Width { get; set; }
    public string ClassName { get; set; }
    public string Render { get; set; }
}