using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers;

public static class Pagination
{
    public static async Task<PaginationResult<T>> CreateAsync<T>(IQueryable<T> query, int pageNumber, int pageSize)
    {
        var count = await query.CountAsync();
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

        return new PaginationResult<T>
        {
            Metadata = new PaginationMetadata
            {
                CurrentPage = pageNumber,
                TotalPages = (int)Math.Ceiling(count / (double)pageSize),
                PageSize = pageSize,
                TotalCount = count
            },
            Items = items
        };
    }

    internal static async Task<PaginationResult<Member>> CreateAsync(object query, int pageNumber, int pageSize)
    {
        throw new NotImplementedException();
    }
}