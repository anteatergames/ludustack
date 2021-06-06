using LuduStack.Application.Interfaces;
using LuduStack.Domain.Core.Enums;
using LuduStack.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LuduStack.Application.ViewModels.Brainstorm
{
    public class BrainstormIdeaViewModel : UserGeneratedCommentBaseViewModel, ICommentableItem
    {
        public Guid SessionId { get; set; }

        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Vote Count")]
        public int VoteCount { get; set; }

        [Display(Name = "Score")]
        public int Score { get; internal set; }

        public VoteValue CurrentUserVote { get; set; }

        public BrainstormIdeaStatus Status { get; set; }

        public List<UserVoteVo> Votes { get; set; }
    }
}