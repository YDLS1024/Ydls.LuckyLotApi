using System;
using System.ComponentModel.DataAnnotations;

namespace Ydls.LuckyLotApi.LuckyLots;

public class CreateKillNumbersDto
{
    [Required]
    public DateTime KillDate { get; set; }

    [Required]
    [MinLength(1)]
    public short[] KillNumber { get; set; } = null!;

    public bool? IsTrue { get; set; }

    [Required]
    public Guid ExpertId { get; set; }
}
