namespace Application.Validator;

public interface ICustomValidator<T>
{
    IReadOnlyList<Notification> Notifications { get; }
    Task<bool> ValidateAsync(T item);
}