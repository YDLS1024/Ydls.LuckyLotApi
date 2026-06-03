using Ydls.LuckyLotApi.Samples;
using Xunit;

namespace Ydls.LuckyLotApi.EntityFrameworkCore.Applications;

[Collection(LuckyLotApiTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<LuckyLotApiEntityFrameworkCoreTestModule>
{

}
