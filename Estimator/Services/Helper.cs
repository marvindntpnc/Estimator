using System.Globalization;

namespace Estimator.Services;

public static class Helper
{
    public static string FormatPrice(decimal price)
    {
        return price.ToString(CultureInfo.InvariantCulture).Replace(".", ",");
    }
}