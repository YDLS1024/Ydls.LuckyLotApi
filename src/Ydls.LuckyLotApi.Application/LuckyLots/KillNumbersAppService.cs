using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Ydls.LuckyLotApi.Permissions;

namespace Ydls.LuckyLotApi.LuckyLots;

[Authorize(LuckyLotApiPermissions.KillNumbers.Default)]
public class KillNumbersAppService : LuckyLotApiAppService, IKillNumbersAppService
{
    private readonly IRepository<KillNumbers, Guid> _repository;
    private readonly IRepository<Experts, Guid> _expertsRepository;
    private readonly KillNumbersMapper _mapper;
    private readonly KillNumberEvaluationService _evaluationService;

    public KillNumbersAppService(
        IRepository<KillNumbers, Guid> repository,
        IRepository<Experts, Guid> expertsRepository,
        KillNumbersMapper mapper,
        KillNumberEvaluationService evaluationService)
    {
        _repository = repository;
        _expertsRepository = expertsRepository;
        _mapper = mapper;
        _evaluationService = evaluationService;
    }

    [AllowAnonymous]
    public async Task<KillNumbersDto> GetAsync(Guid id)
    {
        var entity = await FindWithExpertAsync(id);
        return await MapToDtoAsync(entity);
    }

    [AllowAnonymous]
    public async Task<PagedResultDto<KillNumbersDto>> GetListAsync(GetKillNumbersListInput input)
    {
        var queryable = await _repository.WithDetailsAsync(x => x.Expert);

        if (input.ExpertId.HasValue)
        {
            queryable = queryable.Where(x => x.ExpertId == input.ExpertId.Value);
        }

        if (input.KillDateMin.HasValue)
        {
            queryable = queryable.Where(x => x.KillDate >= input.KillDateMin.Value);
        }

        if (input.KillDateMax.HasValue)
        {
            queryable = queryable.Where(x => x.KillDate <= input.KillDateMax.Value);
        }

        var sorting = string.IsNullOrWhiteSpace(input.Sorting) ? "KillDate DESC" : input.Sorting;
        queryable = queryable.OrderBy(sorting);

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.PageBy(input.SkipCount, input.MaxResultCount));

        var dtos = new KillNumbersDto[items.Count];
        for (var i = 0; i < items.Count; i++)
        {
            dtos[i] = await MapToDtoAsync(items[i]);
        }

        return new PagedResultDto<KillNumbersDto>(totalCount, dtos);
    }

    [Authorize(LuckyLotApiPermissions.KillNumbers.Create)]
    public async Task<KillNumbersDto> CreateAsync(CreateKillNumbersDto input)
    {
        ValidateKillNumbers(input.KillNumber);
        await _expertsRepository.GetAsync(input.ExpertId);

        var entity = _mapper.MapToEntity(input);
        // Hit status is derived from draws, not client input.
        entity.IsTrue = null;
        await _evaluationService.EvaluateKillEntryIfDrawExistsAsync(entity);
        entity = await _repository.InsertAsync(entity, autoSave: true);
        await _evaluationService.RecalculateWinningRateAsync(entity.ExpertId);

        return await MapToDtoAsync(await FindWithExpertAsync(entity.Id));
    }

    [Authorize(LuckyLotApiPermissions.KillNumbers.Edit)]
    public async Task<KillNumbersDto> UpdateAsync(Guid id, UpdateKillNumbersDto input)
    {
        ValidateKillNumbers(input.KillNumber);
        await _expertsRepository.GetAsync(input.ExpertId);

        var entity = await _repository.GetAsync(id);
        var previousExpertId = entity.ExpertId;

        _mapper.Map(input, entity);
        await _evaluationService.EvaluateKillEntryIfDrawExistsAsync(entity);
        entity = await _repository.UpdateAsync(entity, autoSave: true);

        await _evaluationService.RecalculateWinningRateAsync(entity.ExpertId);
        if (previousExpertId != entity.ExpertId)
        {
            await _evaluationService.RecalculateWinningRateAsync(previousExpertId);
        }

        return await MapToDtoAsync(await FindWithExpertAsync(entity.Id));
    }

    [Authorize(LuckyLotApiPermissions.KillNumbers.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        var expertId = entity.ExpertId;
        await _repository.DeleteAsync(entity, autoSave: true);
        await _evaluationService.RecalculateWinningRateAsync(expertId);
    }

    private async Task<KillNumbers> FindWithExpertAsync(Guid id)
    {
        var queryable = await _repository.WithDetailsAsync(x => x.Expert);
        var entity = await AsyncExecuter.FirstOrDefaultAsync(queryable.Where(x => x.Id == id));
        if (entity == null)
        {
            throw new EntityNotFoundException(typeof(KillNumbers), id);
        }

        return entity;
    }

    private Task<KillNumbersDto> MapToDtoAsync(KillNumbers entity)
    {
        return Task.FromResult(new KillNumbersDto
        {
            Id = entity.Id,
            KillDate = entity.KillDate,
            KillNumber = entity.KillNumber,
            IsTrue = entity.IsTrue,
            ExpertId = entity.ExpertId,
            ExpertNickname = entity.Expert?.Nickname
        });
    }

    private static void ValidateKillNumbers(short[] numbers)
    {
        if (numbers.Length == 0 || numbers.Length > LuckyLotsConsts.MaxKillNumbersPerEntry)
        {
            throw new Volo.Abp.BusinessException("LuckyLotApi:InvalidKillNumberCount")
                .WithData("Count", numbers.Length);
        }

        if (numbers.Distinct().Count() != numbers.Length)
        {
            throw new Volo.Abp.BusinessException("LuckyLotApi:DuplicateKillNumbers");
        }

        foreach (var number in numbers)
        {
            if (number < LuckyLotsConsts.MinDigit || number > LuckyLotsConsts.MaxDigit)
            {
                throw new Volo.Abp.BusinessException("LuckyLotApi:InvalidLotteryDigit")
                    .WithData("Digit", number);
            }
        }
    }
}
