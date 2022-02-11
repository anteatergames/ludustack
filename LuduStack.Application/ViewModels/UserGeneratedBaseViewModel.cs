using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels
{
    public abstract class UserGeneratedBaseViewModel : BaseViewModel, IUserGeneratedContent
    {
        [Display(Name = "Author Picture")]
        public string AuthorPicture { get; set; }

        [Display(Name = "Author Name")]
        public string AuthorName { get; set; }

        public UserContentType UserContentType { get; set; }

        [Display(Name = "Url")]
        [RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?", ErrorMessage = "Must be a valid URL")]
        public string Url { get; set; }

        public string ShareUrl { get; set; }

        public string ShareText { get; set; }
    }

    public abstract class UserGeneratedCommentBaseViewModel : UserGeneratedBaseViewModel
    {
        public List<CommentViewModel> Comments { get; set; }

        public List<Guid> Likes { get; set; }

        [Display(Name = "Language")]
        public SupportedLanguage Language { get; set; }

        private int likeCount;

        public int LikeCount
        {
            get => likeCount == 0 ? Likes.Count : likeCount;
            set => likeCount = value;
        }

        private int commentCount;

        [Display(Name = "Comment Count")]
        public int CommentCount
        {
            get => commentCount == 0 ? Comments.Count : commentCount;
            set => commentCount = value;
        }

        protected UserGeneratedCommentBaseViewModel()
        {
            Comments = new List<CommentViewModel>();
            Likes = new List<Guid>();
        }
    }
}