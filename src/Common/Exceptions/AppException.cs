using System.Globalization;
using System.Net;

namespace Common;

public class AppException : Exception
{
    public int StatusCode { get; set; }

    public AppException() : base()
    {
        StatusCode = (int)HttpStatusCode.BadRequest;
    }

    public AppException(string message) : base(message)
    {
        StatusCode = (int)HttpStatusCode.BadRequest;
    }
    
    public AppException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = (int)statusCode;
    }

    public AppException(string message, params object[] args)
        : base(String.Format(CultureInfo.CurrentCulture, message, args))
    {
        StatusCode = (int)HttpStatusCode.BadRequest;
    }
}