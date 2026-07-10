using System;
using Volo.Abp.Application.Dtos;

namespace Ydls.LuckyLotApi.LuckyLots;

public class NumberThreeDto : FullAuditedEntityDto<Guid>
{
    public DateTime OpenDate { get; set; }

    public short One { get; set; }

    public short Two { get; set; }

    public short Three { get; set; }
}
