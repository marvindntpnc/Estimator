using System.Reflection;

namespace Estimator.Services;

public class ListCompareHelper
{
    private static readonly Dictionary<Type, PropertyInfo[]> PropertyCache = new Dictionary<Type, PropertyInfo[]>();
    private static readonly Dictionary<Type, Func<object, object>> IdGetterCache = new Dictionary<Type, Func<object, object>>();

    public static (CompareResult<TarifficatorItem>,List<DuplicateGroup<TarifficatorItem>>) Compare<TarifficatorItem>(IList<TarifficatorItem> oldList, IList<TarifficatorItem> newList)
    {
        var result = new CompareResult<TarifficatorItem>();
        
        // Нормализация цен: округление до 2 знаков для корректного сравнения
        NormalizePrices(oldList);
        NormalizePrices(newList);

        // Быстрое создание словарей для поиска по ID
        var oldDict = CreateDictionary(oldList, out var oldDuplicates);
        var newDict = CreateDictionary(newList, out var newDuplicates);

        // Поиск удаленных элементов
        foreach (var oldId in oldDict.Keys)
        {
            if (!newDict.ContainsKey(oldId))
            {
                result.Removed.Add(oldDict[oldId]);
            }
        }

        // Поиск добавленных и измененных элементов
        foreach (var newId in newDict.Keys)
        {
            if (!oldDict.ContainsKey(newId))
            {
                result.Added.Add(newDict[newId]);
            }
            else
            {
                var oldItem = oldDict[newId];
                var newItem = newDict[newId];
                
                var changedProperties = FindChangedProperties(oldItem, newItem);
                if (changedProperties.Any())
                {
                    result.Changed.Add(new ValueChangedItem<TarifficatorItem>
                    {
                        OldValue = oldItem,
                        NewValue = newItem,
                        ChangedProperties = changedProperties
                    });
                }
            }
        }

        return (result,newDuplicates);
    }

    private static void NormalizePrices<TarifficatorItem>(IEnumerable<TarifficatorItem> list)
    {
        var type = typeof(TarifficatorItem);
        var priceProp = type.GetProperty("Price", BindingFlags.Public | BindingFlags.Instance);
        if (priceProp == null) return;
        if (priceProp.PropertyType != typeof(decimal)) return;

        foreach (var item in list)
        {
            var value = (decimal)(priceProp.GetValue(item) ?? 0m);
            var rounded = Math.Round(value, 2, MidpointRounding.AwayFromZero);
            if (rounded != value)
            {
                priceProp.SetValue(item, rounded);
            }
        }
    }

    private static Dictionary<object, TarifficatorItem> CreateDictionary<TarifficatorItem>(IList<TarifficatorItem> list, out List<DuplicateGroup<TarifficatorItem>> duplicates)
    {
        var dict = new Dictionary<object, TarifficatorItem>();
        var seen = new Dictionary<object, List<TarifficatorItem>>();

        foreach (var item in list)
        {
            var key = GetCompositeKey(item);
            dict[key] = item;
            if (!seen.TryGetValue(key, out var bucket))
            {
                bucket = new List<TarifficatorItem>();
                seen[key] = bucket;
            }
            bucket.Add(item);
        }

        duplicates = seen.Where(k => k.Value.Count > 1)
            .Select(k => new DuplicateGroup<TarifficatorItem> { Key = k.Key, Items = k.Value })
            .ToList();

        return dict;
    }

    private static Func<TarifficatorItem, object> GetIdGetter<TarifficatorItem>()
    {
        // Больше не используем одинарный ключ по Name. Оставлено для совместимости вызовов.
        return item => GetCompositeKey(item);
    }

    private static object GetCompositeKey<TarifficatorItem>(TarifficatorItem item)
    {
        var type = typeof(TarifficatorItem);
        var nameProp = type.GetProperty("Name");
        var codeProp = type.GetProperty("ItemCode");

        var name = (nameProp?.GetValue(item) as string ?? string.Empty).Trim().ToLowerInvariant();
        var code = (codeProp?.GetValue(item) as string ?? string.Empty).Trim().ToLowerInvariant();

        return $"{name}|{code}";
    }

    private static Func<TarifficatorItem, object> CreatePropertyGetter<TarifficatorItem>(PropertyInfo property)
    {
        // Компилируем выражение для быстрого доступа к свойству
        var parameter = System.Linq.Expressions.Expression.Parameter(typeof(TarifficatorItem), "obj");
        var propertyAccess = System.Linq.Expressions.Expression.Property(parameter, property);
        var convert = System.Linq.Expressions.Expression.Convert(propertyAccess, typeof(object));
        var lambda = System.Linq.Expressions.Expression.Lambda<Func<TarifficatorItem, object>>(convert, parameter);
        return lambda.Compile();
    }

    private static PropertyInfo[] GetCachedProperties<TarifficatorItem>()
    {
        var type = typeof(TarifficatorItem);
        
        if (!PropertyCache.TryGetValue(type, out var properties))
        {
            // Игнорируем поле Id при сравнении
            properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.Name != "Id" && p.Name != "ID")
                .ToArray();
            
            PropertyCache[type] = properties;
        }
        
        return properties;
    }

    private static List<string> FindChangedProperties<TarifficatorItem>(TarifficatorItem oldItem, TarifficatorItem newItem)
    {
        var changedProperties = new List<string>();
        var properties = GetCachedProperties<TarifficatorItem>();
        
        foreach (var property in properties)
        {
            var oldValue = property.GetValue(oldItem);
            var newValue = property.GetValue(newItem);
            
            if (!AreEqual(oldValue, newValue))
            {
                changedProperties.Add(property.Name);
            }
        }
        
        return changedProperties;
    }

    private static bool AreEqual(object a, object b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        
        // Для значений и строк используем стандартное сравнение
        if (a.GetType().IsValueType || a is string)
        {
            return a.Equals(b);
        }
        
        // Для сложных объектов можно добавить рекурсивное сравнение,
        // но это может быть медленно для больших структур
        return a.Equals(b);
    }
}

public class CompareResult<TarifficatorItem>
{
    public List<TarifficatorItem> Added { get; set; } = new List<TarifficatorItem>();
    public List<TarifficatorItem> Removed { get; set; } = new List<TarifficatorItem>();
    public List<ValueChangedItem<TarifficatorItem>> Changed { get; set; } = new List<ValueChangedItem<TarifficatorItem>>();
}

public class ValueChangedItem<TarifficatorItem>
{
    public TarifficatorItem OldValue { get; set; }
    public TarifficatorItem NewValue { get; set; }
    public List<string> ChangedProperties { get; set; } = new List<string>();
}

public class DuplicateGroup<TarifficatorItem>
{
    public object Key { get; set; }
    public List<TarifficatorItem> Items { get; set; } = new List<TarifficatorItem>();

}