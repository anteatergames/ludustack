using System;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumCategoryCounterResultVo
    {
        public Guid ForumCategoryId { get; set; }

        public int TopicsCount { get; set; }

        public int PostsCount { get; set; }

        public LatestForumPostResultVo LatestPost { get; set; }
    }
}