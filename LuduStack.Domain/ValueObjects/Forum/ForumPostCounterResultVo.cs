using System;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumPostCounterResultVo
    {
        public Guid OriginalPostId { get; set; }

        public int ReplyCount { get; set; }

        public int ViewCount { get; set; }

        public LatestForumPostResultVo LatestReply { get; set; }
    }
}