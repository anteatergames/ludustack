using LuduStack.Domain.Core.Enums;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.ValueObjects
{
    public class ForumPostListItemVo
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreatedRelativeTime { get; set; }

        public bool IsFixed { get; set; }

        public Guid ForumCategoryId { get; set; }

        public string Title { get; set; }

        public SupportedLanguage Language { get; set; }

        public virtual List<UserVoteVo> Votes { get; set; }

        public string AuthorPicture { get; set; }
        public string AuthorName { get; set; }
        public string UserHandler { get; set; }
        public int ReplyCount { get; set; }
        public int ViewCount { get; set; }
        public LatestForumPostResultVo LatestReply { get; set; }
    }
}