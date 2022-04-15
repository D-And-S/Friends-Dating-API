namespace Friends_Date_API.Helpers
{
    // this class is for default and maximum pageSize
    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1; // page Number will come form user.. default value 1

        private int _pageSize = 10; // default page size

        public int PageSize
        {
            get => _pageSize;

            //if user specify page-size than maximum page size that maximum will be 50
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string CurrentUsername { get; set; }
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 150;
        public string OrderBy { get; set; } = "lastActive";
    }
}
