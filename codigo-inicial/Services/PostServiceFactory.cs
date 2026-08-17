namespace RedditClone.Posts.Services;

// Una fabrica que le pide al contenedor de DI lo que el contenedor de DI
// ya sabe inyectar solo. Se creo "para poder cambiar la implementacion".
public sealed class PostServiceFactory
{
    private readonly IServiceProvider _provider;

    public PostServiceFactory(IServiceProvider provider) => _provider = provider;

    public PostService Create() => _provider.GetRequiredService<PostService>();
}
