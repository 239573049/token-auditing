namespace Token.Auditing;
public class TokenAspNetCoreAuditingOptions
{
    /// <summary>
    /// 以一个被忽略的URL开始。
    /// </summary>
    public List<string> IgnoredUrls { get; } = new List<string>();
}
