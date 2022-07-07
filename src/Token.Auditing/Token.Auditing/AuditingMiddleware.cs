using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Token.Module.Dependencys;
using System.Diagnostics;

namespace Token.Auditing;

public class AuditingMiddleware : IMiddleware, ITransientDependency
{
    private readonly TokenAspNetCoreAuditingOptions tokenAspNetCoreAuditingOptions;
    private readonly TokenAuditingOptions tokenAuditingOptions;

    public AuditingMiddleware(IOptions<TokenAspNetCoreAuditingOptions> tokenAspNetCoreAuditingOptions,
                              IOptions<TokenAuditingOptions> tokenAuditingOptions)
    {
        this.tokenAspNetCoreAuditingOptions = tokenAspNetCoreAuditingOptions.Value;
        this.tokenAuditingOptions = tokenAuditingOptions.Value;
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
                Console.WriteLine($"[debug] {tokenAuditingOptions.ApplicationName} {DateTime.Now:yyyy-MM-dd HH:mm:ss} Path： {context.Request.Path} " +
                                  $"{GetQuery(context.Request.QueryString)}");
                await next(context).ConfigureAwait(false);
                Console.WriteLine($"[debug] {tokenAuditingOptions.ApplicationName} {DateTime.Now:yyyy-MM-dd HH:mm:ss} Path： {context.Request.Path} {GetQuery(context.Request.QueryString)} " +
                                  $"耗时：{startNew.Elapsed.TotalMilliseconds} ms");
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
                                                      tokenAspNetCoreAuditingOptions.IgnoredUrls.Any(x => context
                                                          .Request.Path.Value.StartsWith(x));
}