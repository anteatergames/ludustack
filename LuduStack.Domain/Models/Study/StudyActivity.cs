using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class StudyActivity : Entity
    {
        public Guid ActivityId { get; set; }

        public int Order { get; set; }
    }
}