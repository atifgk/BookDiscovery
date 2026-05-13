namespace BookDiscovery.Domain.Models
{
    public class BookQueryIntent
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public List<string> Keywords { get; set; } = new();
        public bool IsAIExtracted { get; set; } = false;
    }
}
