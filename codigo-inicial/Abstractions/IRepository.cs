namespace RedditClone.Posts.Abstractions;

// "Por si algun dia tenemos mas entidades."
// Hoy la unica entidad del proyecto es Post.
public interface IRepository<T> where T : class
{
    T Add(T entity);
    T? GetById(int id);
    IReadOnlyList<T> GetAll();
    void Update(T entity);
    void Delete(int id);
    IReadOnlyList<T> Find(Func<T, bool> predicate);
    int Count();
}
