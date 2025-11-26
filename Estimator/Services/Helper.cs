using System.Globalization;

namespace Estimator.Services;

public static class Helper
{
    public static string FormatPrice(decimal price)
    {
        var result = price.ToString("C",new CultureInfo("ru-RU"));
        return $"{result}";
    }
}