using Application.Validator;

namespace Common.Responses;

public class TypedResponse<T> : Response
{
    public T Data { get; set; }

    public static TypedResponse<T> Success(T data)
    {
        return new TypedResponse<T> { IsValid = true, Data = data };
    }

    public static TypedResponse<T> Failure(IReadOnlyList<Notification> notifications)
    {
        return new TypedResponse<T> { IsValid = false, Notifications = notifications };
    }
}