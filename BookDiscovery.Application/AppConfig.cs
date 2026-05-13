namespace BookDiscovery.Application
{
    public interface IAppConfig
    {
        string OpenAIApiKey { get; }
    }

    public class AppConfig : IAppConfig
    {
        public required string OpenAIApiKey { get; set; }
    }
}
