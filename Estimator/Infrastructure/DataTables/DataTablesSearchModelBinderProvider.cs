using Estimator.Models.Shared;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Estimator.Infrastructure.DataTables;

public class DataTablesSearchModelBinderProvider: IModelBinderProvider
{
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (typeof(BaseSearchModel).IsAssignableFrom(context.Metadata.ModelType))
        {
            return new DataTablesSearchModelBinder();
        }

        return null;
    }
}