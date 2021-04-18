using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveGamificationCommandValidation : BaseCommandValidation<SaveGamificationCommand>
    {
        public SaveGamificationCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.Gamification)
                .NotNull()
                .WithMessage("No Gamification to save.");
        }
    }
}