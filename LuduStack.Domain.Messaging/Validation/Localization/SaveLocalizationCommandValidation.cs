using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveLocalizationCommandValidation : BaseCommandValidation<SaveLocalizationCommand>
    {
        public SaveLocalizationCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.Localization)
                .NotNull()
                .WithMessage("No Localization to save.");
        }
    }
}