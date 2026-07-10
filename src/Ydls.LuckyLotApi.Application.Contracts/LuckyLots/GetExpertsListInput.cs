using Volo.Abp.Application.Dtos;

namespace Ydls.LuckyLotApi.LuckyLots;

public class GetExpertsListInput : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }
}
