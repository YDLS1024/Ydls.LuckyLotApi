using System;
using Volo.Abp.Application.Dtos;

namespace Ydls.LuckyLotApi.LuckyLots;

public class ExpertsDto : EntityDto<Guid>
{
    public string Nickname { get; set; } = null!;

    public double? WinningRate { get; set; }

    public int KillCount { get; set; }

    public int HitCount { get; set; }
}
