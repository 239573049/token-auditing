using Token.Module;
using Token.Auditing;
using Token.Module.Attributes;

namespace NetCore.Host;

[DependOn(
    typeof(TokenAuditingModule)
)]
public class NetCoreHostModule : TokenModule
{
    public override void ConfigureServices(IServiceCollection services)
    {
        Configure<TokenAuditingOptions>(x =>
        {
            x.IsEnabled = true;
            x.ApplicationName = nameof(NetCoreHostModule);
            x.StartAuditing = (data) => { };
            x.EndAuditing = (date) => { };
            x.ErrorAuditing = (data) => { };
        });
    }
}