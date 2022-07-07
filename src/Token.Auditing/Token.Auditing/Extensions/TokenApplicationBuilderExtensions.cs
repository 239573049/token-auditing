using Microsoft.AspNetCore.Builder;

namespace Token.Auditing.Extensions;

public static class TokenApplicationBuilderExtensions
{
    public static IApplicationBuilder UseAuditing(this IApplicationBuilder app) =>
        app.UseMiddleware<AuditingMiddleware>();

}