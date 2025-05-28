using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;

namespace BookStore.Application.Services
{
    public class RegionProvider : IRegionProvider
    {
        public IEnumerable<Region> GetAllRegions() => Region.GetAllRegions();

        public Region? GetRegionByCode(string code)
        {
            if (string.IsNullOrWhiteSpace(code))
                return null;

            return GetAllRegions()
                .FirstOrDefault(r => string.Compare(r.Code, code, StringComparison.OrdinalIgnoreCase) == 0);
        }

        public bool IsValidRegion(Region region)
        {
            return GetAllRegions().Contains(region);
        }
    }
}
