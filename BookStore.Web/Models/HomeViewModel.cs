using BookStore.Domain.Models;

namespace BookStore.Web.Models
{
    public class HomeViewModel
    {
        public List<Region> Regions { get; set; } = new();
        public Region SelectedRegion { get; set; }
        public int UserSeed { get; set; }
        public decimal AverageLikes { get; set; }
        public decimal AverageReviews { get; set; }
    }
}
