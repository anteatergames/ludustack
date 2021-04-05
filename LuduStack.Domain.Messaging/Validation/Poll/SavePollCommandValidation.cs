using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SavePollCommandValidation : BaseCommandValidation<SavePollCommand>
    {
        public SavePollCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.Poll)
                .NotNull()
                .WithMessage("No Poll to save.");
        }
    }
}