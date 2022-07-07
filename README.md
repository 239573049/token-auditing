# TokenModule

[![NuGet](https://img.shields.io/nuget/dt/Token.Auditing/.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/Token.Auditing//)
[![NuGet](https://img.shields.io/nuget/v/Token.Auditing/.svg?label=NuGet&style=flat&logo=nuget)](https://www.nuget.org/packages/Token.Auditing//)

## 介绍

TokenModule依赖审计日志
目前不支持直接数据库保存
只控制台显示

## 使用教程

```csharp
// 在Module中使用
[DependOn(
             typeof(TokenAuditingModule)
             )]
// 可扩展委托 

    public override void ConfigureServices(IServiceCollection services)
    {
        Configure<TokenAuditingOptions>(x =>
        {
            // 是否启动审计日志
            x.IsEnabled = true;
            // 当前服务名称
            x.ApplicationName = nameof(NetCoreHostModule);
            // 请求前返回参数
            x.StartAuditing = (data) => { };
            // 正常请求结束返回的参数
            x.EndAuditing = (date) => { };
            // 异常结束返回的参数
            x.ErrorAuditing = (data) => { };
        });
    }
```