namespace BookStore.Domain.Models
{
    public class GenerationParameters
    {
        public Region Region { get; set; } = Region.DefaultRegion();
        public int UserSeed { get; set; }
        public decimal AverageLikes { get; set; }
        public decimal AverageReviews { get; set; }
        public int PageNumber { get; set; } = 1;
    }
}
