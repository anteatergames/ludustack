﻿using LuduStack.Application.ViewModels.Team;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Game
{
    public class GameViewModel : UserGeneratedBaseViewModel
    {
        [Required(ErrorMessage = "The Title is Required")]
        [MinLength(2)]
        [MaxLength(100)]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Genre")]
        public GameGenre Genre { get; set; }

        [Display(Name = "Team")]
        public Guid? TeamId { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Cover Image")]
        public string CoverImageUrl { get; set; }

        [Display(Name = "Thumbnail")]
        public string ThumbnailUrl { get; set; }

        [Display(Name = "Engine")]
        public GameEngine Engine { get; set; }

        [Display(Name = "Custom Engine Name")]
        [MaxLength(30, ErrorMessage = "Must have maximum 30 characters")]
        public string CustomEngineName { get; set; }

        [Display(Name = "Main Language")]
        public CodeLanguage Language { get; set; }

        [Display(Name = "Status")]
        public GameStatus Status { get; set; }

        [Display(Name = "Release Date")]
        public DateTime? ReleaseDate { get; set; }

        [Display(Name = "Platforms")]
        public List<GamePlatforms> Platforms { get; set; }

        public List<ExternalLinkBaseViewModel> ExternalLinks { get; set; }

        public List<GameFollowViewModel> Followers { get; set; }

        public TeamViewModel Team { get; set; }

        public List<GameCharacteristicVo> Characteristics { get; set; }

        public List<MediaListItemVo> Media { get; set; }

        #region Counters

        public int FollowerCount { get; set; }
        public int CommentCount { get; set; }
        public int LikeCount { get; set; }
        public double? LocalizationPercentage { get; set; }
        public Guid LocalizationId { get; set; }

        #endregion Counters

        public string NanoGaleryJson { get; set; }

        public GameViewModel()
        {
            ExternalLinks = new List<ExternalLinkBaseViewModel>();
        }
    }
}