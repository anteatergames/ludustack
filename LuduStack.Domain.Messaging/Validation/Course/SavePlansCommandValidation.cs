using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SavePlansCommandValidation : BaseUserCommandValidation<SavePlansCommand>
    {
        public SavePlansCommandValidation()
        {
            ValidateId();
            ValidateUserId();
            ValidatePlans();
        }

        protected void ValidatePlans()
        {
            RuleFor(x => x.Plans)
                .NotEmpty()
                .WithMessage("You need to add plans to be able to save.");
        }
    }
}