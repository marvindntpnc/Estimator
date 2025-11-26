using System.Reflection;
using Estimator.Models.Shared;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;

namespace Estimator.Infrastructure.DataTables;

public class DataTablesSearchModelBinder: IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        if (bindingContext == null)
        {
            throw new ArgumentNullException(nameof(bindingContext));
        }

        var modelType = bindingContext.ModelType;
        if (!typeof(BaseSearchModel).IsAssignableFrom(modelType))
        {
            return Task.CompletedTask;
        }

        // Create instance of the specific model type (descendant of BaseSearchModel)
        var model = Activator.CreateInstance(modelType);
        if (model == null)
        {
            return Task.CompletedTask;
        }

        var form = bindingContext.HttpContext.Request.HasFormContentType
            ? bindingContext.HttpContext.Request.Form
            : default;

        // Helper to read value from either Form or Query
        string Read(string key)
        {
            if (form != default && form.TryGetValue(key, out StringValues formValue) && formValue.Count > 0)
            {
                return formValue[0];
            }
            var query = bindingContext.HttpContext.Request.Query;
            if (query.TryGetValue(key, out var queryValue) && queryValue.Count > 0)
            {
                return queryValue[0];
            }
            return null;
        }
        
        // Helper to check if parameter exists in request (for boolean values)
        bool Exists(string key)
        {
            if (form != default && form.ContainsKey(key))
            {
                return true;
            }
            return bindingContext.HttpContext.Request.Query.ContainsKey(key);
        }

        // Bind standard properties via fallback first (so custom props can override)
        foreach (var property in modelType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (!property.CanWrite) continue;

            var val = Read(property.Name);
            
            // Special handling for bool values - check if parameter exists in request
            if (property.PropertyType == typeof(bool))
            {
                if (!Exists(property.Name))
                {
                    // Parameter doesn't exist - set default to false
                    property.SetValue(model, false);
                    continue;
                }
                // Parameter exists in request but value is null - this happens when checkbox is not checked
                if (val == null)
                {
                    property.SetValue(model, false);
                    continue;
                }
            }
            
            // Special handling for DateTime? (nullable) - set null if value is null or empty
            if (property.PropertyType == typeof(DateTime?))
            {
                if (string.IsNullOrWhiteSpace(val))
                {
                    property.SetValue(model, null);
                    continue;
                }
            }
            
            if (val == null) continue;
            try
            {
                var converted = ConvertTo(val, property.PropertyType);
                property.SetValue(model, converted);
            }
            catch
            {
                // ignore invalid value
            }
        }

        // DataTables-specific
        int draw = TryParseInt(Read("draw"));
        int start = TryParseInt(Read("start"));
        int length = TryParseInt(Read("length"));
        string searchValue = Read("search[value]");

        int? orderColumnIndex = null;
        string orderDir = null;
        string orderColumnData = null;

        // Only first order level as per requirements
        var orderIdxStr = Read("order[0][column]");
        if (int.TryParse(orderIdxStr, out var oc))
        {
            orderColumnIndex = oc;
            orderDir = Read("order[0][dir]");
            // Resolve to columns[oc][data]
            var dataKey = $"columns[{oc}][data]";
            orderColumnData = Read(dataKey);
        }

        // Set properties on BaseSearchModel
        SetProp(model, nameof(BaseSearchModel.Draw), draw);
        SetProp(model, nameof(BaseSearchModel.Start), start);
        SetProp(model, nameof(BaseSearchModel.Length), length > 0 ? length : GetIntProp(model, nameof(BaseSearchModel.PageSize), 25));
        SetProp(model, nameof(BaseSearchModel.SearchValue), searchValue);
        SetProp(model, nameof(BaseSearchModel.OrderColumnIndex), orderColumnIndex);
        SetProp(model, nameof(BaseSearchModel.OrderColumnData), orderColumnData);
        SetProp(model, nameof(BaseSearchModel.OrderDirection), orderDir);

        // Derive paging into PageIndex/PageSize for backward compatibility
        var effectiveLength = GetIntProp(model, nameof(BaseSearchModel.Length), 25);
        var pageIndex = effectiveLength > 0 ? start / effectiveLength : 0;
        SetProp(model, nameof(BaseSearchModel.PageIndex), pageIndex);
        SetProp(model, nameof(BaseSearchModel.PageSize), effectiveLength > 0 ? effectiveLength : 25);

        bindingContext.Result = ModelBindingResult.Success(model);
        return Task.CompletedTask;
    }

    private static object ConvertTo(string value, Type targetType)
    {
        if (targetType == typeof(string)) return value;
        if (targetType == typeof(int) || targetType == typeof(int?))
        {
            if (int.TryParse(value, out var i)) return i;
            return targetType == typeof(int?) ? null : 0;
        }
        if (targetType == typeof(bool) || targetType == typeof(bool?))
        {
            if (bool.TryParse(value, out var b)) return b;
            return targetType == typeof(bool?) ? null : false;
        }
        if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
        {
            // Try to parse dates in format dd.MM.yyyy (used by AirDatepicker)
            if (System.Text.RegularExpressions.Regex.IsMatch(value, @"^\d{2}\.\d{2}\.\d{4}$"))
            {
                var parts = value.Split('.');
                if (parts.Length == 3 && int.TryParse(parts[0], out var day) && 
                    int.TryParse(parts[1], out var month) && int.TryParse(parts[2], out var year))
                {
                    try
                    {
                        return new DateTime(year, month, day);
                    }
                    catch
                    {
                        // Invalid date
                    }
                }
            }
            
            // Try standard DateTime parsing as fallback
            if (DateTime.TryParse(value, out var date))
            {
                return date;
            }
            
            return targetType == typeof(DateTime?) ? null : (object)DateTime.MinValue;
        }
        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, value, true);
        }
        return Convert.ChangeType(value, targetType);
    }

    private static int TryParseInt(string value)
    {
        return int.TryParse(value, out var i) ? i : 0;
    }

    private static void SetProp(object model, string name, object value)
    {
        var prop = model.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        if (prop != null && prop.CanWrite)
        {
            prop.SetValue(model, value);
        }
    }

    private static int GetIntProp(object model, string name, int defaultValue)
    {
        var prop = model.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
        if (prop != null)
        {
            var val = prop.GetValue(model);
            if (val is int i) return i;
        }
        return defaultValue;
    }
}