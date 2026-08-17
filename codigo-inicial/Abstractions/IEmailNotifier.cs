namespace RedditClone.Posts.Abstractions;

// "El dia que mandemos correos ya va a estar listo."
public interface IEmailNotifier
{
    void Send(string to, string subject, string body);
}
