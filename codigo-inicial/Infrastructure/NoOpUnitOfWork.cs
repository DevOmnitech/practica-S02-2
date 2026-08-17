using RedditClone.Posts.Abstractions;

namespace RedditClone.Posts.Infrastructure;

// No hay base de datos, asi que no hay transaccion que abrir ni cerrar.
public sealed class NoOpUnitOfWork : IUnitOfWork
{
    public void BeginTransaction() { }

    public void Commit() { }

    public void Rollback() { }
}
