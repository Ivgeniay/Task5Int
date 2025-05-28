using BookStore.Infrastructure.Interfaces;
using BookStore.Domain.Interfaces;
using BookStore.Domain.Models;
using BookStore.Web.Models.Requests;
using Microsoft.AspNetCore.Mvc;
using BookStore.Application;

namespace BookStore.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly IBookGenerationService _bookGenerationService;
        private readonly IRegionProvider _regionProvider;
        private readonly ICsvExportService _csvExportService;

        public ApiController(
            IBookGenerationService bookGenerationService,
            IRegionProvider regionProvider,
            ICsvExportService csvExportService)
        {
            _bookGenerationService = bookGenerationService;
            _regionProvider = regionProvider;
            _csvExportService = csvExportService;
        }

        [HttpPost("books")]
        public IActionResult GetBooks([FromBody] BookRequest request)
        {
            if (string.Compare(request.RegionCode, AppConstants.UnknownAuthor) == 0)
                return BadRequest("Invalid region code");
            
            Region? region = _regionProvider.GetRegionByCode(request.RegionCode);

            if (region == null)
                return BadRequest("Invalid region code");

            var parameters = new GenerationParameters
            {
                Region = region.Value,
                UserSeed = request.UserSeed,
                AverageLikes = request.AverageLikes,
                AverageReviews = request.AverageReviews,
                PageNumber = request.PageNumber
            };

            var result = _bookGenerationService.GenerateBooks(parameters);
            return Ok(result);
        }

        [HttpGet("book/{index:int}")]
        public IActionResult GetBookDetails(int index, [FromQuery] BookDetailsRequest request)
        {
            var region = _regionProvider.GetRegionByCode(request.RegionCode);
            if (region == null)
            {
                return BadRequest("Invalid region code");
            }

            var parameters = new GenerationParameters
            {
                Region = region.Value,
                UserSeed = request.UserSeed,
                AverageLikes = request.AverageLikes,
                AverageReviews = request.AverageReviews
            };

            var bookDetails = _bookGenerationService.GenerateBookDetails(index, parameters);
            return Ok(bookDetails);
        }

        [HttpPost("export")]
        public IActionResult ExportToCsv([FromBody] ExportRequest request)
        {
            var region = _regionProvider.GetRegionByCode(request.RegionCode);
            if (region == null)
                return BadRequest("Invalid region code");
            

            var allBooks = new List<Book>();
            var parameters = new GenerationParameters
            {
                Region = region.Value,
                UserSeed = request.UserSeed,
                AverageLikes = request.AverageLikes,
                AverageReviews = request.AverageReviews
            };

            for (int page = 1; page <= request.PageCount; page++)
            {
                parameters.PageNumber = page;
                var pageResult = _bookGenerationService.GenerateBooks(parameters);
                allBooks.AddRange(pageResult.Items);
            }

            var csvContent = _csvExportService.ExportToCsv(allBooks, region.Value);

            return File(
                System.Text.Encoding.UTF8.GetBytes(csvContent),
                "text/csv",
                "books.csv"
            );
        }
    }
}
