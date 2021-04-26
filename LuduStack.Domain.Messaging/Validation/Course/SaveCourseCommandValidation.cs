using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveCourseCommandValidation : BaseCommandValidation<SaveCourseCommand>
    {
        public SaveCourseCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.Course)
                .NotNull()
                .WithMessage("No Course to save.");
        }
    }
}