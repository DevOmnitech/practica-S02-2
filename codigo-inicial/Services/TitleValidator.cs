using RedditClone.Posts.Abstractions;

namespace RedditClone.Posts.Services;

// Regla de negocio: el titulo de un post se muestra completo en el feed,
// por eso esta acotado a 300 caracteres.
public sealed class TitleValidator : AbstractValidatorBase<string>
{
    protected override void RunRules(string value, List<string> errors)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add("El titulo es requerido.");
            return;
        }

        var trimmed = value.Trim();

        if (trimmed.Length < 3)
        {
            errors.Add("El titulo debe tener al menos 3 caracteres.");
        }

        if (trimmed.Length > 300)
        {
            errors.Add("El titulo no puede exceder 300 caracteres.");
        }
    }
}
