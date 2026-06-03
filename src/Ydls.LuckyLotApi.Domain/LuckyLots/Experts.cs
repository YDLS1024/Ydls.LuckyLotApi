using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace Ydls.LuckyLotApi.LuckyLots
{
    /// <summary>
    /// 专家
    /// </summary>
    public class Experts:Entity<Guid>
    {
        public string Nickname { get; set; }

        public double? WinningRate { get; set; }

        public List<KillNumbers> KillNumbers { get; set; }
    }
}
