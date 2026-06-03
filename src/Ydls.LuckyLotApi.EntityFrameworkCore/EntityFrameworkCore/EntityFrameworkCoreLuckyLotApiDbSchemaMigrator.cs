using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Ydls.LuckyLotApi.Data;
using Volo.Abp.DependencyInjection;

namespace Ydls.LuckyLotApi.EntityFrameworkCore;

public class EntityFrameworkCoreLuckyLotApiDbSchemaMigrator
    : ILuckyLotApiDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreLuckyLotApiDbSchemaMigrator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolving the LuckyLotApiDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<LuckyLotApiDbContext>()
            .Database
            .MigrateAsync();
    }
}
