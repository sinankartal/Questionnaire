using Application.Validator;

namespace Common.Responses;

public class Response
{
    public bool IsValid { get; set; }
    public IReadOnlyList<Notification> Notifications { get; set; }

    public Response()
    {
        Notifications = new List<Notification>();
    }

    public static Response Success()
    {
        return new Response { IsValid = true };
    }

    public static Response Failure(IReadOnlyList<Notification> notifications)
    {
        return new Response { IsValid = false, Notifications = notifications };
    }
}