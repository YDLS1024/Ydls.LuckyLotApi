using System;
using Volo.Abp.Application.Dtos;

namespace Ydls.LuckyLotApi.LuckyLots;

public class GetNumberThreeListInput : PagedAndSortedResultRequestDto
{
    public DateTime? OpenDateMin { get; set; }

    public DateTime? OpenDateMax { get; set; }
}
