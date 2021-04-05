using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveUserPreferencesCommandValidation : BaseCommandValidation<SaveUserPreferencesCommand>
    {
        public SaveUserPreferencesCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.UserPreferences)
                .NotNull()
                .WithMessage("No User Preferences to save.");
        }
    }
}