using Estimator.Models.Shared.DataTables;
using Microsoft.AspNetCore.Mvc;

namespace Estimator.Components;

public class DataTableViewComponent: ViewComponent
{
    public IViewComponentResult Invoke(DataTableModel model)
    {
        return View("~/Views/Shared/Components/DataTables/Default.cshtml",model);
    }
}