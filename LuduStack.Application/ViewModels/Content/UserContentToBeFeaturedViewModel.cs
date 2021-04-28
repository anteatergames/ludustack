using LuduStack.Domain.Core.Enums;
using System;

namespace LuduStack.Application.ViewModels.Content
{
    public class UserContentToBeFeaturedViewModel : UserGeneratedBaseViewModel
    {
        public string FeaturedImage { get; set; }

        public string Title { get; set; }

        public string Introduction { get; set; }

        public string Content { get; set; }

        public SupportedLanguage Language { get; set; }

        public int LikeCount { get; set; }

        public int CommentCount { get; set; }

        public Guid GameId { get; set; }
        public string GameName { get; set; }

        public bool IsArticle { get; set; }

        public bool TitleCompliant { get; set; }

        public int TitleLength { get; set; }

        public bool IntroCompliant { get; set; }

        public int IntroLength { get; set; }

        public bool ContentCompliant { get; set; }

        public int ContentLength { get; set; }

        public bool IsFeatured { get; set; }

        public Guid? CurrentFeatureId { get; set; }

        public bool HasFeaturedImage { get; set; }

        public int Score { get; set; }
    }
}