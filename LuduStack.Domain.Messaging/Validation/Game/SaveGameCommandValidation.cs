using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveGameCommandValidation : BaseCommandValidation<SaveGameCommand>
    {
        public SaveGameCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.Game)
                .NotNull()
                .WithMessage("No Game to save.");
        }
    }
}