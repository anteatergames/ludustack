using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SavePlansCommandValidation : CourseValidation<SavePlansCommand>
    {
        public SavePlansCommandValidation()
        {
            ValidateId();
        }

        protected void ValidatePlans()
        {
            RuleFor(x => x.Plans)
                .NotEmpty()
                .WithMessage("You need to add plans to save");
        }
    }
}