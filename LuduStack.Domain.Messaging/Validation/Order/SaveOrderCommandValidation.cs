using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveOrderCommandValidation : BaseCommandValidation<SaveOrderCommand>
    {
        public SaveOrderCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.Order)
                .NotNull()
                .WithMessage("No Order to save.");
        }
    }
}