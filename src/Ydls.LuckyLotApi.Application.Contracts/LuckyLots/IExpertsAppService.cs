using System;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Ydls.LuckyLotApi.LuckyLots;

public interface IExpertsAppService : IApplicationService
{
    Task<ExpertsDto> GetAsync(Guid id);

    Task<PagedResultDto<ExpertsDto>> GetListAsync(GetExpertsListInput input);

    Task<ExpertsDto> CreateAsync(CreateExpertsDto input);

    Task<ExpertsDto> UpdateAsync(Guid id, UpdateExpertsDto input);

    Task DeleteAsync(Guid id);
}
