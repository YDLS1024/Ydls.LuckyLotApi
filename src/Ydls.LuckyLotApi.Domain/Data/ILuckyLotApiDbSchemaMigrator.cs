using System.Threading.Tasks;

namespace Ydls.LuckyLotApi.Data;

public interface ILuckyLotApiDbSchemaMigrator
{
    Task MigrateAsync();
}
