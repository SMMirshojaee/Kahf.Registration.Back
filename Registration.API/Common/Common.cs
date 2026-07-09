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
    public string FrontPaymentPage { get; set; } = null!;
    public string RepositoryAddress { get; set; } = null!;
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
    public new static ActionReport<T> UnAuthorize() => new()
    {
        Code = HttpStatusCode.Unauthorized,
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
    public static ActionReport UnAuthorize() => new()
    {
        Code = HttpStatusCode.Unauthorized,
    };
}

public static class Common
{
    private static readonly char[] _chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
    private static readonly char[] _numbers = "1234567890".ToCharArray();

    public static string CreateRandomString(byte length, bool numbersOnly = false)
    {
        byte[] data = new byte[length];
        using RandomNumberGenerator rng = RandomNumberGenerator.Create();
        rng.GetBytes(data);
        char[] source = numbersOnly ? _numbers : _chars;
        StringBuilder result = new StringBuilder(length);
        foreach (byte b in data)
        {
            result.Append(source[b % source.Length]);
        }

        return result.ToString();
    }
    
    public static string HashPassword(string password, string saltString)
    {
        byte[] salt = Encoding.UTF8.GetBytes(saltString);

        using Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt, 1_000, HashAlgorithmName.SHA256);
        byte[] hash = pbkdf2.GetBytes(32); // 256-bit hash

        return Convert.ToBase64String(hash);
    }
}