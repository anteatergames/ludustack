using FluentValidation;
using System;

namespace LuduStack.Domain.Messaging
{
    public class AddCommentUserContentCommandValidation : UserContentValidation<AddCommentUserContentCommand>
    {
        public AddCommentUserContentCommandValidation()
        {
            ValidateId();
            ValidateUserId();
            ValidateText();
        }

        protected void ValidateUserId()
        {
            RuleFor(c => c.UserId)
                .NotEqual(Guid.Empty);
        }

        protected void ValidateText()
        {
            RuleFor(c => c.Text)
                .NotEmpty();
        }
    }
}