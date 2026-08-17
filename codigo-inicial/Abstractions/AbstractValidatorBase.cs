using RedditClone.Posts.Domain;

namespace RedditClone.Posts.Abstractions;

// Clase base "por si algun dia hay muchos validadores".
// Hoy tiene exactamente 2 herederos y dos hooks que nadie sobrescribe.
public abstract class AbstractValidatorBase<T>
{
    public Result<T> Validate(T value)
    {
        OnBeforeValidate(value);

        var errors = new List<string>();
        RunRules(value, errors);

        OnAfterValidate(value);

        return errors.Count > 0
            ? Result.Fail<T>(string.Join(" ", errors))
            : Result.Ok(value);
    }

    protected abstract void RunRules(T value, List<string> errors);

    protected virtual void OnBeforeValidate(T value) { }

    protected virtual void OnAfterValidate(T value) { }
}
