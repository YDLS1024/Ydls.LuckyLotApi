using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Ydls.LuckyLotApi.Permissions;

namespace Ydls.LuckyLotApi.LuckyLots;

[Authorize(LuckyLotApiPermissions.Experts.Default)]
public class ExpertsAppService : LuckyLotApiAppService, IExpertsAppService
{
    private readonly IRepository<Experts, Guid> _repository;
    private readonly IRepository<KillNumbers, Guid> _killNumbersRepository;
    private readonly ExpertsMapper _mapper;

    public ExpertsAppService(
        IRepository<Experts, Guid> repository,
        IRepository<KillNumbers, Guid> killNumbersRepository,
        ExpertsMapper mapper)
    {
        _repository = repository;
        _killNumbersRepository = killNumbersRepository;
        _mapper = mapper;
    }

    [AllowAnonymous]
    public async Task<ExpertsDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return await MapToDtoWithStatsAsync(entity);
    }

    [AllowAnonymous]
    public async Task<PagedResultDto<ExpertsDto>> GetListAsync(GetExpertsListInput input)
    {
        var queryable = await _repository.GetQueryableAsync();

        if (!string.IsNullOrWhiteSpace(input.Filter))
        {
            queryable = queryable.Where(x => x.Nickname.Contains(input.Filter));
        }

        var sorting = string.IsNullOrWhiteSpace(input.Sorting) ? "Nickname" : input.Sorting;
        queryable = queryable.OrderBy(sorting);

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.PageBy(input.SkipCount, input.MaxResultCount));

        var dtos = new List<ExpertsDto>();
        foreach (var item in items)
        {
            dtos.Add(await MapToDtoWithStatsAsync(item));
        }

        return new PagedResultDto<ExpertsDto>(totalCount, dtos);
    }

    [Authorize(LuckyLotApiPermissions.Experts.Create)]
    public async Task<ExpertsDto> CreateAsync(CreateExpertsDto input)
    {
        var entity = _mapper.MapToEntity(input);
        entity.KillNumbers = new List<KillNumbers>();
        entity = await _repository.InsertAsync(entity, autoSave: true);
        return await MapToDtoWithStatsAsync(entity);
    }

    [Authorize(LuckyLotApiPermissions.Experts.Edit)]
    public async Task<ExpertsDto> UpdateAsync(Guid id, UpdateExpertsDto input)
    {
        var entity = await _repository.GetAsync(id);
        _mapper.Map(input, entity);
        entity = await _repository.UpdateAsync(entity, autoSave: true);
        return await MapToDtoWithStatsAsync(entity);
    }

    [Authorize(LuckyLotApiPermissions.Experts.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    private async Task<ExpertsDto> MapToDtoWithStatsAsync(Experts entity)
    {
        var dto = _mapper.MapToDto(entity);
        var killQueryable = await _killNumbersRepository.GetQueryableAsync();
        var kills = await AsyncExecuter.ToListAsync(killQueryable.Where(x => x.ExpertId == entity.Id));
        dto.KillCount = kills.Count;
        dto.HitCount = kills.Count(x => x.IsTrue == true);
        return dto;
    }
}
