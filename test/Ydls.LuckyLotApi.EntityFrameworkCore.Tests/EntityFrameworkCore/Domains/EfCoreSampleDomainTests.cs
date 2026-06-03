using Ydls.LuckyLotApi.Samples;
using Xunit;

namespace Ydls.LuckyLotApi.EntityFrameworkCore.Domains;

[Collection(LuckyLotApiTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<LuckyLotApiEntityFrameworkCoreTestModule>
{

}
