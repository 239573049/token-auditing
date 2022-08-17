using Microsoft.AspNetCore.Http;

namespace Token.Auditing;

public class TokenAuditingOptions
{
    /// <summary>
    /// 是否开启审计日志
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 是否开启异常处理
    /// </summary>
    /// <value></value>
    public bool IsError { get; set; }

    /// <summary>
    /// 写审计日志的应用程序或服务的名称。  
    /// </summary>
    public string ApplicationName { get; set; }

    /// <summary>
    /// 在请求前发生的
    /// </summary>
    public Action<AuditingHttp>? StartAuditing { get; set; }

    /// <summary>
    /// 在正常结束的时候发生的
    /// </summary>
    public Action<AuditingHttp> EndAuditing { get; set; }

    /// <summary>
    /// 在异常结束的时候发生
    /// </summary>
    public Action<AuditingHttp> ErrorAuditing { get; set; }

    /// <summary>
    /// 需要忽略的Url
    /// </summary>
    /// <typeparam name="string"></typeparam>
    /// <returns></returns>
    public List<string> IgnoredUrls { get; set; } = new List<string>();
}