using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ydls.LuckyLotApi.LuckyLots;

public interface IKillNumbersAppService : IApplicationService
{
    Task<KillNumbersDto> GetAsync(Guid id);

    Task<PagedResultDto<KillNumbersDto>> GetListAsync(GetKillNumbersListInput input);

    Task<KillNumbersDto> CreateAsync(CreateKillNumbersDto input);

    Task<KillNumbersDto> UpdateAsync(Guid id, UpdateKillNumbersDto input);

    Task DeleteAsync(Guid id);
}
