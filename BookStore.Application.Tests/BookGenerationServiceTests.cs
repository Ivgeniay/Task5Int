using BookStore.Application.Services;
using BookStore.Domain.Models;

namespace BookStore.Application.Tests
{
    [TestFixture]
    public class BookGenerationServiceTests
    {
        private BookGenerationService _service;
        private GenerationParameters _defaultParameters;

        [SetUp]
        public void SetUp()
        {
            _service = new BookGenerationService();
            _defaultParameters = new GenerationParameters
            {
                Region = Region.DefaultRegion(),
                UserSeed = 12345,
                AverageLikes = 5.0m,
                AverageReviews = 3.0m,
                PageNumber = 1
            };
        }

        [Test]
        public void GenerateBooks_ShouldReturnCorrectPageSize()
        {
            var result = _service.GenerateBooks(_defaultParameters);

            Assert.That(result.Items.Count, Is.EqualTo(AppConstants.PageSize));
            Assert.That(result.HasMore, Is.True);
        }

        [Test]
        public void GenerateBooks_SameSeed_ShouldReturnSameResults()
        {
            var result1 = _service.GenerateBooks(_defaultParameters);
            var result2 = _service.GenerateBooks(_defaultParameters);

            Assert.That(result1.Items.Count, Is.EqualTo(result2.Items.Count));
            for (int i = 0; i < result1.Items.Count; i++)
            {
                Assert.That(result1.Items[i].Title, Is.EqualTo(result2.Items[i].Title));
                Assert.That(result1.Items[i].ISBN, Is.EqualTo(result2.Items[i].ISBN));
                Assert.That(result1.Items[i].Publisher, Is.EqualTo(result2.Items[i].Publisher));
                Assert.That(result1.Items[i].Authors, Is.EqualTo(result2.Items[i].Authors));
            }
        }

        [Test]
        public void GenerateBooks_DifferentSeed_ShouldReturnDifferentResults()
        {
            var parameters2 = new GenerationParameters
            {
                Region = _defaultParameters.Region,
                UserSeed = 54321,
                AverageLikes = _defaultParameters.AverageLikes,
                AverageReviews = _defaultParameters.AverageReviews,
                PageNumber = _defaultParameters.PageNumber
            };

            var result1 = _service.GenerateBooks(_defaultParameters);
            var result2 = _service.GenerateBooks(parameters2);

            Assert.That(result1.Items[0].Title, Is.Not.EqualTo(result2.Items[0].Title));
        }

        [Test]
        public void GenerateBooks_CorrectIndexCalculation_Page1()
        {
            _defaultParameters.PageNumber = 1;
            var result = _service.GenerateBooks(_defaultParameters);

            for (int i = 0; i < result.Items.Count; i++)
                Assert.That(result.Items[i].Index, Is.EqualTo(i + 1));
        }

        [Test]
        public void GenerateBooks_CorrectIndexCalculation_Page2()
        {
            _defaultParameters.PageNumber = 2;
            var result = _service.GenerateBooks(_defaultParameters);

            for (int i = 0; i < result.Items.Count; i++)
                Assert.That(result.Items[i].Index, Is.EqualTo(AppConstants.PageSize + i + 1));
        }

        [Test]
        public void GenerateBooks_CorrectIndexCalculation_Page3()
        {
            _defaultParameters.PageNumber = 3;
            var result = _service.GenerateBooks(_defaultParameters);

            for (int i = 0; i < result.Items.Count; i++)
                Assert.That(result.Items[i].Index, Is.EqualTo(2 * AppConstants.PageSize + i + 1));
        }

        [Test]
        public void GenerateBookDetails_ShouldReturnCorrectBook()
        {
            var bookIndex = 15;
            var details = _service.GenerateBookDetails(bookIndex, _defaultParameters);

            Assert.That(details.Book.Index, Is.EqualTo(bookIndex));
            Assert.That(details.CoverImageUrl, Is.Not.Empty);
            Assert.That(details.CoverImageUrl, Does.Contain(AppConstants.PicsumBaseUrl));
        }

        [Test]
        public void GenerateBookDetails_ZeroLikes_ShouldReturnZeroLikes()
        {
            _defaultParameters.AverageLikes = 0m;
            var details = _service.GenerateBookDetails(1, _defaultParameters);

            Assert.That(details.Likes, Is.EqualTo(0));
        }

        [Test]
        public void GenerateBookDetails_ZeroReviews_ShouldReturnNoReviews()
        {
            _defaultParameters.AverageReviews = 0m;
            var details = _service.GenerateBookDetails(1, _defaultParameters);

            Assert.That(details.Reviews.Count, Is.EqualTo(0));
        }

        [Test]
        public void GenerateBookDetails_FractionalReviews_ShouldHandleProbability()
        {
            _defaultParameters.AverageReviews = 0.5m;
            var reviewCounts = new List<int>();

            for (int i = 0; i < 100; i++)
            {
                var details = _service.GenerateBookDetails(i + 1, _defaultParameters);
                reviewCounts.Add(details.Reviews.Count);
            }

            var zeroReviews = reviewCounts.Count(c => c == 0);
            var oneReview = reviewCounts.Count(c => c == 1);

            Assert.That(zeroReviews, Is.GreaterThan(30));
            Assert.That(oneReview, Is.GreaterThan(30));
            Assert.That(reviewCounts.All(c => c <= 1), Is.True);
        }

        [Test]
        public void GenerateBookDetails_FractionalLikes_ShouldHandleProbability()
        {
            _defaultParameters.AverageLikes = 2.7m;
            var likeCounts = new List<int>();

            for (int i = 0; i < 100; i++)
            {
                var details = _service.GenerateBookDetails(i + 1, _defaultParameters);
                likeCounts.Add(details.Likes);
            }

            var twoLikes = likeCounts.Count(c => c == 2);
            var threeLikes = likeCounts.Count(c => c == 3);

            Assert.That(twoLikes, Is.GreaterThan(20));
            Assert.That(threeLikes, Is.GreaterThan(20));
        }

        [Test]
        public void GenerateBooks_DifferentRegions_ShouldGenerateDifferentContent()
        {
            var enParameters = new GenerationParameters
            {
                Region = Region.ENRegion(),
                UserSeed = 12345,
                AverageLikes = 5.0m,
                AverageReviews = 3.0m,
                PageNumber = 1
            };

            var ruParameters = new GenerationParameters
            {
                Region = Region.RURegion(),
                UserSeed = 12345,
                AverageLikes = 5.0m,
                AverageReviews = 3.0m,
                PageNumber = 1
            };

            var enResult = _service.GenerateBooks(enParameters);
            var ruResult = _service.GenerateBooks(ruParameters);

            Assert.That(enResult.Items[0].Authors[0], Is.Not.EqualTo(ruResult.Items[0].Authors[0]));
            Assert.That(enResult.Items[0].Publisher, Is.Not.EqualTo(ruResult.Items[0].Publisher));
        }

        [Test]
        public void GenerateBooks_AllBooksHaveRequiredFields()
        {
            var result = _service.GenerateBooks(_defaultParameters);

            foreach (var book in result.Items)
            {
                Assert.That(book.Index, Is.GreaterThan(0));
                Assert.That(book.ISBN, Is.Not.Empty);
                Assert.That(book.Title, Is.Not.Empty);
                Assert.That(book.Authors, Is.Not.Empty);
                Assert.That(book.Publisher, Is.Not.Empty);
            }
        }
    }
}
