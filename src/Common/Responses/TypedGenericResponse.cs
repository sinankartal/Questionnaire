using Application.Validator;

namespace Common.Responses;

public class TypedGenericResponse<T> : GenericResponse
{
    public T Data { get; set; }

    public static TypedGenericResponse<T> Success(T data)
    {
        return new TypedGenericResponse<T> { IsValid = true, Data = data };
    }

    public static TypedGenericResponse<T> Failure(IReadOnlyList<Notification> notifications)
    {
        return new TypedGenericResponse<T> { IsValid = false, Notifications = notifications };
    }
}