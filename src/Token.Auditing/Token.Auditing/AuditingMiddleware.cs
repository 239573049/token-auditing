using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Token.Module.Dependencys;
using System.Diagnostics;

namespace Token.Auditing;

public class AuditingMiddleware : IMiddleware, ITransientDependency
{
    private readonly TokenAspNetCoreAuditingOptions _tokenAspNetCoreAuditingOptions;
    private readonly TokenAuditingOptions _tokenAuditingOptions;

    public AuditingMiddleware(IOptions<TokenAspNetCoreAuditingOptions> tokenAspNetCoreAuditingOptions,
        IOptions<TokenAuditingOptions> tokenAuditingOptions)
    {
        _tokenAspNetCoreAuditingOptions = tokenAspNetCoreAuditingOptions.Value;
        _tokenAuditingOptions = tokenAuditingOptions.Value;
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
                _tokenAuditingOptions.StartAuditing(new AuditingHttp(_tokenAuditingOptions.ApplicationName,
                    context.Request.Path, context.Request.QueryString));
                await next(context).ConfigureAwait(false);
                _tokenAuditingOptions.EndAuditing(new AuditingHttp(_tokenAuditingOptions.ApplicationName,
                    context.Request.Path, context.Request.QueryString, startNew.Elapsed.TotalMilliseconds));
            }
            catch (Exception ex)
            {
                _tokenAuditingOptions.ErrorAuditing(new ErrorAuditing(_tokenAuditingOptions.ApplicationName,
                    context.Request.Path, context.Request.QueryString, ex, startNew.Elapsed.TotalMilliseconds));
            }
        }
    }

    private bool IsIgnoredUrl(HttpContext context) => context.Request.Path.Value != null &&
                                                      _tokenAspNetCoreAuditingOptions.IgnoredUrls.Any(x => context
                                                          .Request.Path.Value.StartsWith(x));
}