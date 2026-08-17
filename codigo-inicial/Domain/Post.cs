namespace RedditClone.Posts.Domain;

public sealed class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;

    // text | link | image
    public string Kind { get; set; } = "text";

    public int Upvotes { get; set; }
    public int Downvotes { get; set; }
    public int AwardCount { get; set; }

    public bool IsPinned { get; set; }
    public bool IsNsfw { get; set; }
    public bool IsLocked { get; set; }
    public bool AuthorIsModerator { get; set; }

    public DateTime CreatedAt { get; set; }
    public List<Comment> Comments { get; set; } = new();

    // El score se guarda calculado. Ojo: hay varios lugares que lo escriben.
    public double Score { get; set; }
}
