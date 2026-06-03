using Xunit;

namespace Ydls.LuckyLotApi.EntityFrameworkCore;

[CollectionDefinition(LuckyLotApiTestConsts.CollectionDefinitionName)]
public class LuckyLotApiEntityFrameworkCoreCollection : ICollectionFixture<LuckyLotApiEntityFrameworkCoreFixture>
{

}
