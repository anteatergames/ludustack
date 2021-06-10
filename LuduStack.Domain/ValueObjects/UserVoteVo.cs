using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Domain.ValueObjects
{
    public class UserVoteVo
    {
        public Guid UserId { get; set; }

        public DateTime CreateDate { get; set; }

        public VoteValue VoteValue { get; set; }
    }
}