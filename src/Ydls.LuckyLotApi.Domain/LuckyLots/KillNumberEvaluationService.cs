using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace Ydls.LuckyLotApi.LuckyLots;

/// <summary>
/// Evaluates kill-number hits against draws and recalculates expert winning rates.
/// A kill entry is correct when none of its digits appear in the draw (full avoid).
/// </summary>
public class KillNumberEvaluationService : DomainService
{
    private readonly IRepository<KillNumbers, Guid> _killNumbersRepository;
    private readonly IRepository<Experts, Guid> _expertsRepository;
    private readonly IRepository<NumberThree, Guid> _numberThreeRepository;

    public KillNumberEvaluationService(
        IRepository<KillNumbers, Guid> killNumbersRepository,
        IRepository<Experts, Guid> expertsRepository,
        IRepository<NumberThree, Guid> numberThreeRepository)
    {
        _killNumbersRepository = killNumbersRepository;
        _expertsRepository = expertsRepository;
        _numberThreeRepository = numberThreeRepository;
    }

    public async Task EvaluateForDrawAsync(DateTime openDate, short one, short two, short three)
    {
        var dayStart = openDate.Date;
        var dayEnd = dayStart.AddDays(1);

        var kills = await _killNumbersRepository.GetListAsync(
            x => x.KillDate >= dayStart && x.KillDate < dayEnd);

        if (kills.Count == 0)
        {
            return;
        }

        var drawDigits = new HashSet<short> { one, two, three };
        var expertIds = new HashSet<Guid>();

        foreach (var kill in kills)
        {
            kill.IsTrue = IsFullyCorrect(kill.KillNumber, drawDigits);
            await _killNumbersRepository.UpdateAsync(kill);
            expertIds.Add(kill.ExpertId);
        }

        foreach (var expertId in expertIds)
        {
            await RecalculateWinningRateAsync(expertId);
        }
    }

    public async Task ClearEvaluationForDateAsync(DateTime openDate)
    {
        var dayStart = openDate.Date;
        var dayEnd = dayStart.AddDays(1);

        var kills = await _killNumbersRepository.GetListAsync(
            x => x.KillDate >= dayStart && x.KillDate < dayEnd);

        if (kills.Count == 0)
        {
            return;
        }

        var expertIds = new HashSet<Guid>();
        foreach (var kill in kills)
        {
            kill.IsTrue = null;
            await _killNumbersRepository.UpdateAsync(kill);
            expertIds.Add(kill.ExpertId);
        }

        foreach (var expertId in expertIds)
        {
            await RecalculateWinningRateAsync(expertId);
        }
    }

    public async Task EvaluateKillEntryIfDrawExistsAsync(KillNumbers kill)
    {
        var dayStart = kill.KillDate.Date;
        var dayEnd = dayStart.AddDays(1);

        var draw = await _numberThreeRepository.FirstOrDefaultAsync(
            x => x.OpenDate >= dayStart && x.OpenDate < dayEnd);

        if (draw == null)
        {
            kill.IsTrue = null;
            return;
        }

        var drawDigits = new HashSet<short> { draw.One, draw.Two, draw.Three };
        kill.IsTrue = IsFullyCorrect(kill.KillNumber, drawDigits);
    }

    public async Task RecalculateWinningRateAsync(Guid expertId)
    {
        var expert = await _expertsRepository.FindAsync(expertId);
        if (expert == null)
        {
            return;
        }

        var kills = await _killNumbersRepository.GetListAsync(x => x.ExpertId == expertId);
        var settled = kills.Where(x => x.IsTrue.HasValue).ToList();

        if (settled.Count == 0)
        {
            expert.WinningRate = null;
        }
        else
        {
            var hits = settled.Count(x => x.IsTrue == true);
            expert.WinningRate = Math.Round(hits * 100.0 / settled.Count, 1);
        }

        await _expertsRepository.UpdateAsync(expert);
    }

    /// <summary>
    /// Full correct: none of the kill digits appear in the draw.
    /// </summary>
    public static bool IsFullyCorrect(short[]? killNumbers, ISet<short> drawDigits)
    {
        if (killNumbers == null || killNumbers.Length == 0)
        {
            return false;
        }

        return killNumbers.All(n => !drawDigits.Contains(n));
    }
}
