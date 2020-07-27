using LuduStack.Application.Interfaces;
using System;
using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayParticipationViewModel : GiveawayBasicInfoViewModel, IGiveawayScreenViewModel
    {
        public List<GiveawayPrizeViewModel> Prizes { get; set; }

        public List<GiveawayEntryOptionViewModel> EntryOptions { get; set; }

        public List<GiveawayParticipantViewModel> Participants { get; set; }

        #region Extra

        public bool EmailConfirmed { get; set; }

        public bool CanCountDown { get; set; }

        public bool CanReceiveEntries { get; set; }

        public string StatusMessage { get; set; }

        public int SecondsToEnd { get; set; }

        public bool ShowSponsor { get; set; }

        public bool ShowTimeZone { get; set; }

        public bool Future { get; set; }

        public int EntryCount { get; set; }

        public Guid ParticipantId { get; internal set; }

        #endregion Extra

        public GiveawayParticipationViewModel()
        {
            EntryOptions = new List<GiveawayEntryOptionViewModel>();
        }
    }
}