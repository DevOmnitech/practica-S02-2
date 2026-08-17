using RedditClone.Posts.Abstractions;

namespace RedditClone.Posts.Services;

// Regla de negocio: el cuerpo de un comentario esta acotado para que la
// conversacion siga siendo legible en hilos largos.
public sealed class CommentBodyValidator : AbstractValidatorBase<string>
{
    protected override void RunRules(string value, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add("El comentario es requerido.");
            return;
        }

        var trimmed = value.Trim();

        if (trimmed.Length < 3)
        {
            errors.Add("El comentario debe tener al menos 3 caracteres.");
        }

        if (trimmed.Length > 300)
        {
            errors.Add("El comentario no puede exceder 300 caracteres.");
        }
    }
}
