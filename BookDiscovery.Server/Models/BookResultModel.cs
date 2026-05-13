namespace BookDiscovery.Server.Models
{
    public class BookResultModel
    {
        public string Title { get; set; } = string.Empty;

        public string Author { get; set; } = string.Empty;

        public string PublishedDate { get; set; } = string.Empty;

        public string ShortInfo { get; set; } = string.Empty;

        public string? CoverImage { get; set; }

        public string? OpenLibraryUrl { get; set; }
    }
}
