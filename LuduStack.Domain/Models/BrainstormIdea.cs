using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class BrainstormIdea : Entity
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public Guid SessionId { get; set; }

        public BrainstormIdeaStatus Status { get; set; }

        public virtual BrainstormSession Session { get; set; }

        public virtual List<BrainstormComment> Comments { get; set; }

        public virtual List<UserVoteVo> Votes { get; set; }
    }
}