using FluentValidation;

namespace LuduStack.Domain.Messaging
{
    public class AddCommentUserContentCommandValidation : BaseUserCommandValidation<AddCommentUserContentCommand>
    {
        public AddCommentUserContentCommandValidation()
        {
            ValidateId();
            ValidateUserId();
            ValidateText();
        }

        protected void ValidateText()
        {
            RuleFor(c => c.Text)
                .NotEmpty()
                .WithMessage("You can't send an empty comment.");
        }
    }
}