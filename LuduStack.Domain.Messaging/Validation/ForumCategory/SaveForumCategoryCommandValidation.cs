using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveForumCategoryCommandValidation : BaseCommandValidation<SaveForumCategoryCommand>
    {
        public SaveForumCategoryCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.ForumCategory)
                .NotNull()
                .WithMessage("No Forum Category to save.");
        }
    }
}