using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ydls.LuckyLotApi.LuckyLots;

public interface INumberThreeAppService : IApplicationService
{
    Task<NumberThreeDto> GetAsync(Guid id);

    Task<PagedResultDto<NumberThreeDto>> GetListAsync(GetNumberThreeListInput input);

    Task<NumberThreeDto> CreateAsync(CreateNumberThreeDto input);

    Task<NumberThreeDto> UpdateAsync(Guid id, UpdateNumberThreeDto input);

    Task DeleteAsync(Guid id);
}
