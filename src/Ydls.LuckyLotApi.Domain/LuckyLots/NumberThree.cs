using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;

namespace Ydls.LuckyLotApi.LuckyLots
{
    public class NumberThree:FullAuditedAggregateRoot<Guid>
    {
        public DateTime OpenDate { get; set; }

        public short One {  get; set; }

        public short Two { get; set; }

        public short Three { get; set; }
    }
}
