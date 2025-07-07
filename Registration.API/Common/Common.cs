using System.Net;

namespace Registration.API.Common;

public class AppSettings
{
    public string ConnectionString { get; set; } = null!;
    public int DefaultRegId { get; set; }
}

public class ActionReport
{
    public Exception? Exception { get; set; }
    public HttpStatusCode Code { get; set; }
    public bool Successful => Code == 0;
    public string Message { get; set; } = "عملیات با موفقیت انجام شد";

    public static ActionReport Success() => new()
    {
        Code = 0,
    };

    public static ActionReport Error(HttpStatusCode code, string message = null, Exception exception = null) => new()
    {
        Code = code,
        Message = message,
        Exception = exception
    };
}