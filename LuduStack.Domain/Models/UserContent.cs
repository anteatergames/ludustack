using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Core.Models;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;

namespace LuduStack.Domain.Models
{
    public class UserContent : Entity
    {
        public string FeaturedImage { get; set; }

        public List<MediaListItemVo> Media { get; set; }

        public int? IssueNumber { get; set; }

        public Guid? SeriesId { get; set; }

        public string Title { get; set; }

        public string Introduction { get; set; }

        public string Content { get; set; }

        public UserContentType UserContentType { get; set; }

        public SupportedLanguage Language { get; set; }

        public Guid? GameId { get; set; }
        public virtual Game Game { get; set; }

        public virtual List<UserContentLike> Likes { get; set; }

        public virtual List<UserContentComment> Comments { get; set; }

        public virtual List<Poll> Polls { get; set; }

        public virtual List<UserContentRating> Ratings { get; set; }

        public UserContent()
        {
            Ratings = new List<UserContentRating>();
        }
    }
}