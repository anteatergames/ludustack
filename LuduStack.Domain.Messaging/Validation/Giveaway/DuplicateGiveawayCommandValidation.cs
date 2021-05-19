using LuduStack.Domain.Models;

namespace LuduStack.Domain.Messaging
{
    public class DuplicateGiveawayCommandValidation : BaseCommandValidation<DuplicateGiveawayCommand, Giveaway>
    {
        public DuplicateGiveawayCommandValidation()
        {
            ValidateId();
        }
    }
}