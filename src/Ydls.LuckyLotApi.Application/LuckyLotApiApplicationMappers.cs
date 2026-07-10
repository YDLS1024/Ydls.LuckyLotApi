using Riok.Mapperly.Abstractions;
using Ydls.LuckyLotApi.LuckyLots;

namespace Ydls.LuckyLotApi;

[Mapper]
public partial class NumberThreeMapper
{
    public partial NumberThreeDto MapToDto(NumberThree source);

    public partial NumberThree MapToEntity(CreateNumberThreeDto source);

    public partial void Map(UpdateNumberThreeDto source, NumberThree destination);
}

[Mapper]
public partial class ExpertsMapper
{
    public partial ExpertsDto MapToDto(Experts source);

    public partial Experts MapToEntity(CreateExpertsDto source);

    public partial void Map(UpdateExpertsDto source, Experts destination);
}

[Mapper]
public partial class KillNumbersMapper
{
    public partial KillNumbersDto MapToDto(KillNumbers source);

    public partial KillNumbers MapToEntity(CreateKillNumbersDto source);

    public partial void Map(UpdateKillNumbersDto source, KillNumbers destination);
}
