using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Estimator.Infrastructure;

public class DataTablesResultFilter: IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is JsonResult jsonResult)
        {
            var request = context.HttpContext.Request;
            // Биндим только нужные поля из form-urlencoded
            int draw = int.TryParse(request.Form["draw"], out var d) ? d : 0;
            int start = int.TryParse(request.Form["start"], out var s) ? s : 0;
            int length = int.TryParse(request.Form["length"], out var l) ? l : 10;
            string searchValue = request.Form["search[value]"]; // для совместимости, не используем для фильтрации тут

            // Ожидаем, что action вернул List<T>
            if (jsonResult.Value is System.Collections.IEnumerable enumerable && jsonResult.Value is not string)
            {
                var list = enumerable.Cast<object>().ToList();
                var recordsTotal = list.Count;
                var paged = list.Skip(start).Take(length).ToList();

                context.Result = new JsonResult(new
                {
                    draw,
                    recordsTotal,
                    recordsFiltered = recordsTotal,
                    data = paged
                })
                {
                    StatusCode = 200
                };
            }
        }

        await next();
    }
}