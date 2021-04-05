using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveFeaturedContentCommandValidation : BaseCommandValidation<SaveFeaturedContentCommand>
    {
        public SaveFeaturedContentCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.FeaturedContent)
                .NotNull()
                .WithMessage("No Featured Content to save.");
        }
    }
}