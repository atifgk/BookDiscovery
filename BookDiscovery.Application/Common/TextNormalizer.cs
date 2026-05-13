using System.Text.RegularExpressions;

namespace BookDiscovery.Application.Common
{
    public static class TextNormalizer
    {
        public static string Normalize(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            input = input.ToLowerInvariant();

            input = Regex.Replace(input, @"[^\w\s]", " ");

            input = Regex.Replace(input, @"\s+", " ");

            return input.Trim();
        }
    }
}
