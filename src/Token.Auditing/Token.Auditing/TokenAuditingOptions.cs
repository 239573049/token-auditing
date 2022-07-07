namespace Token.Auditing;
public class TokenAuditingOptions
{
    /// <summary>
    /// 发生异常拦截处理异常
    /// 并且记录异常
    /// </summary>
    public bool IsError { get; set; }
    /// <summary>
    /// 是否开启审计日志
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// 写审计日志的应用程序或服务的名称。  
    /// </summary>
    public string ApplicationName { get; set; }
}
