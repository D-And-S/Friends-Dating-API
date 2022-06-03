using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Friends_Date_API.Helpers
{
    // To Create Pagination
    // Here T is type for our entities or Dto
    // the type of our PageList will be List that's  wy we will inherit List
    public class PageList<T> : List<T>
    {
        // list of items that we get from our query
        // count is for number of item
        //
        public PageList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            CurrentPage = pageNumber;

            //total pages will be like.. 
            /*
                1. jodi amader 10 ta data fetch kori 
                2. protita page e 5 ta kore data thakbe
                3. sekhtre total page hobe 10 / 5 = 2 
                4. jodi 9 ta data thake tahole 9 
                5. tahole protita page e amra 4.5 kore data rakhbo. kintu 4.5 data hoy nah
                6. ajonno amra celing method use korechi and setake cast korechi
                7. jeta akta page 5 ta data and arekta page e 4 ta data show korbe
             */

            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            PageSize = pageSize;
            TotalCount = count;
            AddRange(items);
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static async Task<PageList<T>> CreateAsync(IQueryable<T> source, int pageNaumber, int pageSize)
        {
            var count = await source.CountAsync();

            /*
              1. jodi amader 20 ta record thake 
              2. jodi page number 0 (means no record). 0-1 * 5 = -5
              3. akhon amra page size 5 diyechi ist page e 5 ta data 
              4. jokhon amra second page e jabe i mean 2 - 1 * 5 = 5 means 5 ta data skip kora      
             */
            var items = await source.Skip((pageNaumber -1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

            return new PageList<T>(items, count, pageNaumber, pageSize);
        }

    }
}
