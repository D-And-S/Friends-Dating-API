namespace Friends_Date_API.Helpers
{

    // this class is for when we get data for pagination
    // we want send header along with data 
    public class PaginationHeader
    {
        public PaginationHeader(int currentPage, int itemPerPage, int totalItems, int totalPages)
        {
            CurrentPage = currentPage;
            ItemPerPage = itemPerPage;
            TotalItems = totalItems;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; set; }
        public int ItemPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
