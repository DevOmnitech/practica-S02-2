using RedditClone.Posts.Domain;

namespace RedditClone.Posts.Abstractions;

public interface IPostRepository
{
    Post Add(Post post);
    Post? GetById(int id);
    IReadOnlyList<Post> GetAll();
    void Update(Post post);
}
