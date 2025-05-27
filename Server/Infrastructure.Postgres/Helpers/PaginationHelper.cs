using Application.Models.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Postgres.Helpers;

public static class PaginationHelper
{
    public static async Task<PagedResult<TDto>> PaginateAsync<TEntity, TDto>(
        IQueryable<TEntity> query,
        int pageNumber,
        int pageSize,
        Func<TEntity, TDto> mapper)
    {
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        if (pageNumber > totalPages)
            pageNumber = totalPages == 0 ? 1 : totalPages;

        var pagedEntities = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var mappedItems = pagedEntities.Select(mapper).ToList();

        return new PagedResult<TDto>
        {
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Items = mappedItems
        };
    }
}