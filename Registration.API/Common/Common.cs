using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Registration.API.Common;

public class AppSettings
{
    public string ConnectionString { get; set; } = null!;
    public string SecretKey { get; set; } = null!;
    public string Issuer { get; set; } = null!;
    public string Audience { get; set; } = null!;
    public int DefaultRegId { get; set; }
}

public class ActionReport<T> : ActionReport
{
    public T? Output { get; set; }
    public static ActionReport<T> Success(T output) => new()
    {
        Code = 0,
        Output = output
    };

    public new static ActionReport<T> Error(HttpStatusCode code, string message = null, Exception exception = null) => new()
    {
        Code = code,
        Message = message,
        Exception = exception
    };
    public new static ActionReport<T> Error(ActionReport report) => new()
    {
        Code = report.Code,
        Message = report.Message,
        Exception = report.Exception
    };
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
    public static ActionReport Error(ActionReport report) => new()
    {
        Code = report.Code,
        Message = report.Message,
        Exception = report.Exception
    };
}

public static class Common
{
    private static readonly char[] _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    public static string CreateRandomString(byte length)
    {
        var data = new byte[length];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(data);

        var result = new StringBuilder(length);
        foreach (var b in data)
        {
            result.Append(_chars[b % _chars.Length]);
        }

        return result.ToString();
    }
}