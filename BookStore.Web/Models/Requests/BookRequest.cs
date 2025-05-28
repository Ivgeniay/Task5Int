namespace BookStore.Web.Models.Requests
{
    public class BookRequest
    {
        public string RegionCode { get; set; } = string.Empty;
        public int UserSeed { get; set; }
        public decimal AverageLikes { get; set; }
        public decimal AverageReviews { get; set; }
        public int PageNumber { get; set; }
    }
}
