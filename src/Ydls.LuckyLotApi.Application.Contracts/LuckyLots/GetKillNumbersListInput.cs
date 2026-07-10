using System;
using Volo.Abp.Application.Dtos;

namespace Ydls.LuckyLotApi.LuckyLots;

public class GetKillNumbersListInput : PagedAndSortedResultRequestDto
{
    public Guid? ExpertId { get; set; }

    public DateTime? KillDateMin { get; set; }

    public DateTime? KillDateMax { get; set; }
}
