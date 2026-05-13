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

            var qTitle = query.Title ?? "";
            var qAuthor = query.Author ?? "";

            foreach (var book in candidates)
            {
                int score = 0;
                var explanationParts = new List<string>();

                var title = book.Title ?? "";
                var authorNames = string.Join(", ", book.AuthorNames ?? Enumerable.Empty<string>());

                // 1. Exact title match
                if (!string.IsNullOrEmpty(qTitle) && title.Contains(qTitle))
                {
                    score += 40;
                    explanationParts.Add("Title matches query");
                }

                // 2. Exact author match
                if (!string.IsNullOrEmpty(qAuthor) && authorNames.Contains(qAuthor))
                {
                    score += 35;
                    explanationParts.Add("Author matches query");
                }

                // 3. Both title + author strong match
                if (!string.IsNullOrEmpty(qTitle) &&
                    !string.IsNullOrEmpty(qAuthor) &&
                    title.Contains(qTitle) &&
                    authorNames.Contains(qAuthor))
                {
                    score += 50;
                    explanationParts.Add("Strong title + author match");
                }

                // 4. Keyword matching (AI extracted keywords)
                if (query.Keywords != null)
                {
                    foreach (var kw in query.Keywords)
                    {
                        var normKw = kw;

                        if (title.Contains(normKw) || authorNames.Contains(normKw))
                        {
                            score += 10;
                            explanationParts.Add($"Keyword '{kw}' matched");
                        }
                    }
                }

                // 5. Penalty for weak/unknown author
                if (string.IsNullOrEmpty(authorNames) || authorNames == "Unknown Author")
                {
                    score -= 5;
                    explanationParts.Add("Weak author metadata");
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
    }
}
