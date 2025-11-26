using ClosedXML.Excel;
using Estimator.Domain;

namespace Estimator.Inerfaces;

public interface IExcelService
{
    /// <summary>
    /// Forms Excel workbook for estimate including header, logo and detailed items.
    /// </summary>
    /// <param name="estimate">Estimate entity with loaded items and currency rates.</param>
    /// <returns>Ready to save or send Excel workbook.</returns>
    public Task<XLWorkbook> FormEstimateExcelBookAsync(Estimate estimate);
}