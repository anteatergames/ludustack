using FluentValidation;
using System;

namespace LuduStack.Domain.Messaging
{
    public class SaveBillRateCommandValidation : BaseUserCommandValidation<SaveBillRateCommand, Guid>
    {
        public SaveBillRateCommandValidation()
        {
            ValidateUserId();
            ValidateGameElement();
        }

        protected void ValidateGameElement()
        {
            RuleFor(c => c.GameElement)
                .NotNull()
                .WithMessage("No Game Element is set.");
        }
    }
}