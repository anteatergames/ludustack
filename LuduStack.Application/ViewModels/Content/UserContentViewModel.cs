using LuduStack.Application.Interfaces;
using LuduStack.Application.ViewModels.Poll;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace LuduStack.Application.ViewModels.Content
{
    public class UserContentViewModel : UserGeneratedCommentBaseViewModel, ICommentableItem
    {
        public DateTime PublishDate { get; set; }

        [Display(Name = "Featured Image")]
        public string FeaturedImage { get; set; }

        public string FeaturedImageResponsive { get; set; }
        public string FeaturedImageLquip { get; set; }

        public MediaType? FeaturedMediaType { get; set; }

        [Display(Name = "Images")]
        public List<MediaListItemVo> Media { get; set; }

        [StringLength(128)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Introduction")]
        public string Introduction { get; set; }

        [Display(Name = "Content")]
        public string Content { get; set; }

        [Display(Name = "Related Game")]
        public Guid? GameId { get; set; }

        public string GameTitle { get; set; }

        public string GameThumbnail { get; set; }

        public bool HasFeaturedImage { get; set; }

        public bool IsComplex => HasFeaturedImage;

        public bool HasPoll => Poll != null && Poll.PollOptions.Any();

        public PollViewModel Poll { get; set; }

        public bool IsArticle { get; set; }

        public string NanoGaleryJson { get; set; }
    }
}