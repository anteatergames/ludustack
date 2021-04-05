using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveUserContentCommandValidation : BaseCommandValidation<SaveUserContentCommand>
    {
        public SaveUserContentCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.UserContent)
                .NotNull()
                .WithMessage("No User Content to save.");
        }
    }
}