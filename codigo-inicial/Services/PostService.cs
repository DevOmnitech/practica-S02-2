using RedditClone.Posts.Abstractions;
using RedditClone.Posts.Domain;

namespace RedditClone.Posts.Services;

public sealed class PostService
{
    private readonly IPostRepository _repository;
    private readonly IPostFactory _factory;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailNotifier _notifier;
    private readonly TitleValidator _titleValidator;
    private readonly CommentBodyValidator _commentBodyValidator;
    private readonly ILogger<PostService> _logger;

    public PostService(
        IPostRepository repository,
        IPostFactory factory,
        IUnitOfWork unitOfWork,
        IEmailNotifier notifier,
        TitleValidator titleValidator,
        CommentBodyValidator commentBodyValidator,
        ILogger<PostService> logger)
    {
        _repository = repository;
        _factory = factory;
        _unitOfWork = unitOfWork;
        _notifier = notifier;
        _titleValidator = titleValidator;
        _commentBodyValidator = commentBodyValidator;
        _logger = logger;
    }

    public Result<Post> CreatePost(string? title, string? body, string? kind)
    {
        var titleResult = _titleValidator.Validate(title ?? string.Empty);
        if (!titleResult.IsSuccess)
        {
            return Result.Fail<Post>(titleResult.Error!);
        }

        _unitOfWork.BeginTransaction();

        var post = _factory.Create(
            title!.Trim(),
            body?.Trim() ?? string.Empty,
            string.IsNullOrWhiteSpace(kind) ? "text" : kind!.Trim().ToLowerInvariant());

        // Calculo del score
        post.Score = Math.Round(
            (post.Upvotes - post.Downvotes) * 10.0 + post.Comments.Count * 3.0, 1);

        var saved = _repository.Add(post);

        _unitOfWork.Commit();

        _logger.LogInformation("Post creado {PostId} {PostKind}", saved.Id, saved.Kind);

        return Result.Ok(saved);
    }

    public Result<Post> GetPost(int id)
    {
        var post = _repository.GetById(id);
        return post is null
            ? Result.Fail<Post>($"No existe el post {id}.")
            : Result.Ok(post);
    }

    public Result<Post> Vote(int id, int value)
    {
        if (value != 1 && value != -1)
        {
            return Result.Fail<Post>("El voto debe ser 1 o -1.");
        }

        var post = _repository.GetById(id);
        if (post is null)
        {
            return Result.Fail<Post>($"No existe el post {id}.");
        }

        if (value == 1)
        {
            post.Upvotes++;
        }
        else
        {
            post.Downvotes++;
        }

        // Calculo del score
        var hours = (DateTime.UtcNow - post.CreatedAt).TotalHours;
        post.Score = Math.Round(
            (post.Upvotes - post.Downvotes) * 10.0 + post.Comments.Count * 3.0 - hours * 2.0, 1);

        _repository.Update(post);

        _logger.LogInformation("Voto registrado {PostId} {VoteValue}", id, value);

        return Result.Ok(post);
    }

    public Result<Post> AddComment(int id, string? body)
    {
        var post = _repository.GetById(id);
        if (post is null)
        {
            return Result.Fail<Post>($"No existe el post {id}.");
        }

        if (post.IsLocked)
        {
            return Result.Fail<Post>("El post esta bloqueado.");
        }

        var bodyResult = _commentBodyValidator.Validate(body ?? string.Empty);
        if (!bodyResult.IsSuccess)
        {
            return Result.Fail<Post>(bodyResult.Error!);
        }

        post.Comments.Add(new Comment
        {
            Id = post.Comments.Count + 1,
            PostId = post.Id,
            Body = body!.Trim(),
            CreatedAt = DateTime.UtcNow
        });

        // Calculo del score
        var hours = (DateTime.UtcNow - post.CreatedAt).TotalHours;
        post.Score = Math.Round(
            (post.Upvotes - post.Downvotes) * 10.0 + post.Comments.Count * 3.0 - hours * 2.0, 1);

        _repository.Update(post);

        return Result.Ok(post);
    }

    public Result<Post> GiveAward(int id)
    {
        var post = _repository.GetById(id);
        if (post is null)
        {
            return Result.Fail<Post>($"No existe el post {id}.");
        }

        post.AwardCount++;
        _repository.Update(post);

        return Result.Ok(post);
    }

    // Nadie llama a este metodo. Se escribio junto con IEmailNotifier.
    public void NotifyAuthor(Post post)
        => _notifier.Send("autor@reddit.local", "Tu post recibio actividad", post.Title);
}
