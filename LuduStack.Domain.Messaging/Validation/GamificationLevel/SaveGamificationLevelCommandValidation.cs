using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveGamificationLevelCommandValidation : BaseCommandValidation<SaveGamificationLevelCommand>
    {
        public SaveGamificationLevelCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.GamificationLevel)
                .NotNull()
                .WithMessage("No Gamification Level to save.");
        }
    }
}