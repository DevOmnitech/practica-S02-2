using RedditClone.Posts.Domain;

namespace RedditClone.Posts.Abstractions;

// Una interfaz para construir un objeto que se podria construir con new.
public interface IPostFactory
{
    Post Create(string title, string body, string kind);
}
