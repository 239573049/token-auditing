using Microsoft.AspNetCore.Http;

namespace Token.Auditing;

public class AuditingHttp
{
    /// <summary>
    /// 当前服务名称
    /// </summary>
    public string? ApplicationName { get; set; }

    /// <summary>
    /// 请求路由
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// 请求参数携带在url中的参数
    /// </summary>
    public QueryString? QueryString { get; set; }

    /// <summary>
    /// 全球完成时间（或者异常）
    /// </summary>
    public double? TotalMilliseconds { get; set; }

    public AuditingHttp(string? applicationName, string? path, QueryString? queryString, double? totalMilliseconds = null)
    {
        ApplicationName = applicationName;
        Path = path;
        QueryString = queryString;
        TotalMilliseconds = totalMilliseconds;
    }
}