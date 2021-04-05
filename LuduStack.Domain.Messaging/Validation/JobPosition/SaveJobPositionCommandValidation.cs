using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveJobPositionCommandValidation : BaseCommandValidation<SaveJobPositionCommand>
    {
        public SaveJobPositionCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.JobPosition)
                .NotNull()
                .WithMessage("No Job Position to save.");
        }
    }
}