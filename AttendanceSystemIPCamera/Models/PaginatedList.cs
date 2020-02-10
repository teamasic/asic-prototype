using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace AttendanceSystemIPCamera.Models
{
    public class PaginatedList<T>: List<T>
    {
        public int Page { get; set; }
        public int TotalPage { get; set; }
        public int Total { get; set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            Page = pageIndex;
            TotalPage = (int)Math.Ceiling(count / (double)pageSize);
            Total = count;
            this.AddRange(items);
        }

        public bool HasPrevPage => Page > 1;
        public bool HasNextPage => Page < TotalPage;
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
