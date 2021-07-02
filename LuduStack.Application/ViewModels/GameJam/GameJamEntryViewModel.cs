using LuduStack.Application.ViewModels.Game;
using System;
using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.GameJam
{
    public class GameJamEntryViewModel : UserGeneratedBaseViewModel
    {
        public bool LateSubmission { get; set; }

        public bool IsTeam { get; set; }

        public Guid GameJamId { get; set; }

        public DateTime JoinDate { get; set; }

        public DateTime SubmissionDate { get; set; }

        public Guid GameId { get; set; }

        public Guid? TeamId { get; set; }

        public int FinalPlace { get; set; }

        public List<GameJamVoteViewModel> Votes { get; set; }

        public int SecondsToCountDown { get; set; }

        public string Title { get; set; }

        public string FeaturedImage { get; set; }

        public GameJamViewModel GameJam { get; set; }

        public GameViewModel Game { get; set; }

        public bool Submitted { get { return SubmissionDate != default; } }
    }
}