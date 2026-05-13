using BookDiscovery.Application.Interfaces;
using BookDiscovery.Domain.Models;
using BookDiscovery.Application.Common;

namespace BookDiscovery.Application.Services
{
    public class BookRankingService : IBookRankingService
    {
        public List<BookInfo> Rank(BookQueryIntent? query, List<BookInfo> candidates, string? rawQuery = null)
        {
            var results = new List<BookInfo>();

            bool hasIntent = query != null;

            var qTitle = hasIntent ? (query!.Title ?? "").NormalizeTitle() : "";
            var qAuthor = hasIntent ? (query!.Author ?? "").NormalizeTitle() : "";
            var keywords = hasIntent ? query!.Keywords : new List<string>();

            var fallbackKeywords = !hasIntent && !string.IsNullOrWhiteSpace(rawQuery)
                ? rawQuery.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.NormalizeTitle())
                    .ToList()
                : new List<string>();

            foreach (var book in candidates)
            {
                int score = 0;
                var explanationParts = new List<string>();

                var title = book.Title ?? "";
                var authorNames = book.Author ?? "";

                var normTitle = title.NormalizeTitle();
                var normAuthor = authorNames.NormalizeTitle();

                if (hasIntent)
                {
                    if (!string.IsNullOrWhiteSpace(qTitle) &&
                        normTitle.Contains(qTitle))
                    {
                        score += 50;
                        explanationParts.Add("Title matches extracted intent");
                    }

                    if (!string.IsNullOrWhiteSpace(qAuthor) &&
                        normAuthor.Contains(qAuthor))
                    {
                        score += 40;
                        explanationParts.Add("Author matches extracted intent");
                    }

                    if (!string.IsNullOrWhiteSpace(qTitle) &&
                        !string.IsNullOrWhiteSpace(qAuthor) &&
                        normTitle.Contains(qTitle) &&
                        normAuthor.Contains(qAuthor))
                    {
                        score += 60;
                        explanationParts.Add("Strong title + author match");
                    }

                    if (keywords != null)
                    {
                        foreach (var kw in keywords)
                        {
                            var normKw = (kw ?? "").NormalizeTitle();

                            if (normTitle.Contains(normKw))
                            {
                                score += 12;
                                explanationParts.Add($"Keyword '{kw}' in title");
                            }

                            if (normAuthor.Contains(normKw))
                            {
                                score += 8;
                                explanationParts.Add($"Keyword '{kw}' in author");
                            }
                        }
                    }
                }

                else
                {
                    foreach (var kw in fallbackKeywords)
                    {
                        if (string.IsNullOrWhiteSpace(kw))
                            continue;

                        if (normTitle.Contains(kw))
                        {
                            score += 15;
                            explanationParts.Add($"match: '{kw}' found in title");
                        }

                        if (normAuthor.Contains(kw))
                        {
                            score += 10;
                            explanationParts.Add($"match: '{kw}' found in author");
                        }

                        if (kw.All(char.IsDigit) && kw.Length == 4)
                        {
                            if (book.PublishedYear == kw)
                            {
                                score += 25;
                                explanationParts.Add($"Year match: {kw}");
                            }
                        }
                    }

                    if (explanationParts.Count == 0)
                    {
                        explanationParts.Add("Ranking based on query relevance signals");
                        score += 1;
                    }


                    if (!string.IsNullOrWhiteSpace(authorNames))
                    {
                        score += 2;
                    }
                }

                results.Add(new BookInfo
                {
                    Title = title,
                    Author = authorNames,
                    CoverImage = book.CoverImage,
                    OpenLibraryUrl = book.OpenLibraryUrl,
                    PublishedYear = book.PublishedYear ?? "",
                    Score = score,
                    ShortInfo = string.Join(". ", explanationParts)
                });
            }

            return results
                .OrderByDescending(x => x.Score)
                .Take(5)
                .ToList();
        }
    }
}