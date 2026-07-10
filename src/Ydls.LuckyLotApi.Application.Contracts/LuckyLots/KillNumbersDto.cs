using System;
using Volo.Abp.Application.Dtos;

namespace Ydls.LuckyLotApi.LuckyLots;

public class KillNumbersDto : EntityDto<Guid>
{
    public DateTime KillDate { get; set; }

    public short[] KillNumber { get; set; } = null!;

    public bool? IsTrue { get; set; }

    public Guid ExpertId { get; set; }

    public string? ExpertNickname { get; set; }
}
