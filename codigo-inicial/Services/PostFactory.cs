using RedditClone.Posts.Abstractions;
using RedditClone.Posts.Domain;

namespace RedditClone.Posts.Services;

public sealed class PostFactory : IPostFactory
{
    public Post Create(string title, string body, string kind) => new()
    {
        Title = title,
        Body = body,
        Kind = kind,
        CreatedAt = DateTime.UtcNow
    };
}
