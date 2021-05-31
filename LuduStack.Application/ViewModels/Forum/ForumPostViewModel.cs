using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Forum
{
    public class ForumPostViewModel : UserGeneratedBaseViewModel
    {
        public bool IsOriginalPost { get; set; }

        public bool IsFixed { get; set; }

        public bool IsReported { get; set; }

        public Guid OriginalPostId { get; set; }

        public Guid? ReplyPostId { get; set; }

        public Guid ForumCategoryId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Slug { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public SupportedLanguage Language { get; set; }

        public List<MediaListItemVo> Media { get; set; }

        public virtual List<UserContentLike> Likes { get; set; }

        public virtual List<UserViewVo> Views { get; set; }

        public virtual List<PollVote> Polls { get; set; }

        public string CategoryHandler { get; set; }

        public string CategoryName { get; internal set; }

        public bool IsEditting { get; set; }
    }
}