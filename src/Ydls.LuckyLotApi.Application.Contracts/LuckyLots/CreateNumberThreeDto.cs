using System;
using System.ComponentModel.DataAnnotations;

namespace Ydls.LuckyLotApi.LuckyLots;

public class CreateNumberThreeDto
{
    [Required]
    public DateTime OpenDate { get; set; }

    [Range(LuckyLotsConsts.MinDigit, LuckyLotsConsts.MaxDigit)]
    public short One { get; set; }

    [Range(LuckyLotsConsts.MinDigit, LuckyLotsConsts.MaxDigit)]
    public short Two { get; set; }

    [Range(LuckyLotsConsts.MinDigit, LuckyLotsConsts.MaxDigit)]
    public short Three { get; set; }
}
