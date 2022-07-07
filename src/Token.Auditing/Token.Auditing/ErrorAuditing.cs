using Microsoft.AspNetCore.Http;

namespace Token.Auditing;

public class ErrorAuditing : AuditingHttp
{
    public Exception Exception { get; }

    public ErrorAuditing(string? applicationName,
        string? path,
        QueryString? queryString,
        Exception exception,
        double? totalMilliseconds = null) : base(applicationName, path, queryString, totalMilliseconds)
    {
        Exception = exception;
    }
}