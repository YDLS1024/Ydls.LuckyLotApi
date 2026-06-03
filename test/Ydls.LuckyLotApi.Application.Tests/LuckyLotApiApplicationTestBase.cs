using Volo.Abp.Modularity;

namespace Ydls.LuckyLotApi;

public abstract class LuckyLotApiApplicationTestBase<TStartupModule> : LuckyLotApiTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
