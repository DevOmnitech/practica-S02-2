namespace RedditClone.Posts.Domain;

public sealed class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string Body { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
