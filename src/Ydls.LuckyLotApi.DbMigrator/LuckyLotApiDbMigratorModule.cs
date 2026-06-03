using Ydls.LuckyLotApi.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace Ydls.LuckyLotApi.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(LuckyLotApiEntityFrameworkCoreModule),
    typeof(LuckyLotApiApplicationContractsModule)
)]
public class LuckyLotApiDbMigratorModule : AbpModule
{
}
