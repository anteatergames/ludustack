using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayParticipantViewModel : BaseViewModel
    {
        public string Email { get; set; }

        public string ReferralCode { get; set; }

        public string ShortUrl { get; set; }

        public bool GdprConsent { get; set; }

        public bool WantNotifications { get; set; }

        public bool IsWinner { get; set; }

        public string SecretReceived { get; set; }

        public List<GiveawayEntryViewModel> Entries { get; set; }

        #region Extra

        public bool EmailVerified { get; set; }

        public string EmailMasked
        {
            get
            {
                string pattern = @"(?<=[\w]{1})[\w-\._\+%]*(?=[\w]{1}@)";
                string result = Regex.Replace(Email, pattern, m => new string('*', m.Length));
                return result;
            }
        }

        #endregion Extra
    }
}