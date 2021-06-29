using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace XiechengAPI.Helper
{
    public class PaginationList<T> :List<T>
    {


        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public PaginationList(int currentPage, int pageSize,List<T> items)
        {
            CurrentPage = currentPage;
            PageSize = pageSize;
            AddRange(items);
        }

        public static async Task<PaginationList<T>> CreateAsync(int currentPage,int pageSize,IQueryable<T> result)
        {
            // pagination
            // skip
            var skip = (currentPage - 1) * pageSize;
            result = result.Skip(skip);
            // Display a certain amount of data in page-size
            result = result.Take(pageSize); 
            var item = await result.ToListAsync();
            return new PaginationList<T>(currentPage, pageSize, item);
        }
    }
}
