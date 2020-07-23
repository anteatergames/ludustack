using LuduStack.Domain.Core.Enums;

namespace LuduStack.Application.ViewModels.Giveaway
{
    public class GiveawayEntryOptionViewModel : BaseViewModel
    {
        public GiveawayEntryType Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Url { get; set; }

        public int Points { get; set; }

        public bool IsMandatory { get; set; }
    }
}