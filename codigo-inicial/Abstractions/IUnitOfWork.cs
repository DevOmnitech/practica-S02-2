namespace RedditClone.Posts.Abstractions;

// "Cuando migremos a base de datos real lo vamos a necesitar."
public interface IUnitOfWork
{
    void BeginTransaction();
    void Commit();
    void Rollback();
}
