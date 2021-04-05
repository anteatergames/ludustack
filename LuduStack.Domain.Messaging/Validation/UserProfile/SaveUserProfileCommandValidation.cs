using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveUserProfileCommandValidation : BaseCommandValidation<SaveUserProfileCommand>
    {
        public SaveUserProfileCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.UserProfile)
                .NotNull()
                .WithMessage("No User Profile to save.");
        }
    }
}