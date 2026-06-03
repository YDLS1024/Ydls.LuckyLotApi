using System;
using Volo.Abp.Domain.Entities;

namespace Ydls.LuckyLotApi.LuckyLots
{
    public class KillNumbers:Entity<Guid>
    {
        public DateTime KillDate { get; set; }

        public short[] KillNumber { get; set; }

        public bool? IsTrue { get; set; }

        public Experts Expert { get; set; }
    }
}