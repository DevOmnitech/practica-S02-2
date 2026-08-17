using RedditClone.Posts.Abstractions;
using RedditClone.Posts.Domain;
using RedditClone.Posts.Infrastructure;
using RedditClone.Posts.Services;

// =====================================================================
//  CODIGO INICIAL - PRACTICA S02 (Reddit clone: Posts)
//
//  ADVERTENCIA: este proyecto funciona, pero esta SOBRE-DISENADO A
//  PROPOSITO. Tiene fabricas, genericos, clases base y contratos que no
//  resuelven ningun problema real de hoy. Ademas, el calculo del score
//  esta copiado en varios lugares y las copias YA NO COINCIDEN.
//
//  TU TRABAJO NO ES AGREGAR CAPAS.
//  Tu trabajo es QUITAR lo que sobra (YAGNI), UNIFICAR el conocimiento
//  que si esta duplicado (DRY), SIMPLIFICAR lo que es innecesariamente
//  complejo (KISS) y REGISTRAR tus decisiones (ADRs). Ver todo.md.
// =====================================================================

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<InMemoryPostRepository>();
builder.Services.AddSingleton<IPostRepository>(sp => sp.GetRequiredService<InMemoryPostRepository>());
builder.Services.AddSingleton<IRepository<Post>, InMemoryPostRepository>();
builder.Services.AddSingleton<IPostFactory, PostFactory>();
builder.Services.AddSingleton<IUnitOfWork, NoOpUnitOfWork>();
builder.Services.AddSingleton<IEmailNotifier, ConsoleEmailNotifier>();
builder.Services.AddSingleton<TitleValidator>();
builder.Services.AddSingleton<CommentBodyValidator>();
builder.Services.AddSingleton<PostRankingCalculator>();
builder.Services.AddSingleton<PostService>();
builder.Services.AddSingleton<PostServiceFactory>();

var app = builder.Build();

// ---------------------------------------------------------------------
// POST /posts  ->  crear un post
// ---------------------------------------------------------------------
app.MapPost("/posts", (CreatePostDto dto, PostServiceFactory factory) =>
{
    var service = factory.Create();
    var result = service.CreatePost(dto.Title, dto.Body, dto.Kind);

    return result.IsSuccess
        ? Results.Ok(new
        {
            id = result.Value!.Id,
            title = result.Value!.Title,
            kind = result.Value!.Kind,
            score = result.Value!.Score
        })
        : Results.BadRequest(result.Error);
});

// ---------------------------------------------------------------------
// GET /posts/{id}  ->  consultar un post
// ---------------------------------------------------------------------
app.MapGet("/posts/{id}", (int id, PostService service) =>
{
    var result = service.GetPost(id);

    return result.IsSuccess
        ? Results.Ok(new
        {
            id = result.Value!.Id,
            title = result.Value!.Title,
            kind = result.Value!.Kind,
            upvotes = result.Value!.Upvotes,
            downvotes = result.Value!.Downvotes,
            comments = result.Value!.Comments.Count,
            awards = result.Value!.AwardCount,
            score = result.Value!.Score
        })
        : Results.NotFound(result.Error);
});

// ---------------------------------------------------------------------
// GET /posts  ->  listado ordenado por score
// ---------------------------------------------------------------------
app.MapGet("/posts", (IPostRepository repository) =>
{
    // Calculo del score
    var listado = repository.GetAll()
        .Select(p => new
        {
            id = p.Id,
            title = p.Title,
            kind = p.Kind,
            score = Math.Round((p.Upvotes - p.Downvotes) * 10.0 + p.Comments.Count * 3.0, 1)
        })
        .OrderByDescending(x => x.score)
        .ToList();

    return Results.Ok(listado);
});

// ---------------------------------------------------------------------
// POST /posts/{id}/vote  ->  votar (1 o -1)
// ---------------------------------------------------------------------
app.MapPost("/posts/{id}/vote", (int id, VoteDto dto, PostService service) =>
{
    var result = service.Vote(id, dto.Value);

    if (result.IsSuccess)
    {
        return Results.Ok(new { id = result.Value!.Id, score = result.Value!.Score });
    }

    return result.Error!.StartsWith("No existe")
        ? Results.NotFound(result.Error)
        : Results.BadRequest(result.Error);
});

// ---------------------------------------------------------------------
// POST /posts/{id}/comments  ->  comentar
// ---------------------------------------------------------------------
app.MapPost("/posts/{id}/comments", (int id, CreateCommentDto dto, PostService service) =>
{
    var result = service.AddComment(id, dto.Body);

    if (result.IsSuccess)
    {
        return Results.Ok(new
        {
            id = result.Value!.Id,
            comments = result.Value!.Comments.Count,
            score = result.Value!.Score
        });
    }

    return result.Error!.StartsWith("No existe")
        ? Results.NotFound(result.Error)
        : Results.BadRequest(result.Error);
});

// ---------------------------------------------------------------------
// POST /posts/{id}/awards  ->  dar un premio al post
// ---------------------------------------------------------------------
app.MapPost("/posts/{id}/awards", (int id, PostService service) =>
{
    var result = service.GiveAward(id);

    return result.IsSuccess
        ? Results.Ok(new { id = result.Value!.Id, awards = result.Value!.AwardCount, score = result.Value!.Score })
        : Results.NotFound(result.Error);
});

// ---------------------------------------------------------------------
// GET /posts/{id}/score-debug  ->  compara el score guardado contra el
// que devuelve el calculador OFICIAL. Deberian ser el mismo numero.
// ---------------------------------------------------------------------
app.MapGet("/posts/{id}/score-debug", (int id, IPostRepository repository, PostRankingCalculator calculator) =>
{
    var post = repository.GetById(id);
    if (post is null)
    {
        return Results.NotFound($"No existe el post {id}.");
    }

    return Results.Ok(new
    {
        id = post.Id,
        scoreGuardado = post.Score,
        scoreOficial = Math.Round(calculator.Calculate(post, DateTime.UtcNow), 1)
    });
});

app.Run();

record CreatePostDto(string? Title, string? Body, string? Kind);
record VoteDto(int Value);
record CreateCommentDto(string? Body);
