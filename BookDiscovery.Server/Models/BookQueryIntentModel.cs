namespace BookDiscovery.Server.Models
{
    public class BookQueryIntentModel
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public List<string> Keywords { get; set; } = new();
    }
}
