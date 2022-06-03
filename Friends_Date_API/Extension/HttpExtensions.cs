using Friends_Date_API.Helpers;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Friends_Date_API.Extension
{
    public static class HttpExtensions
    {
        public static void AddPaginationHeader(this HttpResponse response, int currentPage, 
            int itemsPerPage, int totalItems, int totalPages)
        {
            var paginationHeader = new PaginationHeader(currentPage,itemsPerPage, totalItems, totalPages);

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            //custom header 
            response.Headers.Add("Pagination", JsonSerializer.Serialize(paginationHeader,options));

            // we need to ad cors header to make the custom header available
            response.Headers.Add("Access-Control-Expose-Headers", "Pagination");

        }
    }
}
