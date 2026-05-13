using BookDiscovery.Server.Models;

namespace BookDiscovery.Server.Services
{
    public interface IBookRankingService
    {
        List<BookResultModel> Rank(BookQueryIntentModel query, List<OpenLibraryBookDoc> candidates);
    }

    public class BookRankingService : IBookRankingService
    {
        public List<BookResultModel> Rank(BookQueryIntentModel query, List<OpenLibraryBookDoc> candidates)
        {
            var results = new List<BookResultModel>();

            var qTitle = Normalize(query.Title ?? "");
            var qAuthor = Normalize(query.Author ?? "");

            foreach (var book in candidates)
            {
                int score = 0;
                var explanationParts = new List<string>();

                var title = book.Title ?? "";
                var authorNames = string.Join(", ", book.AuthorNames ?? Enumerable.Empty<string>());

                // 1. Exact title match
                if (!string.IsNullOrWhiteSpace(qTitle) && Normalize(title).Contains(qTitle))
                {
                    score += 50;
                    explanationParts.Add("Title closely matches extracted intent");
                }

                // 2. Exact author match
                if (!string.IsNullOrWhiteSpace(qAuthor))
                {
                    var authors = Normalize(authorNames);

                    if (authors.Contains(qAuthor))
                    {
                        score += 40;
                        explanationParts.Add("Author matches extracted intent");
                    }
                }

                // 3. Both title + author strong match
                if (!string.IsNullOrWhiteSpace(qTitle) &&
                    !string.IsNullOrWhiteSpace(qAuthor) &&
                    Normalize(title).Contains(qTitle) &&
                    Normalize(authorNames).Contains(qAuthor))
                {
                    score += 60;
                    explanationParts.Add("Strong title + author match (highest confidence)");
                }

                // 4. Keyword matching (AI extracted keywords)
                if (query.Keywords != null)
                {
                    foreach (var kw in query.Keywords)
                    {
                        var normKw = Normalize(kw);

                        if (Normalize(title).Contains(normKw))
                        {
                            score += 12;
                            explanationParts.Add($"Keyword '{kw}' found in title");
                        }

                        if (Normalize(authorNames).Contains(normKw))
                        {
                            score += 8;
                            explanationParts.Add($"Keyword '{kw}' found in author");
                        }
                    }
                }

                if (string.IsNullOrWhiteSpace(authorNames))
                {
                    score -= 10;
                    explanationParts.Add("Missing reliable author metadata");
                }

                results.Add(new BookResultModel
                {
                    Title = title,
                    Author = authorNames,
                    PublishedYear = book.FirstPublishYear?.ToString() ?? "",
                    //WorkKey = book.OpenLibraryUrl,
                    Score = score,
                    ShortInfo = string.Join("; ", explanationParts)
                });
            }

            // FINAL SORTING
            return results
                .OrderByDescending(x => x.Score)
                .Take(5)
                .ToList();
        }

        private string Normalize(string input)
        {
            return (input ?? "")
                .ToLowerInvariant()
                .Trim();
        }
    }
}
