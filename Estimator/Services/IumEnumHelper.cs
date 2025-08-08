using Estimator.Domain.Enums;

namespace Estimator.Services;

public class IumEnumHelper
{
    public static MeasureType ConvertMeasureTypeString(string measureType)
    {
        switch (measureType)
        {
            case "бал":
                return MeasureType.Balloon;
            case "кг.":
                return MeasureType.Kg;
            case "компл":
                return MeasureType.Kit;
            case "л.":
                return MeasureType.Liter;
            case "м.п.":
                return MeasureType.Meter;
            case "м2":
                return MeasureType.SquareMeter;
            case "м3":
                return MeasureType.CubicMeter;
            case "см.":
                return MeasureType.Centimetre;
            case "коэфф":
                return MeasureType.Rate;
            case "тн.":
                return MeasureType.Tone;
            case "н/ч":
                return MeasureType.Hours;
            default:
                return MeasureType.Piece;
        }
    }
    public static CurrencyType ConvertCurrencyTypeString(string currencyType)
    {
        switch (currencyType)
        {
            case "CNY":
                return CurrencyType.CNY;
            case "EUR":
                return CurrencyType.EUR;
            case "USD":
                return CurrencyType.USD;
            default :
                return CurrencyType.RUB;
        }
    }
}