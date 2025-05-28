namespace BookStore.Domain.Models
{
    public readonly struct Region
    {
        public string Code { get; }
        public string DisplayName { get; }
        public string Language { get; }
        public string Country { get; }
        public string CultureCode { get; }

        public Region(string code, string displayName, string language, string country)
        {
            Code = code;
            DisplayName = displayName;
            Language = language;
            Country = country;
            CultureCode = string.Concat(language, "-", country);
        }

        public static Region DefaultRegion() => ENRegion();
        public static Region ENRegion() => new("en", "English (USA)", "en", "US");
        public static Region RURegion() => new("ru", "Russian (Russia)", "ru", "RU");
        public static Region DERegion() => new("de", "German (Germany)", "de", "DE");
        public static Region JPRegion() => new("ja", "Japanese (Japan)", "ja", "JP");
        public static Region FRRegion() => new("fr", "French (France)", "fr", "FR");
        public static Region ESRegion() => new("es", "Spanish (Spain)", "es", "ES");
        public static Region ZHRegion() => new("zh_CN", "Chinese (China)", "zh", "CN");
        public static Region GERegion() => new("ge", "Georgian (Georgia)", "ka", "GE");

        public static IEnumerable<Region> GetAllRegions()
        {
            yield return ENRegion();
            yield return GERegion();
            yield return RURegion();
            yield return DERegion();
            yield return JPRegion();
            yield return FRRegion();
            yield return ESRegion();
            yield return ZHRegion();
        }

        public override bool Equals(object? obj) => obj is Region other && 
                                                    string.Compare(Code, other.Code, StringComparison.OrdinalIgnoreCase) == 0 &&
                                                    string.Compare(DisplayName, other.DisplayName, StringComparison.OrdinalIgnoreCase) == 0 &&
                                                    string.Compare(Language, other.Language, StringComparison.OrdinalIgnoreCase) == 0 &&
                                                    string.Compare(Country, other.Country, StringComparison.OrdinalIgnoreCase) == 0;
        public override int GetHashCode() =>
            checked (Code.GetHashCode() + 
            DisplayName.GetHashCode() + 
            Language.GetHashCode() + 
            Country.GetHashCode());
        
        public static bool operator ==(Region left, Region right) => left.Equals(right);
        public static bool operator !=(Region left, Region right) => !left.Equals(right);
    }
}
