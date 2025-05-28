namespace BookStore.Web.Models.Requests
{
    public class ExportRequest
    {
        public string RegionCode { get; set; } = string.Empty;
        public int UserSeed { get; set; }
        public decimal AverageLikes { get; set; }
        public decimal AverageReviews { get; set; }
        public int PageCount { get; set; }
    }
}
