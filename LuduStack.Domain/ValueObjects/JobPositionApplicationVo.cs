using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Domain.ValueObjects
{
    public class JobPositionApplicationVo
    {
        public Guid JobPositionId { get; set; }

        public DateTime ApplicationDate { get; set; }

        public WorkType WorkType { get; set; }

        public string Location { get; set; }
    }
}