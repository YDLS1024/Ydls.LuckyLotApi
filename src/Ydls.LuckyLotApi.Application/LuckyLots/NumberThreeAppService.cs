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

[Authorize(LuckyLotApiPermissions.NumberThree.Default)]
public class NumberThreeAppService : LuckyLotApiAppService, INumberThreeAppService
{
    private readonly IRepository<NumberThree, Guid> _repository;
    private readonly NumberThreeMapper _mapper;

    public NumberThreeAppService(
        IRepository<NumberThree, Guid> repository,
        NumberThreeMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [AllowAnonymous]
    public async Task<NumberThreeDto> GetAsync(Guid id)
    {
        var entity = await _repository.GetAsync(id);
        return _mapper.MapToDto(entity);
    }

    [AllowAnonymous]
    public async Task<PagedResultDto<NumberThreeDto>> GetListAsync(GetNumberThreeListInput input)
    {
        var queryable = await _repository.GetQueryableAsync();

        if (input.OpenDateMin.HasValue)
        {
            queryable = queryable.Where(x => x.OpenDate >= input.OpenDateMin.Value);
        }

        if (input.OpenDateMax.HasValue)
        {
            queryable = queryable.Where(x => x.OpenDate <= input.OpenDateMax.Value);
        }

        var sorting = string.IsNullOrWhiteSpace(input.Sorting) ? "OpenDate DESC" : input.Sorting;
        queryable = queryable.OrderBy(sorting);

        var totalCount = await AsyncExecuter.CountAsync(queryable);
        var items = await AsyncExecuter.ToListAsync(
            queryable.PageBy(input.SkipCount, input.MaxResultCount));

        return new PagedResultDto<NumberThreeDto>(
            totalCount,
            items.Select(_mapper.MapToDto).ToList());
    }

    [Authorize(LuckyLotApiPermissions.NumberThree.Create)]
    public async Task<NumberThreeDto> CreateAsync(CreateNumberThreeDto input)
    {
        ValidateDigits(input.One, input.Two, input.Three);

        var entity = _mapper.MapToEntity(input);
        entity = await _repository.InsertAsync(entity, autoSave: true);
        return _mapper.MapToDto(entity);
    }

    [Authorize(LuckyLotApiPermissions.NumberThree.Edit)]
    public async Task<NumberThreeDto> UpdateAsync(Guid id, UpdateNumberThreeDto input)
    {
        ValidateDigits(input.One, input.Two, input.Three);

        var entity = await _repository.GetAsync(id);
        _mapper.Map(input, entity);
        entity = await _repository.UpdateAsync(entity, autoSave: true);
        return _mapper.MapToDto(entity);
    }

    [Authorize(LuckyLotApiPermissions.NumberThree.Delete)]
    public async Task DeleteAsync(Guid id)
    {
        await _repository.DeleteAsync(id);
    }

    private static void ValidateDigits(params short[] digits)
    {
        foreach (var digit in digits)
        {
            if (digit < LuckyLotsConsts.MinDigit || digit > LuckyLotsConsts.MaxDigit)
            {
                throw new Volo.Abp.BusinessException("LuckyLotApi:InvalidLotteryDigit")
                    .WithData("Digit", digit);
            }
        }
    }
}
