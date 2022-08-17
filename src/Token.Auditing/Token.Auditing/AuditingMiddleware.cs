using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Token.Module.Dependencys;

namespace Token.Auditing;

public class AuditingMiddleware : IMiddleware, ITransientDependency
{
    private readonly TokenAuditingOptions _tokenAuditingOptions;
    private readonly ILogger<AuditingMiddleware> _logger;
    public AuditingMiddleware(
                              IOptions<TokenAuditingOptions> tokenAuditingOptions,
                              ILogger<AuditingMiddleware> logger)
    {
        _tokenAuditingOptions = tokenAuditingOptions.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!_tokenAuditingOptions.IsEnabled || IsIgnoredUrl(context))
        {
            await next(context).ConfigureAwait(false);
        }
        else
        {
            var startNew = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation("{ApplicationName} {DateTime} Path： {Path} {query}",_tokenAuditingOptions.ApplicationName,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                  context.Request.Path, GetQuery(context.Request.QueryString));
                await next(context).ConfigureAwait(false);

                _logger.LogInformation("{ApplicationName} {DateTime} Path： {Path} {query} " +
                                  "耗时：{TotalMilliseconds} ms",_tokenAuditingOptions.ApplicationName,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                  context.Request.Path, GetQuery(context.Request.QueryString),startNew.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                
                _logger.LogError("{ApplicationName} {DateTime} Path： {Path} {query} {NewLine} {Exception}" +
                                  "耗时：{TotalMilliseconds} ms",_tokenAuditingOptions.ApplicationName,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                  context.Request.Path, GetQuery(context.Request.QueryString),Environment.NewLine,ex,startNew.Elapsed.TotalMilliseconds);
                if (!_tokenAuditingOptions.IsError)
                {
                    throw ex;
                }
            }
        }
    }

    private string GetQuery(QueryString queryString)
        => queryString.HasValue ? "Query：" + queryString : string.Empty;

    private bool IsIgnoredUrl(HttpContext context) => context.Request.Path.Value != null &&
                                                      _tokenAuditingOptions.IgnoredUrls.Any(x => context
                                                          .Request.Path.Value.StartsWith(x));
}