using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveGiveawayCommandValidation : BaseCommandValidation<SaveGiveawayCommand>
    {
        public SaveGiveawayCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.Giveaway)
                .NotNull()
                .WithMessage("No Giveaway to save.");
        }
    }
}