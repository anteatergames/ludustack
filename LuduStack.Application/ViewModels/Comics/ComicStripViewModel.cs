using LuduStack.Application.Interfaces;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Comics
{
    public class ComicStripViewModel : UserGeneratedCommentBaseViewModel, ICommentableItem
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

        public List<MediaListItemVo> Media { get; set; }

        public int RatingCount { get; set; }

        public decimal TotalRating { get; set; }

        public decimal CurrentUserRating { get; set; }

        public ComicStripViewModel()
        {
            UserContentType = UserContentType.ComicStrip;
            Media = new List<MediaListItemVo>();
        }
    }
}