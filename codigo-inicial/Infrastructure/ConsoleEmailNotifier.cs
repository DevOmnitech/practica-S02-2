using RedditClone.Posts.Abstractions;

namespace RedditClone.Posts.Infrastructure;

public sealed class ConsoleEmailNotifier : IEmailNotifier
{
    private readonly ILogger<ConsoleEmailNotifier> _logger;

    public ConsoleEmailNotifier(ILogger<ConsoleEmailNotifier> logger) => _logger = logger;

    public void Send(string to, string subject, string body)
        => _logger.LogInformation("Correo simulado {To} {Subject}", to, subject);
}
