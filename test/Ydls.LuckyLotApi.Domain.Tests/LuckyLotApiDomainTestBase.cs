using Volo.Abp.Modularity;

namespace Ydls.LuckyLotApi;

/* Inherit from this class for your domain layer tests. */
public abstract class LuckyLotApiDomainTestBase<TStartupModule> : LuckyLotApiTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
