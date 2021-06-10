using System;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumPostCounterResultVo
    {
        public Guid OriginalPostId { get; set; }

        public int AnswerCount { get; set; }

        public int ViewCount { get; set; }

        public LatestForumPostResultVo LatestAnswer { get; set; }
    }
}