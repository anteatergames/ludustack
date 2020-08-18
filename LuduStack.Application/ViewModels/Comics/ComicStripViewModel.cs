using LuduStack.Application.Interfaces;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Models;
using LuduStack.Domain.ValueObjects;
using Org.BouncyCastle.Bcpg;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LuduStack.Application.ViewModels.Comics
{
    public class ComicStripViewModel : UserGeneratedBaseViewModel, ICommentableItem
    {
        [Required]
        public int IssueNumber { get; set; }

        [Required]
        public Guid SeriesId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }

        public DateTime PublishDate { get; set; }

        [Required]
        public string FeaturedImage { get; set; }

        public string FeaturedImageLquip { get; set; }

        public string FeaturedImageResponsive { get; set; }

        public List<ImageListItemVo> Images { get; set; }

        public SupportedLanguage Language { get; set; }

        public List<Guid> Likes { get; set; }

        public int LikeCount { get; set; }

        #region ICommentableItem
        public string Url { get; set; }

        public List<CommentViewModel> Comments { get; set; }

        public int CommentCount { get; set; }
        #endregion ICommentableItem

        public decimal TotalRating { get; set; }

        public decimal CurrentUserRating { get; set; }

        public ComicStripViewModel()
        {
            UserContentType = UserContentType.ComicStrip;
            Images = new List<ImageListItemVo>();
        }
    }
}
