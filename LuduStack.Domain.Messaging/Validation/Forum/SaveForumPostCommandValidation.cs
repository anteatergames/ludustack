using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class SaveForumPostCommandValidation : BaseCommandValidation<SaveForumPostCommand>
    {
        public SaveForumPostCommandValidation()
        {
            ValidateEntity();
            ValidateTitle();
            ValidateContent();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.ForumPost)
                .NotNull()
                .WithMessage("No Forum Post to save.");
        }

        protected void ValidateTitle()
        {
            RuleFor(c => c.ForumPost.Title)
                .NotNull()
                .WithMessage("Your topic must have a title!")
                .MinimumLength(2)
                .WithMessage("You must type at least two characters!")
                .When(x => x.ForumPost.IsOriginalPost);
        }

        protected void ValidateContent()
        {
            RuleFor(c => c.ForumPost.Content)
                .NotNull()
                .WithMessage("You typed nothing!")
                .MinimumLength(2)
                .WithMessage("You must type at least two characters!");
        }
    }
}