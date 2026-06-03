using Volo.Abp.Modularity;

namespace Ydls.LuckyLotApi;

[DependsOn(
    typeof(LuckyLotApiDomainModule),
    typeof(LuckyLotApiTestBaseModule)
)]
public class LuckyLotApiDomainTestModule : AbpModule
{

}
