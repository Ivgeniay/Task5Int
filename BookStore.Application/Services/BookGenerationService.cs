using BookStore.Domain.Models;
using Bogus;
using BookStore.Domain.Interfaces;
using System.Text;

namespace BookStore.Application.Services
{
    public class BookGenerationService : IBookGenerationService
    {
        public PagedResult<Book> GenerateBooks(GenerationParameters parameters)
        {
            var combinedSeed = parameters.UserSeed + parameters.PageNumber;
            Randomizer.Seed = new Random(combinedSeed);

            var faker = new Faker<Book>(parameters.Region.Code)
                .RuleFor(b => b.Index, f => 0)
                .RuleFor(b => b.ISBN, f => f.Commerce.Ean13())
                .RuleFor(b => b.Title, f => GenerateTitle(f))
                .RuleFor(b => b.Authors, f => GenerateAuthors(f))
                .RuleFor(b => b.Publisher, f => f.Company.CompanyName());

            var books = faker.Generate(AppConstants.PageSize);

            var startIndex = (parameters.PageNumber - 1) * AppConstants.PageSize + 1;
            for (int i = 0; i < books.Count; i++)
            {
                books[i].Index = startIndex + i;
            }

            return new PagedResult<Book>
            {
                Items = books,
                HasMore = true
            };
        }

        public BookDetails GenerateBookDetails(int bookIndex, GenerationParameters parameters)
        {
            var pageNumber = CalculatePageNumber(bookIndex);
            var indexInPage = CalculateIndexInPage(bookIndex);

            var bookParams = new GenerationParameters
            {
                Region = parameters.Region,
                UserSeed = parameters.UserSeed,
                AverageLikes = parameters.AverageLikes,
                AverageReviews = parameters.AverageReviews,
                PageNumber = pageNumber
            };

            var books = GenerateBooks(bookParams);
            var book = books.Items[indexInPage];

            var detailsSeed = parameters.UserSeed + bookIndex * AppConstants.BookDetailsSeedMultiplier;
            Randomizer.Seed = new Random(detailsSeed);
            var detailsFaker = new Faker(parameters.Region.Code);

            var likes = GenerateLikes(detailsFaker, parameters.AverageLikes);
            var reviews = GenerateReviews(detailsFaker, parameters.AverageReviews, parameters.Region.Code);
            var coverUrl = GenerateCoverUrl(book.Title, book.Authors.FirstOrDefault() ?? AppConstants.UnknownAuthor);

            return new BookDetails(book)
            {
                Likes = likes,
                Reviews = reviews,
                CoverImageUrl = coverUrl
            };
        }

        private string GenerateTitle(Faker faker)
        {
            var adjectives = new[] { "Great", "Ancient", "Lost", "Hidden", "Secret", "Dark", "Golden", "Silent", "Eternal", "Forbidden" };
            var nouns = new[] { "Journey", "Mystery", "Adventure", "Legend", "Kingdom", "Empire", "Castle", "Forest", "Mountain", "Ocean" };

            var titleTypes = new Func<string>[]
            {
            () => $"The {faker.PickRandom(adjectives)} {faker.PickRandom(nouns)}",
            () => $"{faker.PickRandom(nouns)} of {faker.PickRandom(adjectives)} {faker.PickRandom(nouns)}",
            () => $"A {faker.PickRandom(adjectives)} Story",
            () => $"The {faker.PickRandom(nouns)}",
            () => $"{faker.PickRandom(adjectives)} {faker.PickRandom(nouns)}"
            };

            return faker.PickRandom(titleTypes)();
        }

        private List<string> GenerateAuthors(Faker faker)
        {
            var authorCount = faker.Random.WeightedRandom(
                new[] { 1, 2, 3 },
                new[] { AppConstants.SingleAuthorProbability, AppConstants.TwoAuthorsProbability, AppConstants.ThreeAuthorsProbability });

            var authors = new List<string>();
            for (int i = 0; i < authorCount; i++)
            {
                authors.Add(faker.Name.FullName());
            }

            return authors;
        }

        private int GenerateLikes(Faker faker, decimal averageLikes)
        {
            if (averageLikes == 0) return 0;

            var baseCount = (int)Math.Floor(averageLikes);
            var fractionalPart = averageLikes - baseCount;
            var extraLike = faker.Random.Double() < (double)fractionalPart ? 1 : 0;

            return baseCount + extraLike;
        }

        private List<Review> GenerateReviews(Faker faker, decimal averageReviews, string locale)
        {
            var reviewCount = GenerateReviewCount(faker, averageReviews);
            var reviews = new List<Review>();

            Randomizer.Seed = new Random(faker.Random.Int());
            var reviewFaker = new Faker<Review>(locale)
                .RuleFor(r => r.Author, f => f.Name.FullName())
                .RuleFor(r => r.Text, f => GenerateReviewText(f))
                .RuleFor(r => r.Rating, f => f.Random.Int(AppConstants.MinRating, AppConstants.MaxRating));

            for (int i = 0; i < reviewCount; i++)
            {
                reviews.Add(reviewFaker.Generate());
            }

            return reviews;
        }

        private int GenerateReviewCount(Faker faker, decimal averageReviews)
        {
            if (averageReviews == 0) return 0;

            var baseCount = (int)Math.Floor(averageReviews);
            var fractionalPart = averageReviews - baseCount;
            var extraReview = faker.Random.Double() < (double)fractionalPart ? 1 : 0;

            return baseCount + extraReview;
        }

        private string GenerateReviewText(Faker faker)
        {
            var sentenceCount = faker.Random.Int(AppConstants.MinSentencesInReview, AppConstants.MaxSentencesInReview);
            var sb = new StringBuilder();

            for (int i = 0; i < sentenceCount; i++)
            {
                var wordCount = faker.Random.Int(5, 15);
                var words = faker.Lorem.Words(wordCount);
                var sentence = string.Join(" ", words);
                sentence = char.ToUpper(sentence[0]) + sentence.Substring(1) + ".";

                if (i > 0) sb.Append(" ");
                sb.Append(sentence);
            }

            return sb.ToString();
        }

        private string GenerateCoverUrl(string title, string author)
        {
            var seed = Math.Abs((title + author).GetHashCode());
            return $"{AppConstants.PicsumBaseUrl}/{seed}/{AppConstants.CoverImageWidth}/{AppConstants.CoverImageHeight}";
        }

        private int CalculatePageNumber(int bookIndex) => (bookIndex - 1) / AppConstants.PageSize + 1;

        private int CalculateIndexInPage(int bookIndex) => (bookIndex - 1) % AppConstants.PageSize;
    }



}
