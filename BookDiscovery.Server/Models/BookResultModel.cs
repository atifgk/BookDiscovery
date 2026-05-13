namespace BookDiscovery.Server.Models
{
    public class BookResultModel
    {
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string PublishedYear { get; set; } = string.Empty;

        public string ShortInfo { get; set; } = string.Empty;

        public string? CoverImage { get; set; }

        public string? OpenLibraryUrl { get; set; }

        public int Score { get; set; } = 0;
    }
}
