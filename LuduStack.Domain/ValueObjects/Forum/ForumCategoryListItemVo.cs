using System;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumCategoryListItemVo
    {
        public Guid GroupId { get; set; }

        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public string UserHandler { get; set; }

        public DateTime CreateDate { get; set; }

        public string Handler { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string FeaturedImage { get; set; }

        public string Icon { get; set; }

        public int TopicCount { get; set; }

        public int PostCount { get; set; }

        public LatestForumPostResultVo LatestForumPost { get; set; }
    }
}