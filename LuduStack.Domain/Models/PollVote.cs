using LuduStack.Domain.Core.Models;
using System;

namespace LuduStack.Domain.Models
{
    public class PollVote : Entity
    {
        public Guid PollId { get; set; }

        public Guid PollOptionId { get; set; }
    }
}