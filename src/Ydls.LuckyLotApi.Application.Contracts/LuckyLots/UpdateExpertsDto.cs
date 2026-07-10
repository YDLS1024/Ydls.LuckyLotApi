using System.ComponentModel.DataAnnotations;

namespace Ydls.LuckyLotApi.LuckyLots;

public class UpdateExpertsDto
{
    [Required]
    [StringLength(LuckyLotsConsts.MaxNicknameLength)]
    public string Nickname { get; set; } = null!;

    [Range(0, 100)]
    public double? WinningRate { get; set; }
}
