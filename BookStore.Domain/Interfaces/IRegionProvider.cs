using BookStore.Domain.Models;

namespace BookStore.Domain.Interfaces
{
    public interface IRegionProvider
    {
        IEnumerable<Region> GetAllRegions();
        Region? GetRegionByCode(string code);
        bool IsValidRegion(Region region);
    }
}
