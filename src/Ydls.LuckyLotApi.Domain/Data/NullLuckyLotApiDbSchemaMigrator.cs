using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Ydls.LuckyLotApi.Data;

/* This is used if database provider does't define
 * ILuckyLotApiDbSchemaMigrator implementation.
 */
public class NullLuckyLotApiDbSchemaMigrator : ILuckyLotApiDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
