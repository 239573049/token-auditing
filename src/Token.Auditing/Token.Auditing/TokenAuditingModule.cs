using Token.Auditing.Extensions;
using Token.Module;

namespace Token.Auditing;

public class TokenAuditingModule : TokenModule
{
    public override void OnApplicationShutdown(Microsoft.AspNetCore.Builder.IApplicationBuilder app)
    {
        app.UseAuditing();
    }
}