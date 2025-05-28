using BookStore.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BookStore.Domain.Models;
using BookStore.Web.Models;

namespace BookStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRegionProvider _regionProvider;

        public HomeController(IRegionProvider regionProvider)
        {
            _regionProvider = regionProvider;
        }

        public IActionResult Index()
        {
            var model = new HomeViewModel
            {
                Regions = _regionProvider.GetAllRegions().ToList(),
                SelectedRegion = Region.DefaultRegion(),
                UserSeed = new Random().Next(1, 100000),
                AverageLikes = 5.0m,
                AverageReviews = 3.0m
            };

            return View(model);
        }
    }

}
