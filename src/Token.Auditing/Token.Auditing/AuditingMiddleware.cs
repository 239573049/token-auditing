using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Token.Module.Dependencys;

namespace Token.Auditing;

public class AuditingMiddleware : IMiddleware, ITransientDependency
{
    private readonly TokenAspNetCoreAuditingOptions tokenAspNetCoreAuditingOptions;
    private readonly TokenAuditingOptions tokenAuditingOptions;
    private readonly ILogger<AuditingMiddleware> _logger;
    public AuditingMiddleware(IOptions<TokenAspNetCoreAuditingOptions> tokenAspNetCoreAuditingOptions,
                              IOptions<TokenAuditingOptions> tokenAuditingOptions,
                              ILogger<AuditingMiddleware> logger)
    {
        this.tokenAspNetCoreAuditingOptions = tokenAspNetCoreAuditingOptions.Value;
        this.tokenAuditingOptions = tokenAuditingOptions.Value;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (!tokenAuditingOptions.IsEnabled || IsIgnoredUrl(context))
        {
            await next(context).ConfigureAwait(false);
        }
        else
        {
            var startNew = Stopwatch.StartNew();
            try
            {
                _logger.LogInformation(" {ApplicationName} {Date} Path： {Path} " +
                                  "{message}",tokenAuditingOptions.ApplicationName,
                                  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),context.Request.Path, GetQuery(context.Request.QueryString));

                await next(context).ConfigureAwait(false);

                _logger.LogInformation("{ApplicationName} {DateTime} Path： {Path} {query} " +
                                  "耗时：{TotalMilliseconds} ms",tokenAuditingOptions.ApplicationName,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                  context.Request.Path, GetQuery(context.Request.QueryString),startNew.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError("{ApplicationName} {DateTime}  Path： {Path} {Query}  {Exception} {NewLine}  {FTotalMilliseconds} ms",tokenAuditingOptions.ApplicationName,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                context.Request.Path,
                GetQuery(context.Request.QueryString),
                ex ,Environment.NewLine,startNew.Elapsed.TotalMilliseconds
                );
                if (!tokenAuditingOptions.IsError)
                {
                    throw ex;
                }
            }
        }
    }

    private static string GetQuery(QueryString queryString)
        => queryString.HasValue ? "Query：" + queryString : string.Empty;

    private bool IsIgnoredUrl(HttpContext context) => context.Request.Path.Value != null &&
                                                      tokenAspNetCoreAuditingOptions.IgnoredUrls.Any(x => context
                                                          .Request.Path.Value.StartsWith(x));
}