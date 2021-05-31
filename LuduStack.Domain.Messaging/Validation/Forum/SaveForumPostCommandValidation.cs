using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveForumPostCommandValidation : BaseCommandValidation<SaveForumPostCommand>
    {
        public SaveForumPostCommandValidation()
        {
            ValidateEntity();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.ForumPost)
                .NotNull()
                .WithMessage("No Forum Post to save.");
        }
    }
}