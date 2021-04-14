using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class AddBrainstormIdeaCommentCommandValidation : BaseUserCommandValidation<AddBrainstormIdeaCommentCommand>
    {
        public AddBrainstormIdeaCommentCommandValidation()
        {
            ValidateUserId();
            ValidateEntity();
            ValidateText();
        }

        protected void ValidateEntity()
        {
            RuleFor(c => c.BrainstormComment)
                .NotNull()
                .WithMessage("No Comment to save.");
        }

        protected void ValidateText()
        {
            RuleFor(c => c.BrainstormComment.Text)
                .NotNull()
                .WithMessage("No Text to save.");
        }
    }
}