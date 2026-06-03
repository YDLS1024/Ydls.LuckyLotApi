using Volo.Abp.Modularity;

namespace Ydls.LuckyLotApi;

[DependsOn(
    typeof(LuckyLotApiApplicationModule),
    typeof(LuckyLotApiDomainTestModule)
)]
public class LuckyLotApiApplicationTestModule : AbpModule
{

}
