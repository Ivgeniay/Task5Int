namespace BookStore.Application
{
    public static class AppConstants
    {
        public const int PageSize = 10;
        public const int InitialBatchSize = 20;
        public const int BookDetailsSeedMultiplier = 1000;

        public const int CoverImageWidth = 200;
        public const int CoverImageHeight = 300;

        public const int MinSentencesInReview = 1;
        public const int MaxSentencesInReview = 3;

        public const int MinRating = 1;
        public const int MaxRating = 5;

        public const float SingleAuthorProbability = 0.7f;
        public const float TwoAuthorsProbability = 0.25f;
        public const float ThreeAuthorsProbability = 0.05f;

        public const double ReviewMultiplier = 1.5;
        public const double LikesMultiplier = 2.0;

        public const string UnknownAuthor = "Unknown";
        public const string PicsumBaseUrl = "https://picsum.photos/seed";
    }
}
