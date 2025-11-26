using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using PagedList;

namespace Estimator.Services;

public static class AsyncIQueryableExtensions
{
    public static async Task<IPagedList<T>> ToPagedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize,
        bool getOnlyTotalCount = false)
    {
        if (source == null)
            return new PagedList<T>(new List<T>(), pageNumber-1, pageSize);

        //min allowed page size is 1
        pageSize = Math.Max(pageSize, 1);

        var count = await source.CountAsync();

        var data = new List<T>();
        int skip = (pageNumber - 1) * pageSize;

        if (!getOnlyTotalCount)
            data.AddRange(await source.Skip(skip).Take(pageSize).ToListAsync());

        return new PagedList<T>(data, pageNumber, pageSize);
    }
}