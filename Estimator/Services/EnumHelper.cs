using Estimator.Domain.Enums;

namespace Estimator.Services;

public static class EnumHelper
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
    public static string ConvertMeasureTypeToString(MeasureType measureType)
    {
        switch (measureType)
        {
            case MeasureType.Balloon:
                return "бал";
            case MeasureType.Kg:
                return "кг.";
            case MeasureType.Kit:
                return "компл";
            case MeasureType.Liter:
                return "л.";
            case MeasureType.Meter:
                return "м.п.";
            case MeasureType.SquareMeter:
                return "м2";
            case MeasureType.CubicMeter:
                return "м3";
            case MeasureType.Centimetre:
                return "см.";
            case MeasureType.Rate:
                return "коэфф";
            case MeasureType.Tone:
                return "тн.";
            case MeasureType.Hours:
                return "н/ч";
            default:
                return "шт.";
        }
    }
    
    public static string ConvertCurrencyTypeToString(CurrencyType currencyType)
    {
        switch (currencyType)
        {
            case CurrencyType.CNY:
                return "CNY";
            case CurrencyType.EUR:
                return "EUR";
            case CurrencyType.USD:
                return "USD";
            default :
                return "RUB";
        }
    }

    public static string ConvertTarifficatorItemTypeToString(TarificatorItemType tarificatorItemType)
    {
        switch (tarificatorItemType)
        {
            case TarificatorItemType.Material:
                return "Материал";
            case TarificatorItemType.Service:
                return "Услуга";
            default:
                return "";
                
        }
    }
}