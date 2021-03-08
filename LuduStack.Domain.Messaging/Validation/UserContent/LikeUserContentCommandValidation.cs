using FluentValidation;
using System;

namespace LuduStack.Domain.Messaging
{
    public class LikeUserContentCommandValidation : UserContentValidation<LikeUserContentCommand>
    {
        public LikeUserContentCommandValidation()
        {
            ValidateId();
            ValidateUserId();
        }

        protected void ValidateUserId()
        {
            RuleFor(c => c.UserId)
                .NotEqual(Guid.Empty);
        }
    }
}