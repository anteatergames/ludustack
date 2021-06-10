using System.Globalization;

namespace LuduStack.Application.ViewModels.Poll
{
    public class PollOptionViewModel : BaseViewModel
    {
        public string Text { get; set; }

        public string Image { get; set; }

        public int Votes { get; set; }

        public decimal VotePercentage { get; set; }

        public string VotePercentageText => VotePercentage.ToString("n2", new CultureInfo("en-us"));

        public bool CurrentUserVoted { get; set; }
    }
}