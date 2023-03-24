using Application.Validator;

namespace Common.Responses;

public class GenericResponse
{
    public bool IsValid { get; set; }
    public IReadOnlyList<Notification> Notifications { get; set; }

    public GenericResponse()
    {
        Notifications = new List<Notification>();
    }

    public static GenericResponse Success()
    {
        return new GenericResponse { IsValid = true };
    }

    public static GenericResponse Failure(IReadOnlyList<Notification> notifications)
    {
        return new GenericResponse { IsValid = false, Notifications = notifications };
    }
}