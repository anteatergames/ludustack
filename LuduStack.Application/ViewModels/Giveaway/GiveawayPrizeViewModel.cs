using System.Collections.Generic;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayPrizeViewModel : BaseViewModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string FeaturedImage { get; set; }

        public List<string> Secrets { get; set; }
    }
}