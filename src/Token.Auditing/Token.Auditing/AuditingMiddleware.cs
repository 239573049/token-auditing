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
        if (!_tokenAuditingOptions.IsEnabled || IsIgnoredUrl(context))
        {
            await next(context).ConfigureAwait(false);
        }
        else
        {
            var startNew = Stopwatch.StartNew();
            try
            {
                Console.WriteLine($"[debug] {tokenAuditingOptions.ApplicationName} {DateTime.Now:yyyy-MM-dd HH:mm:ss} Path： {context.Request.Path} " +
                                  $"{GetQuery(context.Request.QueryString)}");
                await next(context).ConfigureAwait(false);

                _logger.LogInformation("{ApplicationName} {DateTime} Path： {Path} {query} " +
                                  "耗时：{TotalMilliseconds} ms",tokenAuditingOptions.ApplicationName,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                                  context.Request.Path, GetQuery(context.Request.QueryString),startNew.Elapsed.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[debug] {tokenAuditingOptions.ApplicationName} {DateTime.Now:yyyy-MM-dd HH:mm:ss} Path： {context.Request.Path} {GetQuery(context.Request.QueryString)} " +
                                  $"异常》{ex+Environment.NewLine}" +
                                  $"耗时：{startNew.Elapsed.TotalMilliseconds} ms");
                if (!tokenAuditingOptions.IsError)
                {
                    throw ex;
                }
            }
        }
    }

    private string GetQuery(QueryString queryString)
        => queryString.HasValue ? "Query：" + queryString : string.Empty;

    private bool IsIgnoredUrl(HttpContext context) => context.Request.Path.Value != null &&
                                                      _tokenAspNetCoreAuditingOptions.IgnoredUrls.Any(x => context
                                                          .Request.Path.Value.StartsWith(x));
}