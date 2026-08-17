using RedditClone.Posts.Abstractions;
using RedditClone.Posts.Domain;

namespace RedditClone.Posts.Infrastructure;

public sealed class InMemoryPostRepository : IPostRepository, IRepository<Post>
{
    private readonly List<Post> _items = new();
    private int _nextId = 1;

    public Post Add(Post post)
    {
        post.Id = _nextId++;
        _items.Add(post);
        return post;
    }

    public Post? GetById(int id) => _items.FirstOrDefault(p => p.Id == id);

    public IReadOnlyList<Post> GetAll() => _items;

    public void Update(Post post)
    {
        // Los objetos ya son por referencia; este metodo existe para cumplir la interfaz.
    }

    // --- Miembros que solo existen para satisfacer IRepository<T> ---

    public void Delete(int id)
    {
        var found = GetById(id);
        if (found is not null)
        {
            _items.Remove(found);
        }
    }

    public IReadOnlyList<Post> Find(Func<Post, bool> predicate) => _items.Where(predicate).ToList();

    public int Count() => _items.Count;
}
