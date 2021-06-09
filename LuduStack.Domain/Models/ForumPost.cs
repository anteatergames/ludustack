using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class ForumPost : Entity
    {
        public bool IsOriginalPost { get; set; }

        public bool IsFixed { get; set; }

        public bool IsReported { get; set; }

        public Guid OriginalPostId { get; set; }

        public Guid? ReplyPostId { get; set; }

        public Guid? ReplyUserId { get; set; }

        public Guid ForumCategoryId { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Content { get; set; }

        public SupportedLanguage Language { get; set; }

        public List<MediaListItemVo> Media { get; set; }

        public virtual List<UserViewVo> Views { get; set; }

        public virtual List<Poll> Polls { get; set; }

        public virtual List<UserVoteVo> Votes { get; set; }
    }
}