using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveProductCommandValidation : BaseCommandValidation<SaveProductCommand>
    {
        public SaveProductCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.Product)
                .NotNull()
                .WithMessage("No Product to save.");
        }
    }
}