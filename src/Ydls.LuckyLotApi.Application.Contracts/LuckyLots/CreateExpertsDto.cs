using System.ComponentModel.DataAnnotations;

namespace Ydls.LuckyLotApi.LuckyLots;

public class CreateExpertsDto
{
    [Required]
    [StringLength(LuckyLotsConsts.MaxNicknameLength)]
    public string Nickname { get; set; } = null!;

    [Range(0, 100)]
    public double? WinningRate { get; set; }
}
