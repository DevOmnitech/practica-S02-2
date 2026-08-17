using RedditClone.Posts.Domain;

namespace RedditClone.Posts.Services;

// Este es el calculo OFICIAL del score. Crecio por parches: cada vez que se
// agrego una regla nueva, alguien metio un if adentro del if anterior.
public sealed class PostRankingCalculator
{
    public double Calculate(Post post, DateTime now)
    {
        double score = 0;

        if (post is not null)
        {
            if (post.Kind == "text")
            {
                if (post.AuthorIsModerator)
                {
                    score = (post.Upvotes - post.Downvotes) * 10 * 1.2;
                }
                else
                {
                    score = (post.Upvotes - post.Downvotes) * 10;
                }
            }
            else
            {
                if (post.Kind == "link")
                {
                    if (post.AuthorIsModerator)
                    {
                        score = (post.Upvotes - post.Downvotes) * 10 * 1.35;
                    }
                    else
                    {
                        score = (post.Upvotes - post.Downvotes) * 10 * 1.15;
                    }
                }
                else
                {
                    if (post.Kind == "image")
                    {
                        if (post.AuthorIsModerator)
                        {
                            score = (post.Upvotes - post.Downvotes) * 10 * 1.5;
                        }
                        else
                        {
                            score = (post.Upvotes - post.Downvotes) * 10 * 1.3;
                        }
                    }
                    else
                    {
                        score = (post.Upvotes - post.Downvotes) * 10;
                    }
                }
            }

            score = score + post.Comments.Count * 3;

            if (post.AwardCount > 0)
            {
                if (post.AwardCount > 5)
                {
                    score = score + 50;
                }
                else
                {
                    if (post.AwardCount > 2)
                    {
                        score = score + 25;
                    }
                    else
                    {
                        score = score + 10;
                    }
                }
            }

            if (post.IsPinned)
            {
                score = score + 1000;
            }

            if (post.IsNsfw)
            {
                if (!post.IsPinned)
                {
                    score = score - 30;
                }
            }

            if (post.IsLocked)
            {
                score = score - 15;
            }

            var hours = (now - post.CreatedAt).TotalHours;
            if (hours > 0)
            {
                score = score - hours * 2;
            }

            if (score < 0)
            {
                if (!post.IsPinned)
                {
                    score = 0;
                }
            }
        }

        return score;
    }
}
